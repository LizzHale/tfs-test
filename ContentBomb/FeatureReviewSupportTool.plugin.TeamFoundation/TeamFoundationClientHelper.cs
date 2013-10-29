using System;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace FeatureReviewSupportTool.TeamFoundation
{
    class TeamFoundationClientHelper
    {
        private string teamServerName;
        private TeamFoundationServer teamServer;
        private WorkItemStore workItemStore;
        private VersionControlServer versionControl;
        private int latestVersion;

        public TeamFoundationClientHelper( string teamServerName )
        {
            this.teamServerName = teamServerName;
        }   
        
        public TeamFoundationClientHelper( TeamFoundationServer server )
        {
            this.teamServer = server;
            this.teamServerName = server.Name;
        }

        public TeamFoundationServer TeamFoundationServer
        {
            get
            {
                if( teamServer == null )
                {
                    teamServer = TeamFoundationServerFactory.GetServer( teamServerName );
                }
                return teamServer;
            }
        }

        public WorkItemStore WorkItemStore
        {
            get
            {
                if( workItemStore == null )
                {
                    workItemStore = (WorkItemStore) TeamFoundationServer.GetService( typeof( WorkItemStore ) );
                }
                return workItemStore;
            }
        }

        public VersionControlServer VersionControl
        {
            get
            {
                if( versionControl == null )
                {
                    versionControl = (VersionControlServer) TeamFoundationServer.GetService( typeof( VersionControlServer ) );
                }
                return versionControl;
            }
        }

        public int LatestVersion
        {
            get
            {
                if( latestVersion == 0 )
                {
                    latestVersion = VersionControl.GetLatestChangesetId();
                }
                return latestVersion;
            }
        }
    }
}
