using System;

using VaultClientOperationsLib;
using VaultLib;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public class VaultClientHelper : IDisposable
    {
        public const int MaxQuerySize = 100000;

        private ClientInstance vaultClient;

        public VaultClientHelper( VaultRepositoryAuthSettings connectionSettings )
        {
            vaultClient = Login( connectionSettings );
        }

        public ClientInstance Client
        {
            get { return vaultClient; }
        }

        public VaultClientFolder Root
        {
            get { return Client.TreeCache.Repository.Root; }
        }

        private static ClientInstance Login( VaultRepositoryAuthSettings connectionSettings )
        {
            ClientInstance myClient = new ClientInstance();
            myClient.Init( VaultClientNetLib.VaultConnection.AccessLevelType.Client );
            myClient.Login( connectionSettings.Url, connectionSettings.User, connectionSettings.Password );
            FindRepository( myClient, connectionSettings.Repository );
            return myClient;
        }

        /// <summary>
        /// This is a helper function that works around a deficiency in the 
        /// Vault Client API.  In the API, there is no method that lets you set the active repository
        /// for a ClientInstance based only on the repository name.  This means that (for now) every
        /// program that uses the API will need to do something like this.
        /// </summary>
        /// <param name="client">A Client Instance that has already had Init and Login called on it.</param>
        /// <param name="repositoryName">The name of the repository to find and connect to.</param>
        private static void FindRepository( ClientInstance client, string repositoryName )
        {
            VaultRepositoryInfo[] reps = null;
            //List all the repositories on the server.
            client.ListRepositories(ref reps);

            //Search for the one that we want.
            foreach (VaultRepositoryInfo r in reps)
            {
                if (String.Compare(r.RepName,repositoryName, true) == 0)
                {
                    //This will load up the client side cache files and refresh the repository structure.
                    //See http://support.sourcegear.com/viewtopic.php?t=6 for more on client side cache files.
                    client.SetActiveRepositoryID(r.RepID, client.Connection.Username, r.UniqueRepID, true, true);
                    break;
                }
            }
            if (client.ActiveRepositoryID == -1)
                throw new Exception(string.Format("Repository {0} not found", repositoryName));
        
        }

        #region IDisposable Members

        public void Dispose()
        {
            if( vaultClient != null )
            {
                vaultClient.Logout( );
                vaultClient = null;
            }
        }

        #endregion
    }
}
