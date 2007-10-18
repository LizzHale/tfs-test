using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace Sep.Windows.Forms
{
    public class WaitCursor : System.IDisposable
    {
        public WaitCursor( Control control )
        {
            clientControl = control;

            originalCursor = clientControl.Cursor;
            clientControl.Cursor = Cursors.WaitCursor;
        }

        public void Dispose()
        {
            clientControl.Cursor = originalCursor;
        }

        private Cursor originalCursor;
        private Control clientControl;
    }
      
}