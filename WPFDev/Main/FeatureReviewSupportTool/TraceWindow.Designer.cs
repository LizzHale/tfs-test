namespace FeatureReviewSupportTool
{
    partial class TraceWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.traceOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // traceOutput
            // 
            this.traceOutput.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceOutput.Location = new System.Drawing.Point( 12, 12 );
            this.traceOutput.Multiline = true;
            this.traceOutput.Name = "traceOutput";
            this.traceOutput.ReadOnly = true;
            this.traceOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.traceOutput.Size = new System.Drawing.Size( 420, 334 );
            this.traceOutput.TabIndex = 0;
            // 
            // TraceWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 444, 358 );
            this.Controls.Add( this.traceOutput );
            this.Name = "TraceWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SEP Feature Review Support Tool - Trace";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.TraceWindow_FormClosed );
            this.Load += new System.EventHandler( this.TraceWindow_Load );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox traceOutput;
    }
}