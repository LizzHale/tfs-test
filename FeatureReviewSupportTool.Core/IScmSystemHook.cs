using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportTool
{
    public interface IScmSystemHook
    {
        Control ConnectionSettingsControl { get; }
        void SaveConnectionSettings( Form parentForm, CancelEventArgs e );

        Core.ExternalToolsSettings ExternalToolsSettings { get; }
        void SaveExternalToolsSettings();

        ChangeHistoryDataSet GetVersions( string query );              

        bool CanShowBlame { get; }
        
        void ShowBlame( object repositoryId, long numberOfVersionsBack );
        void ShowFile( object repositoryId, long version );
        
        bool CanGetFile { get; }
        
        bool FileVersionExists( object repositoryId, long version );
        string GetFile( object repositoryId, long version, TemporaryDirectory tempDir );
        string TransformSourceControlPathToFilesystemPath( string sourceControlPath );

        void ShowDiff( object repositoryId, long firstVersion, long lastVersion );
    }
}
