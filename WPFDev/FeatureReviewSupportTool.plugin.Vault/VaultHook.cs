using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Sep.ConfigurationManagement.Vault.CodeReview;
using FeatureReviewSupportTool.plugin.Vault;
using FeatureReviewSupportTool.plugin.Vault.Properties;
using System.Diagnostics;

namespace FeatureReviewSupportTool
{
    class VaultHook : IScmSystemHook
    {
        private VaultConnectionSettings connectionSettingsControl;
        private FileRetriever fileRetriever;
        private VaultRepositoryAuthSettings connectionSettings = new VaultRepositoryAuthSettings();

        public VaultHook()
        {
            LoadSettings();      
        }

        public override string ToString()
        {
            return "Vault";
        }

        public Control ConnectionSettingsControl
        {
            get
            {
                if( connectionSettingsControl == null )
                {
                    connectionSettingsControl = new VaultConnectionSettings();
                    connectionSettingsControl.Bind( connectionSettings );
                }
                return connectionSettingsControl;
            }
        }

        public void SaveConnectionSettings( Form parentForm, CancelEventArgs e )
        {
            if( !connectionSettings.IsDirty )
                return;

            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                connectionSettings.User = Settings.Default.User;
                connectionSettings.EncryptedPassword = Settings.Default.EncryptedPassword;
                connectionSettings.Host = Settings.Default.Host;
                connectionSettings.Repository = Settings.Default.Repository;
            }
            catch( Exception e )
            {
                Trace.WriteLine( "Unable to load vault settings:" );
                Trace.WriteLine( e );
            }
            finally
            {
                connectionSettings.WipeOffTheMud();
            }
        }

        private void SaveSettings()
        {
            Settings.Default.User = connectionSettings.User;
            Settings.Default.EncryptedPassword = connectionSettings.EncryptedPassword;
            Settings.Default.Host = connectionSettings.Host;
            Settings.Default.Repository = connectionSettings.Repository;
            Settings.Default.Save();
        }

        public void SaveExternalToolsSettings( )
        {
            ToolsSettings.Default.Save( );
        }

        public Core.ExternalToolsSettings ExternalToolsSettings
        {
            get
            {
                if( ToolsSettings.Default.ExternalTools == null )
                {
                    ToolsSettings.Default.ExternalTools = new FeatureReviewSupportTool.Core.ExternalToolsSettings( );
                }

                return ToolsSettings.Default.ExternalTools;
            }
        }     
        
        public ChangeHistoryDataSet GetVersions( string query )
        {
            IVaultHistoryQuery history = GetHistoryQuerier( query );
            return history.GetVersions( query, connectionSettings );
        }

        private IVaultHistoryQuery GetHistoryQuerier( string query )
        {
            return (query.StartsWith( "$" ) ?
                (IVaultHistoryQuery) new BranchHistoryQuery() :
                (IVaultHistoryQuery) new FeatureTagHistoryQuery());
        }

        #region Show Blame, Diff, File

        public bool CanShowBlame
        {
            get
            {
                return true;
            }
        }

        public void ShowBlame( object repositoryId, long numberOfVersionsBack )
        {
            HtmlBlame.ShowHtmlBlame( connectionSettings, (string) repositoryId, numberOfVersionsBack );
        }

        public void ShowFile( object repositoryId, long version )
        {
            DiffTwoFileVersions diff = new DiffTwoFileVersions( connectionSettings, ExternalToolsSettings );
            diff.ShowFile( (string) repositoryId, version );
        }

        public bool CanGetFile
        {
            get { return true; }
        }        

        public string GetFile( object repositoryId, long version, TemporaryDirectory tempDir )
        {
            if( fileRetriever == null )
            {
                fileRetriever = new FileRetriever( this.connectionSettings );
            }
            return fileRetriever.GetFile( (string) repositoryId, version, tempDir );                    
        }
        
        public bool FileVersionExists( object repositoryId, long version )
        {
            if( fileRetriever == null )
            {
                fileRetriever = new FileRetriever( this.connectionSettings );
            }
            return fileRetriever.CanGetFile( (string) repositoryId, version );
        }
        
        public string TransformSourceControlPathToFilesystemPath( string sourceControlPath )
        {
            return sourceControlPath.Substring( sourceControlPath.IndexOf( "$" ) + 1 );
        }        

        public void ShowDiff( object repositoryId, long firstVersion, long lastVersion )
        {
            DiffTwoFileVersions diff = new DiffTwoFileVersions( connectionSettings, ExternalToolsSettings );
            diff.ShowDiff( (string) repositoryId, Math.Max( 1, firstVersion ), lastVersion );
        }

        #endregion
    }
}
