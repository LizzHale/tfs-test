using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

using WorkItemIdList = System.Collections.Generic.List<int>;
using System.Text.RegularExpressions;

namespace FeatureReviewSupportTool.TeamFoundation
{
    public class ParsedQuery
    {
        public static ParsedQuery Parse( string query )
        {
            if( query.StartsWith( "$" ) )
            {
                string[] queryParts = query.Split( ';' );
                Match versionParts = Regex.Match( queryParts[1], @"(?<startVersion>\w+)(?:\s*[-~,]?\s*(?<endVersion>\w+))?" );
                return new ParsedQuery( queryParts[0].Trim(),
                    ParseVersion( versionParts, "startVersion" ),
                    ParseVersion( versionParts, "endVersion" ) );
            }
            else
            {
                return new ParsedQuery( ParseWorkItemIds( query ) );
            }
        }

        private static VersionSpec ParseVersion( Match versionParts, string captureName )
        {
            string versionSpec = versionParts.Groups[captureName].Value;
            if( String.IsNullOrEmpty( versionSpec ) )
            {
                return VersionSpec.Latest;
            }
            else
            {
                return VersionSpec.ParseSingleSpec( versionSpec, null );
            }
        }

        private static WorkItemIdList ParseWorkItemIds( string workItemIds )
        {
            return new List<string>( workItemIds.Split( ',' ) ).ConvertAll<int>( delegate( string s ) { return Convert.ToInt32( s.Trim() ); } );
        }

        private ParsedQuery( WorkItemIdList workItemIds )
        {
            HasVersions = false;
            WorkItemIds = workItemIds;
        }

        private ParsedQuery( string serverPath, VersionSpec startVersion, VersionSpec endVersion )
        {
            HasVersions = true;
            WorkItemIds = new WorkItemIdList();

            ServerPath = serverPath;
            StartVersion = startVersion;
            EndVersion = endVersion;
        }

        public readonly WorkItemIdList WorkItemIds;

        public readonly bool HasVersions;
        public readonly string ServerPath;
        public readonly VersionSpec StartVersion;
        public readonly VersionSpec EndVersion;
    }
}
