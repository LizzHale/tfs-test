using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

using VaultClientOperationsLib;
using VaultClientNetLib;
using VaultLib;
using VaultClientNetLib.ClientService;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    /// <summary>
    /// </summary>
    [Obsolete( "Continue refactoring" )]
    public sealed class HtmlBlame : VaultClientConsumer
    {
        private HtmlBlame( VaultRepositoryAuthSettings settings ) : 
            this( settings.Host, settings.Repository, settings.User, settings.Password )
        {
        }

        private HtmlBlame( string server, string repository, string username, string password )
        {
            base.server = server;
            base.repository = repository;
            base.username = username;
            base.password = password;
            EstablishConnection();
        }

        public static void ShowHtmlBlame( VaultRepositoryAuthSettings settings, string repositoryPath, long version )
        {
            HtmlBlame blame = new HtmlBlame( settings );
            blame.Client.viewBlameInHTML( repositoryPath, version );
        }
    }
}
