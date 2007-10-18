using System;
using System.Collections.Generic;
using System.Text;

using VaultClientOperationsLib;
using VaultLib;

using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportTool.plugin.Vault.HistoryQuery
{
    class VaultHistoryDataSet : ChangeHistoryDataSet
    {
        public VaultHistoryDataSet()
        {
        }

        public VaultHistoryDataSet( string pathToOmit )
            : this()
        {
            int charactersToDrop = pathToOmit.Length;
            if( ! pathToOmit.EndsWith( "/" ) )
            {
                charactersToDrop ++;
            }
            Table.Columns[ DisplayedPath ].ExtendedProperties[ "truncate.expression" ] =
                string.Format( "SUBSTRING( {0}, {1} + 1, LEN( {0} ) - {1} )", RepositoryPath, charactersToDrop );
        }

        public void Add( VaultClientHelper client, VaultHistoryItemBase historyItem )
        {
            VaultClientFile file = client.Root.FindFileRecursive( historyItem.ID );
            if( file == null )
                return;

            string fullPath = file.FullPath;

            Add( fullPath, fullPath, historyItem.Version, file.Version, historyItem.TxDate, historyItem.UserName, historyItem.Comment );
        }

        private int FindVersionWithLabel( ClientInstance client, VaultClientFile file, string label )
        {
            string token;
            int inheritedRowCount;
            int recursiveRowCount;
            client.BeginLabelQuery( file.FullPath, file.ID, false, true, true, true, VaultClientHelper.MaxQuerySize, out inheritedRowCount, out recursiveRowCount, out token );

            if( inheritedRowCount < 1 )
            {
                client.EndLabelQuery( token );
                return -1;
            }

            VaultLabelItemX[] items;
            client.GetLabelQueryItems_Main( token, 0, inheritedRowCount, out items );

            client.EndLabelQuery( token );

            foreach( VaultLabelItemX x in items )
            {
                if( x.Label == label )
                {
                    return (int) x.Version;
                }
            }

            return -1;
        }
    }
}
