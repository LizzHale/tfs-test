using System;
using System.Windows.Forms;

using FeatureReviewSupportTool.ScmSystemHooks.ClearCase;

namespace FeatureReviewSupportTool
{
    public partial class ClearCaseConnectionSettings : UserControl
    {
        public ClearCaseConnectionSettings()
        {
            InitializeComponent();
        }

        internal void Bind( ConnectionSettings settings )
        {
            connectionSettingsBindingSource.DataSource = settings;
        }

        private void browseForViewDir_Click( object sender, EventArgs e )
        {
            folderBrowserDialog.SelectedPath = viewDirectory.Text;
            if( DialogResult.OK == folderBrowserDialog.ShowDialog( this ) )
            {
                ((ConnectionSettings) connectionSettingsBindingSource.DataSource).ClearcaseViewDirectory = folderBrowserDialog.SelectedPath;
                connectionSettingsBindingSource.ResetCurrentItem();
            }
        }

        private void sinceEnabled_CheckedChanged( object sender, EventArgs e )
        {
            sinceLabel.Enabled = sinceEnabled.Checked;
            since.Enabled = sinceEnabled.Checked;
        }
    }
}
