namespace FeatureReviewSupportTool
{
    partial class TeamFoundationConnectionSettingsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TeamFoundationConnectionSettingsControl ) );
            this.tfServerLabel = new System.Windows.Forms.Label();
            this.tfServer = new System.Windows.Forms.TextBox();
            this.followReviewLinks = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
            this.openCril = new System.Windows.Forms.CheckBox();
            this.openWorkItem = new System.Windows.Forms.CheckBox();
            this.pickServerButton = new System.Windows.Forms.Button();
            this.warningProvider = new System.Windows.Forms.ErrorProvider( this.components );
            ((System.ComponentModel.ISupportInitialize) (this.warningProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tfServerLabel
            // 
            this.tfServerLabel.AutoSize = true;
            this.tfServerLabel.Location = new System.Drawing.Point( 3, 3 );
            this.tfServerLabel.Name = "tfServerLabel";
            this.tfServerLabel.Size = new System.Drawing.Size( 57, 13 );
            this.tfServerLabel.TabIndex = 0;
            this.tfServerLabel.Text = "TF Serve&r:";
            // 
            // tfServer
            // 
            this.tfServer.Location = new System.Drawing.Point( 66, 3 );
            this.tfServer.Name = "tfServer";
            this.tfServer.ReadOnly = true;
            this.tfServer.Size = new System.Drawing.Size( 121, 20 );
            this.tfServer.TabIndex = 1;
            // 
            // followReviewLinks
            // 
            this.followReviewLinks.AutoSize = true;
            this.followReviewLinks.Checked = true;
            this.followReviewLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.followReviewLinks.Location = new System.Drawing.Point( 66, 30 );
            this.followReviewLinks.Name = "followReviewLinks";
            this.followReviewLinks.Size = new System.Drawing.Size( 160, 17 );
            this.followReviewLinks.TabIndex = 3;
            this.followReviewLinks.Text = "Treat reviews as part of task";
            this.toolTip1.SetToolTip( this.followReviewLinks, "If the given work item number is a review, automatically look for any associated " +
                    "tasks. For each task that is found, include any associated review items." );
            this.followReviewLinks.UseVisualStyleBackColor = true;
            // 
            // openCril
            // 
            this.openCril.AutoSize = true;
            this.openCril.Location = new System.Drawing.Point( 232, 30 );
            this.openCril.Name = "openCril";
            this.openCril.Size = new System.Drawing.Size( 79, 17 );
            this.openCril.TabIndex = 4;
            this.openCril.Text = "Open CRIL";
            this.toolTip1.SetToolTip( this.openCril, "If a review item with a linked CRIL is entered in the box, open that CRIL in exce" +
                    "l" );
            this.openCril.UseVisualStyleBackColor = true;
            // 
            // openWorkItem
            // 
            this.openWorkItem.AutoSize = true;
            this.openWorkItem.Location = new System.Drawing.Point( 317, 30 );
            this.openWorkItem.Name = "openWorkItem";
            this.openWorkItem.Size = new System.Drawing.Size( 104, 17 );
            this.openWorkItem.TabIndex = 5;
            this.openWorkItem.Text = "Open Work Item";
            this.toolTip1.SetToolTip( this.openWorkItem, "Open the work item in a web browser" );
            this.openWorkItem.UseVisualStyleBackColor = true;
            this.openWorkItem.CheckedChanged += new System.EventHandler( this.openWorkItem_CheckedChanged );
            // 
            // pickServerButton
            // 
            this.pickServerButton.Location = new System.Drawing.Point( 205, 3 );
            this.pickServerButton.Name = "pickServerButton";
            this.pickServerButton.Size = new System.Drawing.Size( 75, 23 );
            this.pickServerButton.TabIndex = 2;
            this.pickServerButton.Text = "Choose...";
            this.pickServerButton.UseVisualStyleBackColor = true;
            this.pickServerButton.Click += new System.EventHandler( this.pickServerButton_Click );
            // 
            // warningProvider
            // 
            this.warningProvider.ContainerControl = this;
            this.warningProvider.Icon = ((System.Drawing.Icon) (resources.GetObject( "warningProvider.Icon" )));
            // 
            // TeamFoundationConnectionSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.openWorkItem );
            this.Controls.Add( this.pickServerButton );
            this.Controls.Add( this.openCril );
            this.Controls.Add( this.followReviewLinks );
            this.Controls.Add( this.tfServer );
            this.Controls.Add( this.tfServerLabel );
            this.Name = "TeamFoundationConnectionSettingsControl";
            this.Size = new System.Drawing.Size( 459, 58 );
            ((System.ComponentModel.ISupportInitialize) (this.warningProvider)).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label tfServerLabel;
        private System.Windows.Forms.TextBox tfServer;
        private System.Windows.Forms.CheckBox followReviewLinks;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox openCril;
        private System.Windows.Forms.Button pickServerButton;
        private System.Windows.Forms.CheckBox openWorkItem;
        private System.Windows.Forms.ErrorProvider warningProvider;
    }
}
