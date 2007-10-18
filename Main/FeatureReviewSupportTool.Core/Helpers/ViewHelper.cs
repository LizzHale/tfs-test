using System;
using System.Diagnostics;
using System.IO;

using FeatureReviewSupportTool.Core;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    public static class ViewHelper
    {
        /// <summary>
        /// Opens the file, and waits for the viewing process to exit.
        /// </summary>
        /// <param name="path">The path of the file to open.</param>
        public static void View( string path, ExternalToolsSettings externalToolsSettings )
        {
            Process viewer = ExternalToolsHelper.GetViewerProgram(
                path,
                FileViewer,
                ExternalToolsHelper.Quote(path),
                externalToolsSettings);
                                
            viewer.Start();

            viewer.WaitForExit();
        }

        private static string FileViewer
        {
            get
            {
                string viewer = @"C:\Vim\vim62\gvim.exe";

                // if there are other optional viewers to try before notepad, insert them here:
                // (ideally this would be a user prefs thing.
                //if( ! File.Exists( viewer ) )
                //    viewer = @"c:\path\to\file\editor.exe";

                if( !File.Exists( viewer ) )
                    viewer = "notepad";

                return viewer;
            }
        }
    }
}
