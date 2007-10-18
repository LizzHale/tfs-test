// This file really needs to be refactored.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using FeatureReviewSupportTool.plugin.Vault;
using FeatureReviewSupportTool.Core;

using VaultClientOperationsLib;
using VaultClientNetLib;
using VaultClientNetLib.ClientService;
using VaultLib;

namespace  Sep.ConfigurationManagement.Vault.CodeReview
{
    public class DiffTwoFileVersions : VaultClientConsumer
    {
        private FileRetriever fileRetriever;
        private ExternalToolsSettings externalToolsSettings;
        
        public DiffTwoFileVersions( VaultRepositoryAuthSettings settings, ExternalToolsSettings externalToolsSettings ) : 
            this( settings.Host, settings.Repository, settings.User, settings.Password )
        {
            this.externalToolsSettings = externalToolsSettings;
            fileRetriever = new FileRetriever( settings );
        }

        public DiffTwoFileVersions( string server, string repository, string username, string password )
        {
            base.server = server;
            base.repository = repository;
            base.username = username;
            base.password = password;
            EstablishConnection();
        }

        private void ViewWorkItem( object args )
        {
            DiffParameters dp = (DiffParameters) args;
            DoShowFile( dp.fileRepositoryPath, dp.firstVersion );
        }

        private void DiffWorkItem( object args )
        {
            DiffParameters dp = (DiffParameters) args;
            DoDiff( dp.fileRepositoryPath, dp.firstVersion, dp.secondVersion);
        }

        public void ShowFile( string fileRepositoryPath, long version )
        {
            ThreadPool.QueueUserWorkItem( 
                new WaitCallback( ViewWorkItem ), 
                new DiffParameters( fileRepositoryPath, version, 0 ) );
        }

        public void ShowDiff( string fileRepositoryPath, long firstVersion, long secondVersion )
        {
            ThreadPool.QueueUserWorkItem( 
                new WaitCallback( DiffWorkItem ), 
                new DiffTwoFileVersions.DiffParameters( fileRepositoryPath, firstVersion, secondVersion ) );
        }

        private void DoShowFile( string fileRepositoryPath, long version )
        {
            using( TemporaryDirectory tempDir = new TemporaryDirectory() )
            {
                string filePath = fileRetriever.GetFile( fileRepositoryPath, version, tempDir );
                if( filePath != "" )
                {
                    ViewHelper.View( filePath, externalToolsSettings );
                }
            }
        }

        private void DoDiff( string fileRepositoryPath, long firstVersion, long secondVersion )
        {
            // We need two temp folders because we'll retrieve two files, which will probably have the
            // same name, and we don't want to overwrite the other.
            using( TemporaryDirectory tempDir1 = new TemporaryDirectory(),
                tempDir2 = new TemporaryDirectory() )
            {
                try
                {
                    string firstVersionFilePath = fileRetriever.GetFile( fileRepositoryPath, firstVersion, tempDir1 );
                    string secondVersionFilePath = fileRetriever.GetFile( fileRepositoryPath, secondVersion, tempDir2 );                    
                    
                    if (firstVersionFilePath == String.Empty || secondVersionFilePath == String.Empty)
                    {
                        return;
                    }

                    #region Diff the files

                    string diffApplication = "sgdm.exe";
                    string diffArgsFormatString = Client.UserOptions.GetString( VaultOptions.CustomDiff_Args );

                    // Process the arg format string to insert the correct paths and captions.
                    string processedArgString = Client.ReplaceDiffArgs(
                        diffArgsFormatString,
                        firstVersionFilePath,
                        Path.GetFileName( firstVersionFilePath ) + " " + firstVersion.ToString( ),
                        secondVersionFilePath,
                        Path.GetFileName( secondVersionFilePath ) + " " + secondVersion.ToString( ) );

                    Console.WriteLine( "Launching diff application." );
                    
                    Process diffProc = ExternalToolsHelper.GetDiffProgram(
                        firstVersionFilePath,
                        secondVersionFilePath,
                        diffApplication,
                        processedArgString,
                        externalToolsSettings);

                    diffProc.Start();

                    // Do we want to wait?  Probably not
                    diffProc.WaitForExit();

                    #endregion
                }
                catch( Exception e )
                {
                    // Intentionally squashed.
                    System.Diagnostics.Trace.WriteLine( e );
                }
            }
        }

        #region DiffParameters inner struct

        private struct DiffParameters
        {
            public DiffParameters( string f, long first, long second )
            {
                fileRepositoryPath = f;
                firstVersion = first;
                secondVersion = second;

                if( firstVersion == 0 ) firstVersion = -1;
            }

            public string fileRepositoryPath;
            public long firstVersion;
            public long secondVersion;
        }

        #endregion
    }
}
