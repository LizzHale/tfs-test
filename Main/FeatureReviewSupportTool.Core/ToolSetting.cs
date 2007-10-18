using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace FeatureReviewSupportTool.Core
{
    [Serializable( )]
    public class ToolSetting
    {
        private string fileExtension;
        private string toolPath;
        private string toolCommandLine;

        public ToolSetting( )
        {
            fileExtension = String.Empty;
            toolPath = String.Empty;
            toolCommandLine = String.Empty;
        }

        public override string ToString( )
        {
            if( fileExtension == String.Empty )
            {
                return "(no file extension)";
            }
            else
            {
                return fileExtension;
            }
        }

        [Description( "The file extension to associate with this tool." ),
        DisplayName( "File Extension" )]
        public string FileExtension
        {
            get
            {
                return fileExtension;
            }
            set
            {
                fileExtension = StandardizeFileExtension( value ).ToLower();
            }
        }

        [Description( "The path to the tool." ),
        DisplayName( "Tool Path" ),
        Editor( typeof( FileNameEditor ), typeof( UITypeEditor ) )]
        public string ToolPath
        {
            get
            {
                return toolPath;
            }
            set
            {
                toolPath = value;
            }
        }

        [Description( "The command line to pass to the tool." ),
        DisplayName( "Tool Command Line" )]
        public string ToolCommandLine
        {
            get
            {
                return toolCommandLine;
            }
            set
            {
                toolCommandLine = value;
            }
        }

        // Extensions should be in the form ".extension".
        private string StandardizeFileExtension( string inputExtension )
        {
            if( inputExtension.StartsWith( "." ) )
            {
                return inputExtension;
            }
            else if( inputExtension.StartsWith( "*." ) )
            {
                return inputExtension.Replace( "*.", "." );
            }
            else
            {
                return "." + inputExtension;
            }
        }

    }
}
