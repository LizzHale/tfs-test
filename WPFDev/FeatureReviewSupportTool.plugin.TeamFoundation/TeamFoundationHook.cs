using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

using Sep.ConfigurationManagement.Vault.CodeReview;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Client;
using FeatureReviewSupportTool.TeamFoundation;
using System.IO;
using System.Diagnostics;
using System.Threading;
using FeatureReviewSupportTool.plugin.TeamFoundation.Properties;
using FeatureReviewSupportTool.Core;

namespace FeatureReviewSupportTool
{
    class TeamFoundationHook : IScmSystemHook
    {
        private TeamFoundationConnectionSettingsControl connectionSettingsControl;

        public override string ToString( )
        {
            return "MS Team Foundation";
        }

        public Control ConnectionSettingsControl
        {
            get
            {
                if( connectionSettingsControl == null )
                {
                    connectionSettingsControl = new TeamFoundationConnectionSettingsControl( );
                    connectionSettingsControl.TeamServerName = Settings.Default.TeamServerName;
                    connectionSettingsControl.FollowReviewLinks = Settings.Default.FollowReviewLinks;
                    connectionSettingsControl.OpenCril = Settings.Default.AutomaticallyOpenCril;
                    connectionSettingsControl.OpenWorkItem = Settings.Default.AutomaticallyOpenWorkItem;
                }

                return connectionSettingsControl;
            }
        }

        private TeamFoundationServer TeamServer
        {
            get
            {
                return connectionSettingsControl.TeamServer;
            }
        }

        public void SaveConnectionSettings( Form parentForm, CancelEventArgs e )
        {
            if( connectionSettingsControl != null )
            {
                Settings.Default.TeamServerName = connectionSettingsControl.TeamServerName;
                Settings.Default.FollowReviewLinks = connectionSettingsControl.FollowReviewLinks;
                Settings.Default.AutomaticallyOpenCril = connectionSettingsControl.OpenCril;
                Settings.Default.AutomaticallyOpenWorkItem = connectionSettingsControl.OpenWorkItem;
                Settings.Default.Save( );
            }
        }

        public void SaveExternalToolsSettings( )
        {
            ToolsSettings.Default.Save( );
        }

        public ExternalToolsSettings ExternalToolsSettings
        {
            get
            {
                if( ToolsSettings.Default.ExternalTools == null )
                {
                    ToolsSettings.Default.ExternalTools = new ExternalToolsSettings( );
                }

                return ToolsSettings.Default.ExternalTools;
            }
        }

        public ChangeHistoryDataSet GetVersions( string query )
        {
            TeamFoundationHistoryQuery tfsQuery = new TeamFoundationHistoryQuery( TeamServer );
            return tfsQuery.GetVersions( query, connectionSettingsControl.FollowReviewLinks, connectionSettingsControl.OpenCril, connectionSettingsControl.OpenWorkItem );
        }

        public bool CanShowBlame
        {
            get { return false; }
        }

        public void ShowBlame( object repositoryId, long numberOfVersionsBack )
        {
            throw new NotImplementedException( );
        }

        public void ShowFile( object repositoryId, long version )
        {
            ThreadPool.QueueUserWorkItem( delegate( object args )
            {
                using( TemporaryDirectory tempDir = new TemporaryDirectory( ) )
                {
                    try
                    {                        
                        string downloadedFile = GetFile( repositoryId, version, tempDir );
                        ViewHelper.View(downloadedFile, ExternalToolsSettings);
                    }
                    catch( Exception e )
                    {
                        Trace.WriteLine( e );
                    }
                }
            } );
        }
        
        public bool FileVersionExists( object repositoryId, long version )
        {
            return GetItem( Convert.ToInt32( repositoryId ), Convert.ToInt32( version ) ) != null;
        }

        public bool CanGetFile
        {
            get { return true; }
        }

        public string GetFile( object repositoryId, long version, TemporaryDirectory tempDir )
        {
            try
            {
                return DownloadFileVersion( repositoryId, version, tempDir );
            }
            catch( Exception e )
            {
                Trace.WriteLine( e );
                return null;
            }
        }

        public string TransformSourceControlPathToFilesystemPath( string sourceControlPath )
        {
            return sourceControlPath.Substring( sourceControlPath.IndexOf( "$" ) + 1 );
        }

        public void ShowDiff( object repositoryId, long firstVersion, long lastVersion )
        {
            ThreadPool.QueueUserWorkItem( delegate( object args )
            {
                using( TemporaryDirectory tempDir1 = new TemporaryDirectory( ),
                    tempDir2 = new TemporaryDirectory( ) )
                {
                    try
                    {
                        string firstFile = DownloadFileVersion( repositoryId, firstVersion, tempDir1 );
                        string secondFile = DownloadFileVersion( repositoryId, lastVersion, tempDir2 );

                        Process viewer = ExternalToolsHelper.GetDiffProgram(
                            firstFile,
                            secondFile,
                            @"C:\program files\Microsoft Visual Studio 8\Common7\IDE\diffmerge.exe",
                            string.Format( "{0} {1}", ExternalToolsHelper.Quote( firstFile ), ExternalToolsHelper.Quote( secondFile ) ),
                            ExternalToolsSettings
                            );

                        viewer.Start( );

                        viewer.WaitForExit( );
                    }
                    catch( Exception e )
                    {
                        Trace.WriteLine( e );
                    }
                }
            } );
        }

        private string DownloadFileVersion( object repositoryId, long version, TemporaryDirectory tempDir )
        {
            return DownloadFileVersion( Convert.ToInt32( repositoryId ), ( int ) version, tempDir );
        }

        private string DownloadFileVersion( int repositoryId, int version, TemporaryDirectory tempDir )
        {
            try
            {
                Item itemToDownload = GetItem( repositoryId, version );
                string downloadedFile = Path.Combine( tempDir.DirectoryPath, Path.GetFileName( itemToDownload.ServerItem ) );
                itemToDownload.DownloadFile( downloadedFile );
                return downloadedFile;
            }
            catch( Exception e )
            {
                throw new ApplicationException( string.Format( "Unable to download file {0} version {1}.", repositoryId, version ), e );
            }
        }

        private Item GetItem( int repositoryId, int version )
        {
            TeamFoundationClientHelper helper = new TeamFoundationClientHelper( TeamServer );
            return helper.VersionControl.GetItem( repositoryId, version );
        }

        private Item GetChangesetItem( Changeset changeset, string repositoryPath )
        {
            foreach( Change change in changeset.Changes )
            {
                if( change.Item.ServerItem == repositoryPath )
                {
                    return change.Item;
                }
            }
            throw new ApplicationException( "Could not find \"" + repositoryPath + "\" in changeset + " + changeset.ChangesetId + "." );
        }
    }
}
