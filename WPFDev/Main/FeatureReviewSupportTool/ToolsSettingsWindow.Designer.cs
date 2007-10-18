namespace FeatureReviewSupportTool
{
    partial class ToolsSettingsWindow
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
            if( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.toolsProperties = new System.Windows.Forms.PropertyGrid( );
            this.ok = new System.Windows.Forms.Button( );
            this.label1 = new System.Windows.Forms.Label( );
            this.scmSystemSelector = new System.Windows.Forms.ComboBox( );
            this.SuspendLayout( );
            // 
            // toolsProperties
            // 
            this.toolsProperties.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.toolsProperties.Location = new System.Drawing.Point( 12, 44 );
            this.toolsProperties.Name = "toolsProperties";
            this.toolsProperties.Size = new System.Drawing.Size( 411, 261 );
            this.toolsProperties.TabIndex = 0;
            // 
            // ok
            // 
            this.ok.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.ok.Location = new System.Drawing.Point( 327, 323 );
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size( 96, 32 );
            this.ok.TabIndex = 18;
            this.ok.Text = "&OK";
            this.ok.Click += new System.EventHandler( this.ok_Click );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 12, 9 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 70, 13 );
            this.label1.TabIndex = 20;
            this.label1.Text = "SCM System:";
            // 
            // scmSystemSelector
            // 
            this.scmSystemSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scmSystemSelector.FormattingEnabled = true;
            this.scmSystemSelector.Location = new System.Drawing.Point( 88, 6 );
            this.scmSystemSelector.Name = "scmSystemSelector";
            this.scmSystemSelector.Size = new System.Drawing.Size( 178, 21 );
            this.scmSystemSelector.TabIndex = 23;
            this.scmSystemSelector.SelectedValueChanged += new System.EventHandler( this.scmSystemSelector_SelectedValueChanged );
            // 
            // ToolsSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 435, 367 );
            this.Controls.Add( this.scmSystemSelector );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.ok );
            this.Controls.Add( this.toolsProperties );
            this.Name = "ToolsSettingsWindow";
            this.Text = "Configure External Tools";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.ToolsSettingsWindow_FormClosed );
            this.Load += new System.EventHandler( this.ToolsSettingsWindow_Load );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.PropertyGrid toolsProperties;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox scmSystemSelector;
    }
}