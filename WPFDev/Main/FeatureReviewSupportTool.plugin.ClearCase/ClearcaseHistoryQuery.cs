using System;
using System.Collections.Generic;
using System.Text;
using ClearCase;
using Sep.ConfigurationManagement.Vault.CodeReview;
using System.Text.RegularExpressions;

namespace FeatureReviewSupportTool.ScmSystemHooks.ClearCase
{
    class ClearcaseHistoryQuery
    {
        private const int dateFieldNumber = 0;
        private const int objectNameFieldNumber = 1;
        private const int userFieldNumber = 2;
        private const int commentFieldNumber = 3;
        private const int eventTypeFieldNumber = 4;
        private const string formatString = ">>>%d!!!%n!!!%u!!!%Nc!!!%e<<<\\n";
        // Example:
        // $$$25-Apr-03.13:05:44!!!c:\views\maburke_e2c_view\agt_e2c_gs\Source\T56GS@@\main\1!!!25-Apr-03.13:05:44!!!mbarring!!!Cleaning up for DGS.
        // Added directory element "common".
        // Added directory element "e2c".
        // Added directory element "e2c_dll".
        // Added directory element "DBConvert".
        // Added directory element "report_db".
        // Added directory element "temp".
        // Added directory element "help".
        // Added directory element "install".
        // Added directory element "database".
        // Added file element "AllProjects.sln".
        // Added file element "AllProjects.vcproj".###

        private ConnectionSettings settings;

        /// <summary>
        /// Gets a normalized version of the clearcase view directory, including the trailing "\".
        /// </summary>
        private string ClearcaseViewDirectory
        {
            get
            {
                return settings.ClearcaseViewDirectory + (settings.ClearcaseViewDirectory.EndsWith( @"\" ) ? "" : @"\");
            }
        }

        public ClearcaseHistoryQuery( ConnectionSettings settings )
        {
            this.settings = settings;
        }

        public ChangeHistoryDataSet GetHistory( string query )
        {
            return GetHistory( HistoryQueryHelper.GetTaskTags( query ) );
        }

        public ChangeHistoryDataSet GetHistory( string[] taskTags )
        {
            ChangeHistoryDataSet history = new ChangeHistoryDataSet();
            foreach( Change change in GetChanges() )
            {
                if( HistoryQueryHelper.IsChangeRelatedToTask( change.Comment, taskTags ) )
                {
                    System.Diagnostics.Trace.WriteLine( change.Version.ToString() + " - " + change.ElementId );
                    history.Add(
                        change.ElementId,
                        change.RepositoryPath,
                        change.Version,
                        -1,
                        change.Date,
                        change.User,
                        change.Comment );
                }
            }
            return history;
        }

        private IEnumerable<Change> GetChanges()
        {
            string rawHistory = ClientHelper.CmdExec( GetHistoryCommand() );
            Regex historyMatcher = new Regex( ">>>(.*?)<<<", RegexOptions.Singleline );
            MatchCollection matches = historyMatcher.Matches( rawHistory );
            List<Change> changes = new List<Change>();
            foreach( Match match in matches )
            {
                string changeLine = string.Empty;
                try
                {
                    changeLine = match.Groups[1].Value;
                    System.Diagnostics.Trace.WriteLine( "Parsing " + changeLine );
                    changes.Add( Parse( changeLine ) );
                }
                catch( Exception e )
                {
                    System.Diagnostics.Trace.WriteLine( e );
                }
            }
            return changes;
        }

        private string GetHistoryCommand()
        {
            string sinceClause = (settings.IsDateFilterEnabled ? string.Format( "-since {0:dd-MMM-yyyy}", settings.Since ) : "");
            string command = string.Format( "lshistory -nco -rec {2} -fmt '{0}' {1}", formatString, settings.ClearcaseViewDirectory, sinceClause );
            return command;
        }

        private Change Parse( string changeDescription )
        {
            IList<string> parts = ParseHelper.Split( "!!!", changeDescription );
            Change change = new Change();
            change.ElementId = ParseHelper.GetPath( parts[objectNameFieldNumber] );
            change.RepositoryPath = change.ElementId.Substring( ClearcaseViewDirectory.Length );
            change.Version = Convert.ToInt32( ParseHelper.GetVersionNumber( parts[objectNameFieldNumber] ) );
            change.Date = Convert.ToDateTime( parts[dateFieldNumber] );
            change.User = parts[userFieldNumber];
            change.Comment = string.Format( "({0}) {1}", parts[eventTypeFieldNumber], parts[commentFieldNumber] );
            return change;
        }

        private class Change
        {
            public string ElementId;
            public string RepositoryPath;
            public int Version;
            public DateTime Date;
            public string User;
            public string Comment;
        }
    }
}
