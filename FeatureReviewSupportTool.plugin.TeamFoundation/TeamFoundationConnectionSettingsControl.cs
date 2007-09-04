using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Proxy;

namespace FeatureReviewSupportTool
{
    public partial class TeamFoundationConnectionSettingsControl : UserControl
    {
        private TeamFoundationServer server;

        public TeamFoundationConnectionSettingsControl()
        {
            InitializeComponent();
            tfServer.DataBindings.Add( "Text", this, "TeamServerName" );
        }

        public event EventHandler TeamServerNameChanged;

        public string TeamServerName
        {
            get
            {
                return server == null ? string.Empty : server.Name;
            }
            set
            {
                server = value == null ? null : new TeamFoundationServer( value );
                Send( TeamServerNameChanged );
            }
        }

        public TeamFoundationServer TeamServer
        {
            get
            {
                return server;
            }
        }

        public bool FollowReviewLinks
        {
            get { return followReviewLinks.Checked; }
            set { followReviewLinks.Checked = value; }
        }

        public bool OpenCril
        {
            get { return openCril.Checked; }
            set { openCril.Checked = value; }
        }

        public bool OpenWorkItem
        {
            get { return openWorkItem.Checked; }
            set { openWorkItem.Checked = value; }
        }

        private void pickServerButton_Click( object sender, EventArgs e )
        {
            DomainProjectPicker dlg = new DomainProjectPicker( DomainProjectPickerMode.None );
            if( dlg.ShowDialog() == DialogResult.OK )
            {
                server = dlg.SelectedServer;
                Send( TeamServerNameChanged );
            }
        }

        private void Send( EventHandler eventHandler )
        {
            if( eventHandler != null )
            {
                eventHandler( this, EventArgs.Empty );
            }
        }

        private void openWorkItem_CheckedChanged( object sender, EventArgs e )
        {
            warningProvider.SetError( openWorkItem, GetOpenWorkItemMessage() );
        }

        private string GetOpenWorkItemMessage()
        {
            if( !OpenWorkItem )
                return "";
            return "If you receive the error \"You are not logged in or session is expired.\" open http://teamplain/ in the browser window that shows the error, and then use the back button to return to the work item.";
        }
    }
}
