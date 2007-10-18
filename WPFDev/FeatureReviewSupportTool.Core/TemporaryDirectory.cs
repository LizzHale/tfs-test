using System;
using System.IO;
using System.Diagnostics;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public class TemporaryDirectory : IDisposable
    {
        private string tempDir;

        public TemporaryDirectory()
        {
            tempDir = Path.Combine( Path.GetTempPath(), "frt_" + Guid.NewGuid().ToString() );
            Directory.CreateDirectory( tempDir );
        }

        public string DirectoryPath
        {
            get { return tempDir; }
        }

        public void Dispose()
        {
            try
            {
                if( Directory.Exists( tempDir ) )
                    Directory.Delete( tempDir, true );

            }
            catch( Exception e )
            {
                Trace.WriteLine( "Error removing temp directory: " + e );
            }
        }
    }
}
