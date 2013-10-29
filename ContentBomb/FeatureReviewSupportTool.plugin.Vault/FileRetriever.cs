using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using Sep.ConfigurationManagement.Vault.CodeReview;

using VaultClientOperationsLib;
using VaultClientNetLib;
using VaultClientNetLib.ClientService;
using VaultLib;

namespace FeatureReviewSupportTool.plugin.Vault
{
    internal class FileRetriever : VaultClientConsumer
    {
        public FileRetriever( VaultRepositoryAuthSettings settings )
            : this( settings.Host, settings.Repository, settings.User, settings.Password )
        {
        }

        public FileRetriever( string server, string repository, string username, string password )
        {
            base.server = server;
            base.repository = repository;
            base.username = username;
            base.password = password;
            EstablishConnection( );
        }

        public bool CanGetFile( string fileRepositoryPath, long version )
        {
            VaultClientFile unused;
            return CanGetFile( fileRepositoryPath, version, out unused );
        }

        public bool CanGetFile( string fileRepositoryPath, long version, out VaultClientFile clientFile )
        {
            clientFile = Client.TreeCache.Repository.Root.FindFileRecursive( fileRepositoryPath );

            if( clientFile == null )
            {
                Trace.WriteLine( "Couldn't find file {0} in the tree.", fileRepositoryPath );
                return false;
            }

            return true;
        }

        public string GetFile( string fileRepositoryPath, long version, TemporaryDirectory tempDir )
        {
            try
            {
                // Find the file by path in our current tree cache.
                VaultClientFile foundFile;
                if( !CanGetFile( fileRepositoryPath, version, out foundFile ) )
                {
                    return String.Empty;
                }

                // This is important: do not modify the VaultClientFile reference returned by FindFileRecursive,
                // as you will modify your tree cache (which will be saved to disk).  Make a copy of the
                // object and modify that one.
                VaultClientFile copy = new VaultClientFile( foundFile );

                // Modify the copy to represent the requested file version.
                // Files added in this feature will have an initial version of 0, which doesn't actually exist in the
                // Vault repository.  When we are asked to retrieve such a file, we bump the version number up to 1.
                copy.Version = Math.Max(1, version);

                // Get the file.
                Client.GetByDisplayVersionToNonWorkingFolder( copy, MakeWritableType.MakeAllFilesWritable, SetFileTimeType.Current, copy.Parent.FullPath, tempDir.DirectoryPath, null );

                return Path.Combine( tempDir.DirectoryPath, copy.Name );
            }
            catch( Exception e )
            {
                // Intentionally squashed.
                Trace.WriteLine( e );
                return String.Empty;
            }
        }
    }
}
