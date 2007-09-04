namespace FeatureReviewSupportTool
{
    partial class ClearCaseConnectionSettings
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
            this.viewDirLabel = new System.Windows.Forms.Label();
            this.viewDirectory = new System.Windows.Forms.TextBox();
            this.browseForViewDir = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
            this.sinceEnabled = new System.Windows.Forms.CheckBox();
            this.sinceLabel = new System.Windows.Forms.Label();
            this.since = new System.Windows.Forms.DateTimePicker();
            this.connectionSettingsBindingSource = new System.Windows.Forms.BindingSource( this.components );
            ((System.ComponentModel.ISupportInitialize) (this.connectionSettingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // viewDirLabel
            // 
            this.viewDirLabel.AutoSize = true;
            this.viewDirLabel.Location = new System.Drawing.Point( 3, 6 );
            this.viewDirLabel.Name = "viewDirLabel";
            this.viewDirLabel.Size = new System.Drawing.Size( 78, 13 );
            this.viewDirLabel.TabIndex = 0;
            this.viewDirLabel.Text = "View Directory:";
            // 
            // viewDirectory
            // 
            this.viewDirectory.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewDirectory.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.connectionSettingsBindingSource, "ClearcaseViewDirectory", true ) );
            this.viewDirectory.Location = new System.Drawing.Point( 87, 3 );
            this.viewDirectory.Name = "viewDirectory";
            this.viewDirectory.Size = new System.Drawing.Size( 316, 20 );
            this.viewDirectory.TabIndex = 1;
            this.toolTip1.SetToolTip( this.viewDirectory, "Root Directory to search. This must be an element (directory or file) in a VOB in" +
                    " a View." );
            // 
            // browseForViewDir
            // 
            this.browseForViewDir.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseForViewDir.Location = new System.Drawing.Point( 409, 3 );
            this.browseForViewDir.Name = "browseForViewDir";
            this.browseForViewDir.Size = new System.Drawing.Size( 75, 23 );
            this.browseForViewDir.TabIndex = 2;
            this.browseForViewDir.Text = "Browse...";
            this.browseForViewDir.UseVisualStyleBackColor = true;
            this.browseForViewDir.Click += new System.EventHandler( this.browseForViewDir_Click );
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // sinceEnabled
            // 
            this.sinceEnabled.AutoSize = true;
            this.sinceEnabled.Checked = true;
            this.sinceEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sinceEnabled.DataBindings.Add( new System.Windows.Forms.Binding( "Checked", this.connectionSettingsBindingSource, "IsDateFilterEnabled", true ) );
            this.sinceEnabled.Location = new System.Drawing.Point( 6, 31 );
            this.sinceEnabled.Name = "sinceEnabled";
            this.sinceEnabled.Size = new System.Drawing.Size( 15, 14 );
            this.sinceEnabled.TabIndex = 5;
            this.toolTip1.SetToolTip( this.sinceEnabled, "Use this filter to speed up history queries." );
            this.sinceEnabled.UseVisualStyleBackColor = true;
            this.sinceEnabled.CheckedChanged += new System.EventHandler( this.sinceEnabled_CheckedChanged );
            // 
            // sinceLabel
            // 
            this.sinceLabel.AutoSize = true;
            this.sinceLabel.Location = new System.Drawing.Point( 44, 32 );
            this.sinceLabel.Name = "sinceLabel";
            this.sinceLabel.Size = new System.Drawing.Size( 37, 13 );
            this.sinceLabel.TabIndex = 3;
            this.sinceLabel.Text = "Since:";
            // 
            // since
            // 
            this.since.DataBindings.Add( new System.Windows.Forms.Binding( "Value", this.connectionSettingsBindingSource, "Since", true ) );
            this.since.Location = new System.Drawing.Point( 87, 28 );
            this.since.Name = "since";
            this.since.Size = new System.Drawing.Size( 200, 20 );
            this.since.TabIndex = 4;
            // 
            // connectionSettingsBindingSource
            // 
            this.connectionSettingsBindingSource.DataSource = typeof( FeatureReviewSupportTool.ScmSystemHooks.ClearCase.ConnectionSettings );
            // 
            // ClearCaseConnectionSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.sinceEnabled );
            this.Controls.Add( this.since );
            this.Controls.Add( this.sinceLabel );
            this.Controls.Add( this.browseForViewDir );
            this.Controls.Add( this.viewDirectory );
            this.Controls.Add( this.viewDirLabel );
            this.Name = "ClearCaseConnectionSettings";
            this.Size = new System.Drawing.Size( 492, 65 );
            ((System.ComponentModel.ISupportInitialize) (this.connectionSettingsBindingSource)).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label viewDirLabel;
        private System.Windows.Forms.TextBox viewDirectory;
        private System.Windows.Forms.Button browseForViewDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label sinceLabel;
        private System.Windows.Forms.DateTimePicker since;
        private System.Windows.Forms.CheckBox sinceEnabled;
        private System.Windows.Forms.BindingSource connectionSettingsBindingSource;
    }
}
