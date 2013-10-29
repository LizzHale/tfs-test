using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sep.ConfigurationManagement.Vault.CodeReview;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using FeatureReviewSupportTool.plugin.TeamFoundation.Properties;

namespace FeatureReviewSupportTool.TeamFoundation
{
    class TeamFoundationHistoryQuery
    {
        const string ReviewWorkItemType = "Review";
        const string TaskWorkItemType = "Task";

        TeamFoundationClientHelper clientHelper;

        public TeamFoundationHistoryQuery( TeamFoundationServer server ) : this( new TeamFoundationClientHelper( server ) )
        {
        }

        public TeamFoundationHistoryQuery( TeamFoundationClientHelper clientHelper )
        {
            this.clientHelper = clientHelper;
        }

        public ChangeHistoryDataSet GetVersions( string query, bool followReviewLinks, bool openCril, bool openWorkItem )
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( query );
            OpenExternalInformation( parsedQuery.WorkItemIds, openWorkItem, openCril );
            return CreateHistoryDataSet( GetChangesets( parsedQuery, followReviewLinks ) );
        }

        private ChangeHistoryDataSet CreateHistoryDataSet( List<Changeset> changesets )
        {
            ChangeHistoryDataSet versions = new ChangeHistoryDataSet();
            foreach( Changeset changeset in changesets )
            {
                Merge( versions, changeset );
            }
            return versions;
        }

        private void Merge( ChangeHistoryDataSet versions, Changeset changeset )
        {
            foreach( Change change in changeset.Changes )
            {
                if( change.Item.ItemType == ItemType.File )
                {
                    versions.Add( change.Item.ItemId,
                        change.Item.ServerItem,
                        changeset.ChangesetId,
                        clientHelper.LatestVersion,
                        changeset.CreationDate,
                        changeset.Committer,
                        BuildComment( changeset, change ) );
                }
            }
        }

        private string BuildComment( Changeset changeset, Change change )
        {
            return "[" + GetAssociatedWorkItems( changeset ) + "] " +
                "(" + change.ChangeType + ") " +
                changeset.Comment;
        }

        private string GetAssociatedWorkItems( Changeset changeset )
        {
            List<string> workItemIds = new List<string>();
            foreach( WorkItem workItem in changeset.WorkItems )
            {
                workItemIds.Add( workItem.Id.ToString() );
            }
            return string.Join( ",", workItemIds.ToArray() );
        }

        private List<Changeset> GetChangesets( ParsedQuery query, bool followReviewLinks )
        {
            List<Changeset> changesets = new List<Changeset>();
            changesets.AddRange( GetChangesets( GetDevelopmentTasks( query.WorkItemIds, followReviewLinks ) ) );
            if( query.HasVersions ) changesets.AddRange( GetChangesets( query.ServerPath, query.StartVersion, query.EndVersion ) );
            return changesets;
        }

        private List<Changeset> GetChangesets( List<WorkItem> workItems )
        {
            List<Changeset> changesets = new List<Changeset>();
            foreach( int changesetId in GetChangesetIds( workItems ) )
            {
                changesets.Add( clientHelper.VersionControl.GetChangeset( changesetId ) );
            }
            return changesets;
        }

        private List<Changeset> GetChangesets( string path, VersionSpec startVersionSpec, VersionSpec endVersionSpec )
        {
            VersionSpec refinedStartVersion = Refine( path, startVersionSpec, true );
            VersionSpec refinedEndVersion = Refine( path, endVersionSpec, false );
            List<Changeset> changesets = new List<Changeset>();
            foreach( Changeset changeset in clientHelper.VersionControl.QueryHistory( path, VersionSpec.Latest, 0, RecursionType.Full, null, refinedStartVersion, refinedEndVersion, Int32.MaxValue, true, false ) )
            {
                changesets.Add( changeset );
            }
            return changesets;
        }

        private VersionSpec Refine( string path, VersionSpec startVersionSpec, bool isStart )
        {
            if( startVersionSpec is LabelVersionSpec )
            {
                int changesetId = GetMostRecentChangesetId( path, (LabelVersionSpec) startVersionSpec );
                return MakeChangesetSpec( changesetId, isStart );
            }
                return startVersionSpec;
        }

        private VersionSpec MakeChangesetSpec( int changesetId, bool isStart )
        {
            return new ChangesetVersionSpec( changesetId + (isStart ? 1 : 0) );
        }

        private int GetMostRecentChangesetId( string path, LabelVersionSpec labelVersionSpec )
        {
            int mostRecentChangesetId = 0;
            foreach( VersionControlLabel label in clientHelper.VersionControl.QueryLabels( labelVersionSpec.Label, path, null, true ) )
            {
                foreach( Item item in label.Items )
                {
                    mostRecentChangesetId = Math.Max( item.ChangesetId, mostRecentChangesetId );
                }
            }
            return mostRecentChangesetId;
        }

        private IEnumerable<int> GetChangesetIds( List<WorkItem> workItems )
        {
            SortedList<int, object> changesetIds = new SortedList<int, object>();
            foreach( WorkItem workItem in workItems )
            {
                AddChangesets( changesetIds, workItem );
            }
            return changesetIds.Keys;
        }

        private void AddChangesets( SortedList<int, object> changesetIds, WorkItem workItem )
        {
            foreach( Link link in workItem.Links )
            {
                ExternalLink extLink = link as ExternalLink;
                if( extLink != null )
                {
                    ArtifactId artifact = LinkingUtilities.DecodeUri( extLink.LinkedArtifactUri );
                    if( String.Equals( artifact.ArtifactType, "Changeset", StringComparison.Ordinal ) )
                    {
                        changesetIds[Convert.ToInt32( artifact.ToolSpecificId )] = "don't care";
                    }
                }
            }
        }

        private void OpenExternalInformation( IList<int> workItemIds, bool openWorkItem, bool openCril )
        {
            foreach( int workItemId in workItemIds )
            {
                WorkItem workItem = clientHelper.WorkItemStore.GetWorkItem( workItemId );
                if( openWorkItem )
                {
                    OpenWorkItem( workItem );
                }
                if( openCril )
                {
                    if( workItem.Type.Name == ReviewWorkItemType )
                    {
                        OpenIssueLogs( workItem );
                    }
                }
            }
        }

        private List<WorkItem> GetDevelopmentTasks( IList<int> workItemIds, bool followReviewLinks )
        {
            List<WorkItem> workItems = new List<WorkItem>();
            foreach( int workItemId in workItemIds )
            {
                WorkItem workItem = clientHelper.WorkItemStore.GetWorkItem( workItemId );
                workItems.Add( workItem );
                if( followReviewLinks )
                {
                    if( workItem.Type.Name == ReviewWorkItemType )
                    {
                        IEnumerable<WorkItem> relatedWorkItems = GetAssociatedWorkItems( workItem );
                        workItems.AddRange( relatedWorkItems );
                        foreach( WorkItem relatedWorkItem in relatedWorkItems )
                        {
                            workItems.AddRange( GetAssociatedReviews( relatedWorkItem ) );
                        }
                    }
                    else
                    {
                        workItems.AddRange( GetAssociatedReviews( workItem ) );
                    }
                }
            }
            return workItems;
        }

        private void OpenWorkItem( WorkItem workItem )
        {
            string workItemUrl = "";
            try
            {
                workItemUrl = string.Format( Settings.Default.WorkItemUrlFormatString, workItem.Id );
                Process.Start( workItemUrl );
            }
            catch( Exception e )
            {
                Debug.WriteLine( "Failed to open work item " + workItem.Id + " (" + workItemUrl + ") in teamplain: " + e.Message );
            }
        }

        private void OpenIssueLogs( WorkItem workItem )
        {
            foreach( Link link in workItem.Links )
            {
                Hyperlink hyperlink = link as Hyperlink;
                if( hyperlink != null && hyperlink.Location.ToLower().EndsWith( "xls" ) )
                {
                    System.Threading.ThreadPool.QueueUserWorkItem( delegate( object state )
                    {
                        try
                        {
                            Debug.WriteLine( "Opening \"" + hyperlink.Location + "\"" );
                            Process.Start( "excel", "\"" + hyperlink.Location + "\"" ).WaitForExit();
                        }
                        catch( Exception e )
                        {
                            Debug.WriteLine( "Failed to open excel: " + e.Message );
                        }
                    } );
                }
            }
        }

        private IEnumerable<WorkItem> GetAssociatedReviews( WorkItem workItem )
        {
            return GetAssociatedWorkItems( workItem, ReviewWorkItemType );
        }

        private IEnumerable<WorkItem> GetAssociatedWorkItems( WorkItem workItem )
        {
            return GetAssociatedWorkItems( workItem, null );
        }

        private IEnumerable<WorkItem> GetAssociatedWorkItems( WorkItem source, string workItemType )
        {
            List<WorkItem> workItems = new List<WorkItem>();
            foreach( Link link in source.Links )
            {
                RelatedLink relatedLink = link as RelatedLink;
                if( relatedLink != null )
                {
                    WorkItem relatedWorkItem = clientHelper.WorkItemStore.GetWorkItem( relatedLink.RelatedWorkItemId );
                    if( workItemType == null || relatedWorkItem.Type.Name == workItemType )
                    {
                        workItems.Add( relatedWorkItem );
                    }
                }
            }
            return workItems;
        }
    }
}
