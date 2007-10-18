using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using FeatureReviewSupportTool.Core;
using FeatureReviewSupportTool.plugin.ClearCase.Properties;
using FeatureReviewSupportTool.ScmSystemHooks.ClearCase;
using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportTool
{
    class ClearCaseHook : IScmSystemHook
    {
        private ClearCaseConnectionSettings connectionSettingsControl;
        private ConnectionSettings settings = new ConnectionSettings();

        public override string ToString()
        {
            return "ClearCase";
        }

        public System.Windows.Forms.Control ConnectionSettingsControl
        {
            get
            {
                if( connectionSettingsControl == null )
                {
                    connectionSettingsControl = new ClearCaseConnectionSettings();
                    connectionSettingsControl.Bind( settings );
                }
                return connectionSettingsControl;
            }
        }
        
        public void SaveConnectionSettings( System.Windows.Forms.Form parentForm, System.ComponentModel.CancelEventArgs e )
        {
            settings.Save( );
        }
        
        public void SaveExternalToolsSettings(  )
        {
            ToolsSettings.Default.Save();                                    
        }

        public ExternalToolsSettings ExternalToolsSettings
        {
            get
            {
                if (ToolsSettings.Default.ExternalTools == null)
                {
                    ToolsSettings.Default.ExternalTools = new ExternalToolsSettings();
                }
                
                return ToolsSettings.Default.ExternalTools;
            }
        }

        public Sep.ConfigurationManagement.Vault.CodeReview.ChangeHistoryDataSet GetVersions( string query )
        {
            return new ClearcaseHistoryQuery( settings ).GetHistory( query );
        }

        public bool CanShowBlame
        {
            get { return true; }
        }

        public void ShowBlame( object repositoryId, long numberOfVersionsBack )
        {
            ThreadPool.QueueUserWorkItem( delegate
            {
                try
                {
                    using( TemporaryDirectory tempDir = new TemporaryDirectory() )
                    {
                        string tempFile = Path.Combine( tempDir.DirectoryPath, Path.GetFileName( repositoryId.ToString() ) );
                        ClientHelper.CmdExec( "annotate -out \"{1}\" {0}@@/main/LATEST", repositoryId, tempFile );
                        ViewHelper.View( tempFile, ExternalToolsSettings );
                    }
                }
                catch( Exception e )
                {
                    Trace.WriteLine( e );
                }
            } );
        }

        public void ShowFile( object repositoryId, long version )
        {
            ThreadPool.QueueUserWorkItem( delegate
            {
                try
                {
                    using( TemporaryDirectory tempDir = new TemporaryDirectory() )
                    {
                        string tempFile = GetFile( repositoryId, version, tempDir );
                        ViewHelper.View( tempFile, ExternalToolsSettings );
                    }
                }
                catch( Exception e )
                {
                    Trace.WriteLine( e );
                }
            } );
        }
        
        public bool CanGetFile
        {
            get { return true; }
        }            

        public string GetFile( object repositoryId, long version, TemporaryDirectory tempDir )
        {
            string tempFile = Path.Combine( tempDir.DirectoryPath, Path.GetFileName( repositoryId.ToString() ) );
            ClientHelper.CmdExec( "annotate -out \"{2}\" -fmt \"\" -nheader \"{0}@@/main/{1}\"", repositoryId, version, tempFile );
            return tempFile;
        }

        public bool FileVersionExists( object repositoryId, long version )
        {
            return !Directory.Exists( (string) repositoryId ) && version >= 0 && version < GetMaxVersion( repositoryId );
        }

        private long GetMaxVersion( object repositoryId )
        {
            return ParseHelper.GetVersionNumber( ClientHelper.CmdExec( "lshist -short -last 1 \"{0}\"", repositoryId ) );
        }        
        
        public string TransformSourceControlPathToFilesystemPath( string sourceControlPath )
        {
            return sourceControlPath;
        }

        public void ShowDiff( object repositoryId, long firstVersion, long lastVersion )
        {
            ThreadPool.QueueUserWorkItem( delegate
            {
                try
                {
                    using( TemporaryDirectory tempDir1 = new TemporaryDirectory( ), tempDir2 = new TemporaryDirectory( ) )
                    {
                        string tempFile1 = GetFile( repositoryId, firstVersion, tempDir1 );
                        string tempFile2 = GetFile( repositoryId, lastVersion, tempDir2 );

                        Process viewer = ExternalToolsHelper.GetDiffProgram(
                            tempFile1,
                            tempFile2,
                            String.Empty,
                            String.Empty,
                            ExternalToolsSettings );
                            
                        // If no suitable default tool exists, use the ClearCase diff tool.
                        if( viewer.StartInfo.FileName == String.Empty )
                        {
                            ClientHelper.CmdExec( "diff -g \"{0}@@/main/{1}\" \"{0}@@/main/{2}\"", repositoryId, firstVersion, lastVersion );
                        }
                        else
                        {
                            viewer.Start( );
                            viewer.WaitForExit( );
                        }
                    }                    
                }
                catch( Exception e )
                {
                    Trace.WriteLine( e );
                }
            } );
        }
    }
}
