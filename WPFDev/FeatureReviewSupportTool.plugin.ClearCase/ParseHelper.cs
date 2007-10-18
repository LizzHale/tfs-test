using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureReviewSupportTool.ScmSystemHooks.ClearCase
{
    static class ParseHelper
    {
        public static IList<string> Split( string separator, string s )
        {
            return s.Split( new string[] { separator }, StringSplitOptions.None );
        }

        // Example elementVersionName: "c:\my_view\my_vob\my_element@@\main\1"
        // Returns: "c:\my_view\my_vob\my_element"
        public static string GetPath( string elementVersionName )
        {
            IList<string> objectNameParts = SplitElementVersionName( elementVersionName );
            return objectNameParts[0];
        }

        // Example elementVersionName: "c:\my_view\my_vob\my_element@@\main\1"
        // Returns: 1
        public static int GetVersionNumber( string elementVersionName )
        {
            IList<string> objectNameParts = SplitElementVersionName( elementVersionName );
            IList<string> versionParts = Split( @"\", objectNameParts[1] );
            return Convert.ToInt32( versionParts[versionParts.Count - 1] );
        }

        private static IList<string> SplitElementVersionName( string elementVersionName )
        {
            return Split( "@@", elementVersionName );
        }
    }
}
