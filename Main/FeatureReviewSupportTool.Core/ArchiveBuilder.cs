using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using ICSharpCode.SharpZipLib.Zip;

using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportTool.Core
{
    public class ArchiveBuilder
    {
        private IScmSystemHook currentScmSystem;

        public ArchiveBuilder( IScmSystemHook currentScmSystem )
        {
            this.currentScmSystem = currentScmSystem;
        }

        public void Build( DataSet changeHistory, string fileName )
        {
            using( TemporaryDirectory revisionsDirectory = new TemporaryDirectory( ) )
            {
                RetrieveFiles( changeHistory, revisionsDirectory );
                MakeArchive( fileName, revisionsDirectory );
            }
        }

        private void RetrieveFiles( DataSet changeHistory, TemporaryDirectory revisionsDirectory )
        {
            foreach( DataRow row in changeHistory.Tables[ 0 ].Rows )
            {
                try
                {
                    object objectId = row[ ChangeHistoryDataSet.UniqueId ];
                    long firstVersion = (long) row[ ChangeHistoryDataSet.FirstVersion ];
                    long lastVersion = (long) row[ ChangeHistoryDataSet.LastVersion ];
                    string displayedPath = (string) row[ ChangeHistoryDataSet.DisplayedPath ];

                    RetrieveAndPlaceRevisionOfFile( objectId, firstVersion, revisionsDirectory, displayedPath );
                    RetrieveAndPlaceRevisionOfFile( objectId, lastVersion, revisionsDirectory, displayedPath );
                }
                catch( Exception e )
                {
                    Trace.WriteLine( e );
                }
            }
        }

        private void RetrieveAndPlaceRevisionOfFile( object objectId, long version, TemporaryDirectory tempDir, string displayedPath )
        {
            if( currentScmSystem.FileVersionExists( objectId, version ) )
            {
                DirectoryInfo newDirectory = Directory.CreateDirectory( tempDir.DirectoryPath
                    + Path.DirectorySeparatorChar
                    + Path.GetDirectoryName( currentScmSystem.TransformSourceControlPathToFilesystemPath( displayedPath ) ) );

                string pathToFile = currentScmSystem.GetFile( objectId, version, tempDir );

                if( pathToFile != String.Empty )
                {
                    File.Move( pathToFile,
                        newDirectory.FullName + Path.DirectorySeparatorChar + Path.GetFileName( pathToFile ) + "." + version );
                }
            }
        }

        private void MakeArchive( string archiveFileName, TemporaryDirectory revisionsDirectory )
        {
            ZipFile zipFile = ZipFile.Create( archiveFileName );
            try
            {
                zipFile.BeginUpdate( );

                foreach( string filePath in Directory.GetFiles( revisionsDirectory.DirectoryPath, "*", SearchOption.AllDirectories ) )
                {
                    zipFile.Add( filePath, filePath.Replace( revisionsDirectory.DirectoryPath, "" ) );
                }

                zipFile.CommitUpdate( );
            }
            catch( Exception e )
            {
                Trace.WriteLine( e );
                zipFile.AbortUpdate( );
            }
            finally
            {
                zipFile.Close( );
            }
        }
    }
}
