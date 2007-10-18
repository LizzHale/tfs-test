using System;
using System.Data;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public interface IVaultHistoryQuery
    {
        ChangeHistoryDataSet GetVersions( string selector, VaultRepositoryAuthSettings connectionSettings );
    }
}
