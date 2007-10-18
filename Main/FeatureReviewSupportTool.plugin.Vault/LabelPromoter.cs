using System;
using System.Data;
using System.Diagnostics;

using VaultClientOperationsLib;
using VaultClientNetLib;
using VaultLib;
using VaultClientNetLib.ClientService;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
	/// <summary>
	/// Summary description for LabelPromoter.
	/// </summary>
	public class LabelPromoter : VaultClientConsumer
	{
        const int maxQuerySize = 100000;

        VaultClientFolder root;

        public LabelPromoter( VaultRepositoryAuthSettings settings ) : 
            this( settings.Host, settings.Repository, settings.User, settings.Password )
        {
        }

        public LabelPromoter( string server, string repository, string username, string password )
        {
            base.server = server;
            base.repository = repository;
            base.username = username;
            base.password = password;
            EstablishConnection();

            root = Client.TreeCache.Repository.Root;
        }

        public void PromoteLabelVersion( string filePath, string label, long newVersion )
        {
            VaultClientFile file = root.FindFileRecursive( filePath );
            file.Version = newVersion;
            
            VaultLabelPromotionItem item = new VaultLabelPromotionItem();

            item.ChangeType = VaultLabelPromotionChangeType.Modify;
            item.ItemName = file.Name;
            item.ItemPath = file.FullPath;
            item.Version = file.Version;
            item.ID = file.ID;

            long labelId = GetLabelId( root, label );
            item.Version = newVersion;
            SynchObVerId( item );

            long currentLabeledVersion = findVersionWithLabel( file, label );

            // Move
            if( currentLabeledVersion != -1 )
            {
                item.ChangeType = VaultLabelPromotionChangeType.Modify;
            }
            else // Add
            {
                item.ChangeType = VaultLabelPromotionChangeType.Add;
            }

            VaultLabelPromotionItem [] items = new VaultLabelPromotionItem [] {item};

            DateTime lastDate = VaultDate.EmptyDate();
            int nIdxFailed = 0;
            string conflict = null;

            int promoteResult = Client.PromoteLabelItems( root.FullPath, labelId, label, ref lastDate, items, out nIdxFailed, out conflict );
            string resultString = VaultConnection.GetSoapExceptionMessage( promoteResult );
        }

        long GetLabelId( VaultClientFolder folder, string label )
        {
            long labelId = 0;
            string [] discoveredSubItemPaths = null;
            DateTime lastModified = VaultDate.EmptyDate();
            VaultRepositoryDelta repDelta = null;
            VaultFileDelta fileDelta = null;
            long rootId = 0;

            try
            {
                Client.GetLabelStructureByName( label, ref labelId, folder.FullPath, string.Empty, out discoveredSubItemPaths, out lastModified, out repDelta, out fileDelta, out rootId );
            }
            catch (Exception )
            {
                // No label on item
                return -1;
            }
            return labelId;
        }

        void SynchObVerId( VaultLabelPromotionItem item )
        {
            VaultObjectVersionInfo[] ovis = null;
            Client.GetObjectVersionList( item.ID, ref ovis, false);

            item.ObjVerID = -1;

            foreach( VaultObjectVersionInfo ovi in ovis )
            {
                if( ovi.Version == item.Version )
                {
                    item.ObjVerID = ovi.ObjVerID;
                    return;
                }
            }
        }
        long findVersionWithLabel( VaultClientFile file, string label )
        {
            string token;
            int inheritedRowCount;
            int recursiveRowCount;
            int maxQuerySize = 5000;
            Client.BeginLabelQuery( file.FullPath, file.ID, false, true, true, true, maxQuerySize, out inheritedRowCount, out recursiveRowCount, out token );

            if( inheritedRowCount < 1 )
            {
                Client.EndLabelQuery( token );
                return -1;
            }

            VaultLabelItemX [] items;
            Client.GetLabelQueryItems_Main( token, 0, inheritedRowCount, out items );
            
            Client.EndLabelQuery( token );

            foreach( VaultLabelItemX x in items )
            {
                if( x.Label == label )
                {
                    return x.Version;
                }
            }

            return -1;
        }
    }
}
