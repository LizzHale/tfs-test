using System;
using System.Diagnostics;
using ClearCase;

namespace FeatureReviewSupportTool.ScmSystemHooks.ClearCase
{
    public static class ClientHelper
    {
        public static string CmdExec( string format, params object[] formatParameters )
        {
            return CmdExec( string.Format( format, formatParameters ) );
        }

        public static string CmdExec( string command )
        {
            IClearTool cleartool = new ClearToolClass();
            Trace.WriteLine( "cleartool " + command );
            return cleartool.CmdExec( command );
        }
    }
}
