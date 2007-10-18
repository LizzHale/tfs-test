using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public static class HistoryQueryHelper
    {
        public static bool IsChangeRelatedToTask( string changeComment, params string[] taskTags )
        {
            List<string> taskIds = new List<string>();
            MatchCollection matches = Regex.Matches( changeComment, "\\[([^]]+)\\]" );
            foreach( Match match in matches )
            {
                if( HasIntersection( taskTags, GetTaskTags( match ) ) )
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasIntersection( string[] setA, string[] setB )
        {
            foreach( string a in setA )
            {
                foreach( string b in setB )
                {
                    if( 0 == string.Compare( a, b, true ) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string[] GetTaskTags( Match commentMatch )
        {
            return GetTaskTags( commentMatch.Groups[1].Value );
        }

        public static string[] GetTaskTags( string taskTagList )
        {
            return MapTrim( taskTagList.Split( ',' ) );
        }

        private static string[] MapTrim( string[] s )
        {
            string[] rv = new string[s.Length];
            for( int i = 0; i < s.Length; i++ )
            {
                rv[i] = s[i].Trim();
            }
            return rv;
        }
    }
}
