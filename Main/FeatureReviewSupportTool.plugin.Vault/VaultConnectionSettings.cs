using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportTool
{
    public partial class VaultConnectionSettings : UserControl
    {
        public VaultConnectionSettings()
        {
            InitializeComponent();
        }

        internal void Bind( VaultRepositoryAuthSettings connectionSettings )
        {
            host.DataBindings.Add( "Text", connectionSettings, "Host" );
            userName.DataBindings.Add( "Text", connectionSettings, "User" );
            password.DataBindings.Add( "Text", connectionSettings, "Password" );
            repository.DataBindings.Add( "Text", connectionSettings, "Repository" );
        }
    }
}
