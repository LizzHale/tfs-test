using System;
using System.Collections;
using System.Data;

using VaultLib;
using VaultClientOperationsLib;

using FeatureReviewSupportTool.plugin.Vault.HistoryQuery;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public class FeatureTagHistoryQuery : IVaultHistoryQuery
    {
        public ChangeHistoryDataSet GetVersions( string selector, VaultRepositoryAuthSettings connectionSettings )
        {
            return GetVersions( HistoryQueryHelper.GetTaskTags( selector ), connectionSettings );
        }

        private ChangeHistoryDataSet GetVersions( string[] featureTags, VaultRepositoryAuthSettings connectionSettings )
        {
            using( VaultClientHelper client = new VaultClientHelper( connectionSettings ) )
            {
                VaultHistoryQueryRequest query = PrepareQuery( client );
                VaultHistoryDataSet versions = new VaultHistoryDataSet();

                foreach( string featureTag in featureTags )
                {
                    query.CommentSubstring = featureTag;
                    MergeResults( client, versions, Process( client, query ), featureTag );
                }

                return versions;
            }
        }

        private VaultHistoryQueryRequest PrepareQuery( VaultClientHelper client )
        {
            VaultHistoryQueryRequest history = new VaultHistoryQueryRequest();
            
            history.BeginDate = VaultDate.EmptyDate();
            history.EndDate = VaultDate.EmptyDate();

            history.DateFilterMask = VaultQueryRequestDates.DoNotFilter;
            history.CommentFilter = VaultQueryRequestComments.FilteredComment;

            history.Recursive = true;
            history.RepID = client.Client.ActiveRepositoryID;

            VaultClientFolder vcfolder = client.Client.TreeCache.Repository.Root.FindFolderRecursive( "$" );
            history.TopName = vcfolder.FullPath;
            history.TopID = vcfolder.ID;
            history.IsFolder = true;

            // set the sort order
            history.Sorts = new long [] {
                                            (long) (VaultQueryRequestSort.NameSort    | VaultQueryRequestSort.AscSort),
                                            (long) (VaultQueryRequestSort.VersionSort | VaultQueryRequestSort.AscSort),
            };

            return history;
        }

        private VaultHistoryItem [] Process( VaultClientHelper client, VaultHistoryQueryRequest query )
        {
            // Start the query
            int nRowsRetrieved = 0;
            string strQryToken = null;
            VaultHistoryItem[] histitems = null;

            client.Client.Connection.HistoryBegin( query, VaultClientHelper.MaxQuerySize, ref nRowsRetrieved, ref strQryToken );

            if( nRowsRetrieved > 0 )
            {
                client.Client.Connection.HistoryFetch( strQryToken, 0, nRowsRetrieved - 1, ref histitems );
            }

            client.Client.Connection.HistoryEnd( strQryToken );

            return histitems;
        }

        private void MergeResults( VaultClientHelper client, VaultHistoryDataSet destination, VaultHistoryItem [] history, string featureTag )
        {
            if( history == null )
                return;

            foreach( VaultHistoryItem item in history )
            {
                if( HistoryQueryHelper.IsChangeRelatedToTask( item.Comment, featureTag ) )
                {
                    destination.Add( client, item );
                }
            }
        }
    }
}
