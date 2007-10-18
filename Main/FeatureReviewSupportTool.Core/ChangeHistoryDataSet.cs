using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public class ChangeHistoryDataSet : DataSet
    {
        public const string UniqueId        = "UniqueId";
        public const string RepositoryPath  = "RepositoryPath";
        public const string DisplayedPath   = "DisplayedRepositoryPath";
        public const string FirstVersion    = "FirstVersion";
        public const string LastVersion     = "LastVersion";
        public const string CurrentVersion  = "CurrentVersion";
        public const string Comments        = "Comments";
        public const string VersionNumbers  = "VersionNumbers";
        public const string Marked          = "Marked";

        private CommentsManager comments = new CommentsManager( );

        public ChangeHistoryDataSet( )
        {
            Table.Columns.Add( UniqueId );
            Table.Columns.Add( RepositoryPath ).Caption = "Full Path";
            Table.Columns.Add( DisplayedPath ).Caption = "Path";
            DisplayedPathExpression = RepositoryPath;
            Table.Columns.Add( FirstVersion, typeof( long ) ).Caption = "Initial";
            Table.Columns.Add( LastVersion, typeof( long ) ).Caption = "Final";
            Table.Columns.Add( CurrentVersion, typeof( long ) ).Caption = "Current";
            Table.Columns.Add( Comments ).Caption = "Comments";
            Table.Columns.Add( VersionNumbers ).Caption = "Version Numbers";
            AddMarkedColumn( Table );
            Table.PrimaryKey = new DataColumn [] { Table.Columns[ UniqueId ] };
        }

        public static void EnsureMarkedColumnExists( DataSet dataSet )
        {
            if( dataSet != null )
            {
                EnsureMarkedColumnExists( dataSet.Tables[0] );
            }
        }

        public static void EnsureMarkedColumnExists( DataTable table )
        {
            if( table != null && ! table.Columns.Contains( Marked ) )
            {
                AddMarkedColumn( table );
            }
        }

        private static void AddMarkedColumn( DataTable table )
        {
            DataColumn markedColumn = table.Columns.Add( Marked, typeof( bool ) );
            markedColumn.Caption = "Marked";
            markedColumn.DefaultValue = false;
        }

        public DataTable Table
        {
            get
            {
                if( Tables.Count == 0 )
                    Tables.Add( );

                return Tables[0];
            }
        }

        private string DisplayedPathExpression
        {
            get
            {
                return Table.Columns[ DisplayedPath ].Expression;
            }
            set
            {
                Table.Columns[ DisplayedPath ].Expression = (value == null || value == string.Empty) ? RepositoryPath : value;
            }
        }

        public void Add( object uniqueId, string repositoryPath, long changeVersion, long currentVersion, DateTime changeDate, string user, string comment )
        {
            DataRow itemRow = Table.Rows.Find( uniqueId );

            // Add if needed
            if( itemRow == null )
            {
                itemRow = Table.NewRow();
                itemRow[UniqueId] = uniqueId;
                itemRow[RepositoryPath] = repositoryPath;
                itemRow[FirstVersion] = changeVersion - 1;
                itemRow[LastVersion] = changeVersion;
                itemRow[CurrentVersion] = currentVersion;
                Table.Rows.Add( itemRow );
            }
            else
            {
                if( IsNewer( changeVersion, itemRow[LastVersion] ) )
                {
                    itemRow[LastVersion] = changeVersion;
                    itemRow[RepositoryPath] = repositoryPath;
                }
                else if( IsNewer( itemRow[FirstVersion], changeVersion ) )
                {
                    itemRow[FirstVersion] = changeVersion - 1;
                }
            }

            comments.Add( itemRow, changeVersion, changeDate, user, comment );
        }

        private bool IsNewer( object version, object compareTo )
        {
            return IsNewer( (long) version, (long) compareTo );
        }

        private bool IsNewer( long version, long compareTo )
        {
            return version > compareTo;
        }

        private class CommentsManager
        {
            private const string ChangeDate = "date";
            private const string ChangeUser = "user";
            private const string ChangeComment = "comment";

            private IDictionary<DataRow, SortedList<long, IDictionary>> rowsToCommentsCollection = new Dictionary<DataRow, SortedList<long, IDictionary>>();

            private SortedList<long, IDictionary> this[DataRow row]
            {
                get
                {
                    if( ! rowsToCommentsCollection.ContainsKey( row ) )
                    {
                        rowsToCommentsCollection.Add( row, new SortedList<long, IDictionary>() );
                    }
                    return rowsToCommentsCollection[row];
                }
            }

            public void Add( DataRow itemRow, long changeVersion, DateTime changeDate, string user, string comment )
            {
                Hashtable changeInfo = new Hashtable();
                changeInfo[ ChangeDate ] = changeDate;
                changeInfo[ ChangeUser ] = user;
                changeInfo[ ChangeComment ] = comment;
                this[itemRow][changeVersion] = changeInfo;
                FormatChangeComments( itemRow );
            }

            private void FormatChangeComments( DataRow row )
            {
                string comments = string.Empty;
                ArrayList versionNumbers = new ArrayList( );
                foreach( long version in this[row].Keys )
                {
                    comments += FormatChangeComment( version, this[row][version] );
                    versionNumbers.Add( version.ToString() );
                }
                row[ Comments ] = comments;
                row[ VersionNumbers ] = string.Join( ",", (string []) versionNumbers.ToArray( typeof( string ) ) );
            }

            private string FormatChangeComment( long version, IDictionary changeInfo )
            {
                return string.Format( "{0} - {1} - {2} - {3}", changeInfo[ChangeDate], version, changeInfo[ChangeUser], changeInfo[ChangeComment] ) + Environment.NewLine;
            }
        }
    }
}
