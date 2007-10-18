using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace FeatureReviewSupportTool.Core
{
    public class ExternalToolsHelper
    {
        public static Process GetViewerProgram( string downloadedFile, string fallbackProgram, string fallbackProgramArguments, ExternalToolsSettings settings )
        {
            return GetProgram(
                new string[ ] { Constants.FileMetaString },
                new string[ ] { downloadedFile },
                settings.DefaultViewerTool,
                settings.DefaultViewerToolCommandLine,
                fallbackProgram,
                fallbackProgramArguments,
                settings.ViewerTools );
        }

        public static Process GetDiffProgram( string downloadedFile1, string downloadedFile2, string fallbackProgram, string fallbackProgramArguments, ExternalToolsSettings settings )
        {
            return GetProgram(
                new string[ ] { Constants.FirstFileMetaString, Constants.SecondFileMetaString },
                new string[ ] { downloadedFile1, downloadedFile2 },
                settings.DefaultDiffTool,
                settings.DefaultDiffToolCommandLine,
                fallbackProgram,
                fallbackProgramArguments,
                settings.DiffTools );
        }

        public static string Quote( string str )
        {
            return String.Format( "\"{0}\"", str );
        }
        
        private static Process GetProgram( string[ ] metaStrings, string[ ] files, string defaultToolPath, string defaultToolCommandLine, string fallbackProgram, string fallbackProgramArguments, List<ToolSetting> overridesList )
        {
            if( metaStrings.Length != files.Length )
            {
                throw new ArgumentException( "metaStrings and files must be the same size" );
            }

            Process viewer = new Process( );
            string fileExtension = GetFileExtension( files[ 0 ] );
            
            string[] quotedFiles = Array.ConvertAll<string, string>( files, delegate( string s ) { return Quote( s ); } );

            ToolSetting tool = overridesList.Find( delegate( ToolSetting t ) { return t.FileExtension == fileExtension; } );

            if( tool != null && File.Exists( tool.ToolPath ) )
            {
                viewer.StartInfo.FileName = tool.ToolPath;
                viewer.StartInfo.Arguments = MultipleReplace( tool.ToolCommandLine, metaStrings, quotedFiles );
            }
            else if( File.Exists( defaultToolPath ) )
            {
                viewer.StartInfo.FileName = defaultToolPath;
                viewer.StartInfo.Arguments = MultipleReplace( defaultToolCommandLine, metaStrings, quotedFiles );
            }
            else
            {
                viewer.StartInfo.FileName = fallbackProgram;
                viewer.StartInfo.Arguments = fallbackProgramArguments;
            }

            return viewer;
        }

        private static string MultipleReplace( string sourceString, string[ ] searchTerms, string[ ] replacements )
        {
            if( searchTerms.Length != replacements.Length )
            {
                throw new ArgumentException( "searchTerms and replacements must be of the same length." );
            }

            string modifiedString = sourceString;

            for( int i = 0; i < searchTerms.Length; i++ )
            {
                string term = searchTerms[ i ];
                string replacement = replacements[ i ];
                modifiedString = modifiedString.Replace( term, replacement );
            }

            return modifiedString;
        }

        private static string GetFileExtension( string filePath )
        {
            return new FileInfo( filePath ).Extension.ToLower( );
        }
    }
}
