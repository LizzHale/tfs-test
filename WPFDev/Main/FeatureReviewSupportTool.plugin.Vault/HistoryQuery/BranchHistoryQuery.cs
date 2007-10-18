using System;
using System.Collections;
using System.Data;

using VaultLib;
using VaultClientOperationsLib;

using FeatureReviewSupportTool.plugin.Vault.HistoryQuery;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public class BranchHistoryQuery : IVaultHistoryQuery
    {
        public ChangeHistoryDataSet GetVersions( string featureBranch, VaultRepositoryAuthSettings connectionSettings )
        {
            using( VaultClientHelper client = new VaultClientHelper( connectionSettings ) )
            {
                VaultHistoryDataSet results = new VaultHistoryDataSet( featureBranch );
                foreach( VaultTxHistoryItem changeSet in GetChangeSets( client, featureBranch ) )
                {
                    foreach( VaultHistoryItemBase change in GetChanges( client, changeSet ) )
                    {
                        if( change != null )
                        {
                            // Don't query promotion info because it's done via branches, not labels.
                            results.Add( client, change );
                        }
                    }
                }

                return results;
            }
        }

        private VaultTxHistoryItem [] GetChangeSets( VaultClientHelper client, string featureBranch )
        {
            int count = 0;
            string queryToken = null;
            client.Client.Connection.VersionHistoryBegin( VaultClientHelper.MaxQuerySize, client.Client.ActiveRepositoryID, GetFolderId( client, featureBranch ), VaultDate.EmptyDate( ), VaultDate.EmptyDate( ), 0, ref count, ref queryToken );
            VaultTxHistoryItem [] historyItems = null;
            client.Client.Connection.VersionHistoryFetch( queryToken, 0, count - 1, ref historyItems );
            client.Client.Connection.VersionHistoryEnd( queryToken );
            return historyItems;
        }

        private VaultTxDetailHistoryItem [] GetChanges( VaultClientHelper client, VaultTxHistoryItem changeSet )
        {
            string changeSetComment = null;
            VaultTxDetailHistoryItem [] changes = { };
            client.Client.Connection.GetTxDetail( client.Client.ActiveRepositoryID, changeSet.TxID, ref changeSetComment, ref changes );
            FillInMissingComments( changes, changeSetComment );
            return changes;
        }

        private void FillInMissingComments( VaultTxDetailHistoryItem [] changes, string changeSetComment )
        {
            foreach( VaultTxDetailHistoryItem change in changes )
            {
                if( change.Comment == null || change.Comment == string.Empty )
                {
                    change.Comment = changeSetComment;
                }
            }
        }

        private long GetFolderId( VaultClientHelper client, string repositoryPath )
        {
            VaultClientFolder folder = client.Root.FindFolderRecursive( repositoryPath );
            if( folder == null )
            {
                throw new ApplicationException( "Could not find folder \"" + repositoryPath + "\"" );
            }
            return folder.ID;
        }
    }
}
