namespace FeatureReviewSupportTool
{
    partial class VaultConnectionSettings
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
            this.password = new System.Windows.Forms.TextBox();
            this.userName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.repository = new System.Windows.Forms.TextBox();
            this.host = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point( 267, 24 );
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size( 100, 20 );
            this.password.TabIndex = 16;
            // 
            // userName
            // 
            this.userName.Location = new System.Drawing.Point( 267, 0 );
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size( 100, 20 );
            this.userName.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 203, 24 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 56, 16 );
            this.label3.TabIndex = 15;
            this.label3.Text = "&Password:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 203, 0 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 56, 16 );
            this.label2.TabIndex = 11;
            this.label2.Text = "&User:";
            // 
            // repository
            // 
            this.repository.Location = new System.Drawing.Point( 83, 24 );
            this.repository.Name = "repository";
            this.repository.Size = new System.Drawing.Size( 100, 20 );
            this.repository.TabIndex = 14;
            // 
            // host
            // 
            this.host.Location = new System.Drawing.Point( 83, 0 );
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size( 100, 20 );
            this.host.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 3, 24 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 64, 16 );
            this.label4.TabIndex = 13;
            this.label4.Text = "&Repository:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point( 3, 0 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 56, 16 );
            this.label5.TabIndex = 9;
            this.label5.Text = "&Host:";
            // 
            // VaultConnectionSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.password );
            this.Controls.Add( this.userName );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.repository );
            this.Controls.Add( this.host );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.label5 );
            this.Name = "VaultConnectionSettings";
            this.Size = new System.Drawing.Size( 384, 49 );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.TextBox repository;
        private System.Windows.Forms.TextBox host;
    }
}
