using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FeatureReviewSupportTool
{
    class TextBoxTraceListener : TraceListener
    {
        private TextBox textBox;

        public TextBoxTraceListener( TextBox textBox )
        {
            this.textBox = textBox;
        }

        public override void Write( string message )
        {
            if( textBox.InvokeRequired )
            {
                textBox.Invoke( (MethodInvoker) delegate() { DoWrite( message ); } );
            }
            else
            {
                DoWrite( message );
            }
        }

        private void DoWrite( string message )
        {
            textBox.SuspendLayout();

            bool moveToEnd = (textBox.SelectionStart == textBox.Text.Length);
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;
            textBox.Text = textBox.Text + message;
            if( moveToEnd )
            {
                textBox.SelectionStart = textBox.Text.Length;
            }
            else
            {
                textBox.SelectionStart = selectionStart;
                textBox.SelectionLength = selectionLength;
            }
            textBox.ScrollToCaret();

            textBox.ResumeLayout();
        }

        public override void WriteLine( string message )
        {
            Write( message + Environment.NewLine );
        }
    }
}
