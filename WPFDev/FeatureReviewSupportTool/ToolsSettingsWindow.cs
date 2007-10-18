using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeatureReviewSupportTool
{
    public partial class ToolsSettingsWindow : Form
    {
        private IScmSystemHook scmSystem;

        public IScmSystemHook ScmSystem
        {
            get
            {
                return scmSystem;
            }
            set
            {
                scmSystem = value;
                toolsProperties.SelectedObject = scmSystem.ExternalToolsSettings;
            }
        }

        public ComboBox.ObjectCollection AvailableScmSystems
        {
            set
            {
                scmSystemSelector.Items.Clear( );
                foreach( object o in value )
                {
                    scmSystemSelector.Items.Add( o );
                }
            }
        }

        public ToolsSettingsWindow( )
        {
            InitializeComponent( );
        }

        private void ToolsSettingsWindow_Load( object sender, EventArgs e )
        {
            if( ScmSystem != null && scmSystemSelector.Items.Contains( ScmSystem ) )
            {
                scmSystemSelector.SelectedItem = ScmSystem;
            }
        }

        private void scmSystemSelector_SelectedValueChanged( object sender, EventArgs e )
        {
            ScmSystem = (IScmSystemHook) scmSystemSelector.SelectedItem;
        }

        private void ok_Click( object sender, EventArgs e )
        {           
            Close( );
        }

        private void ToolsSettingsWindow_FormClosed( object sender, FormClosedEventArgs e )
        {
            ScmSystem.SaveExternalToolsSettings( );
        }
    }
}