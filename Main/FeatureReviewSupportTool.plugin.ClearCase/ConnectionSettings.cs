using System;
using System.Collections.Generic;
using System.Text;
using FeatureReviewSupportTool.plugin.ClearCase.Properties;

namespace FeatureReviewSupportTool.ScmSystemHooks.ClearCase
{
    class ConnectionSettings
    {
        private Settings BaseSettings
        {
            get { return Settings.Default; }
        }

        public string ClearcaseViewDirectory
        {
            get { return BaseSettings.ClearcaseViewDirectory; }
            set { BaseSettings.ClearcaseViewDirectory = value; }
        }

        public DateTime Since
        {
            get { return DateTime.Now.Subtract( BaseSettings.Since ); }
            set { BaseSettings.Since = DateTime.Now.Subtract( value ); }
        }

        public bool IsDateFilterEnabled
        {
            get { return BaseSettings.IsDateFilterEnabled; }
            set { BaseSettings.IsDateFilterEnabled = value; }
        }

        internal void Save()
        {
            BaseSettings.Save();
        }
    }
}
