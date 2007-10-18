using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace FeatureReviewSupportTool.Core
{
    [Serializable( )]
    public class ExternalToolsSettings
    {
        private string defaultViewerTool;
        private string defaultViewerToolCommandLine;
        private string defaultDiffTool;
        private string defaultDiffToolCommandLine;

        private List<ToolSetting> viewerTools;
        private List<ToolSetting> diffTools;

        public ExternalToolsSettings( )
        {
            defaultViewerToolCommandLine = "";
            defaultViewerTool = "";
            defaultDiffToolCommandLine = "";
            defaultDiffTool = "";

            viewerTools = new List<ToolSetting>( );
            diffTools = new List<ToolSetting>( );
        }

        [Category( "View" ),
        Description( "The default viewing tool.  If this is left blank, the default application associated with the file being viewed will be used." ),
        DisplayName( "Default Viewer Tool" ),
        Editor( typeof( FileNameEditor ), typeof( UITypeEditor ) )]
        public string DefaultViewerTool
        {
            get
            {
                return defaultViewerTool;
            }
            set
            {
                defaultViewerTool = value;
            }
        }

        [Category( "View" ),
        Description( "The command line for the default viewing tool.  Use " + Constants.FileMetaString + " to mark the position for the file being viewed." ),
        DisplayName( "Default Viewer Tool Command Line" )]
        public string DefaultViewerToolCommandLine
        {
            get
            {
                return defaultViewerToolCommandLine;
            }
            set
            {
                defaultViewerToolCommandLine = value;
            }
        }

        [Category( "View" ),
        Description( "Defines overrides for the default viewing tool on a per-file extension basis." ),
        DisplayName( "Viewer Tool Overrides" )]
        public List<ToolSetting> ViewerTools
        {
            get
            {
                return viewerTools;
            }
        }

        [Category( "Diff" ),
        Description( "The default diff tool.  If this is left blank, the source control system's default diff tool will be used." ),
        DisplayName( "Default Diff Tool" ),
        Editor( typeof( FileNameEditor ), typeof( UITypeEditor ) )]
        public string DefaultDiffTool
        {
            get
            {
                return defaultDiffTool;
            }
            set
            {
                defaultDiffTool = value;
            }
        }

        [Category( "Diff" ),
        Description( "The command line for the default diff tool. Use " + Constants.FirstFileMetaString + " and " + Constants.SecondFileMetaString + " to mark the positions of the files being compared." ),
        DisplayName( "Default Diff Tool Command Line" )]
        public string DefaultDiffToolCommandLine
        {
            get
            {
                return defaultDiffToolCommandLine;
            }
            set
            {
                defaultDiffToolCommandLine = value;
            }
        }

        [Category( "Diff" ),
        Description( "Defines overrides for the default diff tool on a per-file extension basis." ),
        DisplayName( "Diff Tool Overrides" )]
        public List<ToolSetting> DiffTools
        {
            get
            {
                return diffTools;
            }
        }
    }
}
