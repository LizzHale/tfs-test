using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FeatureReviewSupportTool
{
    public partial class TraceWindow : Form
    {
        public TraceWindow()
        {
            InitializeComponent();
        }

        private TraceListener myTraceListener;
        public TraceListener MyTraceListener
        {
            get
            {
                if( myTraceListener == null )
                {
                    myTraceListener = new TextBoxTraceListener( traceOutput );
                }
                return myTraceListener;
            }
        }
	

        private void TraceWindow_Load( object sender, EventArgs e )
        {
            Trace.Listeners.Add( MyTraceListener );
        }

        private void TraceWindow_FormClosed( object sender, FormClosedEventArgs e )
        {
            Trace.Listeners.Remove( MyTraceListener );
        }
    }
}