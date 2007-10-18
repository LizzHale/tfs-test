using System;

using VaultClientOperationsLib;
using VaultClientNetLib;
using VaultLib;
using VaultClientNetLib.ClientService;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
	/// <summary>
	/// Summary description for VaultClientConsumer.
	/// </summary>
	public class VaultClientConsumer
	{
		public VaultClientConsumer()
		{
		}

        public string Server
        {
            get
            {
                return this.server;
            }
        }
        protected string server;

        public string Repository
        {
            get
            {
                return this.repository;
            }
        }
        protected string repository;
        

        protected string username;
        protected string password;

        protected ClientInstance Client
        {
            get
            {
                return this.client;
            }
        }
        private ClientInstance client;
        
        public VaultClientFolder Root
        {
            get
            {
                return this.root;
            }
        }
        private VaultClientFolder root;
        
        protected void EstablishConnection()
        {
            if (server.StartsWith("http://") == false)
                server = "http://" + server;

            if (server.EndsWith("/VaultService") == false)
                server += "/VaultService";

            ClientInstance myClient = new ClientInstance();
            myClient.Init(VaultClientNetLib.VaultConnection.AccessLevelType.Client);
            myClient.Login(server, username, password);
            
            FindRepository(myClient, repository);

            client = myClient;

            root = Client.TreeCache.Repository.Root;
        }

        #region API Helpers
        /// <summary>
        /// This is a helper function that works around a deficiency in the 
        /// Vault Client API.  In the API, there is no method that lets you set the active repository
        /// for a ClientInstance based only on the repository name.  This means that (for now) every
        /// program that uses the API will need to do something like this.
        /// </summary>
        /// <param name="client">A Client Instance that has already had Init and Login called on it.</param>
        /// <param name="repositoryName">The name of the repository to find and connect to.</param>
        void FindRepository(ClientInstance client, string repositoryName)
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
        #endregion
	}
}
