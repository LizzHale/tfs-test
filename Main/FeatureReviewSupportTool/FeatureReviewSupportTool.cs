using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Sep.ConfigurationManagement.Vault.CodeReview;
using Sep.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using FeatureReviewSupportTool.Properties;

namespace FeatureReviewSupportTool
{
    public class FeatureReviewForm : System.Windows.Forms.Form
    {
        #region Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            LoadPlugins();
            UpgradeSettingsIfNecessary();
            Application.Run(new FeatureReviewForm());
        }

        private static void UpgradeSettingsIfNecessary()
        {
            if( Settings.Default.ShouldUpgradeSettings )
            {
                UpgradeSettings();
                Settings.Default.ShouldUpgradeSettings = false;
                Settings.Default.Save();
            }
        }

        private static void UpgradeSettings()
        {
            ForeachTypeDo( delegate( Type type )
            {
                PropertyInfo defaultProperty = type.GetProperty( "Default" );
                if( type.IsSubclassOf( typeof( ApplicationSettingsBase ) ) && (defaultProperty != null) )
                {
                    Trace.WriteLine( "Upgrading settings for " + type.FullName );
                    ApplicationSettingsBase settings = (ApplicationSettingsBase) defaultProperty.GetValue( type, new object[0] );
                    settings.Upgrade();
                }
            } );
        }

        private static void LoadPlugins()
        {
            foreach( string file in Directory.GetFiles( GetApplicationDirectory(), "FeatureReviewSupportTool.plugin.*.dll" ) )
            {
                try
                {
                    Trace.WriteLine( "Loading plugin " + file );
                    Assembly.LoadFile( file );
                }
                catch( Exception e )
                {
                    Trace.WriteLine( e );
                }
            }
        }

        private static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName( Application.ExecutablePath );
        }

        #endregion

        #region Type-scanning helpers

        private delegate void TypeFunction( Type t );

        private static void ForeachTypeDo( TypeFunction func )
        {
            foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                ForeachTypeDo( assembly, func );
            }
        }

        private static void ForeachTypeDo( Assembly assembly, TypeFunction func )
        {
            foreach( Type type in assembly.GetTypes() )
            {
                func( type );
            }
        }

        #endregion

        private System.Windows.Forms.TextBox getFeatureHistoryEditBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button getFeatureHistoryButton;
        private System.Windows.Forms.Button showBlame;
        private System.Windows.Forms.Button copyForExcelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button diffButton;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DataGridView versionGrid;
        private System.Windows.Forms.CheckBox comments;
        private System.Windows.Forms.CheckBox showFullRepositoryPath;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuItem diffContextMenuItem;
        private System.Windows.Forms.MenuItem viewContextMenuItem;
        private System.Windows.Forms.ContextMenu gridContextMenu;
        private System.Windows.Forms.Button viewButton;
        private System.Windows.Forms.MenuItem markContextMenuItem;
        private System.Windows.Forms.MenuItem unmarkContextMenuItem;
        private System.Windows.Forms.ProgressBar progressBar1;
        private ComboBox scmSystem;
        private Label label6;
        private Button viewTraceButton;
        private Button makeArchiveButton;
        private Button setToolsButton;
        private SaveFileDialog makeArchiveFileDialog;
        private DataView currentDataSet;

        #region Properties

        private DataRow CurrentRow
        {
            get
            {
                if (versionGrid.SelectedRows.Count > 0 && currentDataSet.Count > versionGrid.SelectedRows[0].Index)
                {
                    return currentDataSet[versionGrid.SelectedRows[0].Index].Row;
                }
                else
                {
                    return null;
                }
            }
        }

        private object SelectedItemId
        {
            get
            {
                return GetSelectedCell( ChangeHistoryDataSet.UniqueId );
            }
        }

        private string SelectedRepositoryPath
        {
            get
            {
                return GetSelectedString( ChangeHistoryDataSet.RepositoryPath );
            }
        }

        private long SelectedItemFirstVersion
        {
            get
            {
                return GetSelectedLong( ChangeHistoryDataSet.FirstVersion );
            }
        }

        private long SelectedItemLastVersion
        {
            get
            {
                return GetSelectedLong( ChangeHistoryDataSet.LastVersion );
            }
        }

        private long SelectedItemCurrentVersion
        {
            get
            {
                return GetSelectedLong( ChangeHistoryDataSet.CurrentVersion );
            }
        }

        private DataSet CurrentDataSet
        {
            get
            {
                return (currentDataSet == null) ? null : currentDataSet.Table.DataSet;
            }
            set
            {
                if( currentDataSet == null && value == null )
                    return;

                ChangeHistoryDataSet.EnsureMarkedColumnExists( value );
                currentDataSet = (value == null) ? null : value.Tables[0].DefaultView;
                UpdateGrid();
            }
        }

        private Rectangle NormalBounds
        {
            get
            {
                return WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
            }
        }

        #endregion

        public FeatureReviewForm()
        {
            InitializeComponent();

            LoadHooks();

            this.Activated += new EventHandler(FeatureReviewForm_Activate);
        }

        private void LoadHooks()
        {
            Type iScmHookType = typeof( IScmSystemHook );
            ForeachTypeDo( delegate( Type type )
            {
                if( iScmHookType.IsAssignableFrom( type ) && !type.IsAbstract )
                {
                    try
                    {
                        scmSystem.Items.Add( Activator.CreateInstance( type ) );
                        Trace.WriteLine( "Added SCM hook \"" + type.Name + "\"" );
                    }
                    catch( Exception e )
                    {
                        Trace.WriteLine( e );
                        MessageBox.Show( "Tried to add SCM hook using " + type.FullName + ", but got an error: " + e.Message );
                    }
                }
            } );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.getFeatureHistoryEditBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.getFeatureHistoryButton = new System.Windows.Forms.Button();
            this.showBlame = new System.Windows.Forms.Button();
            this.copyForExcelButton = new System.Windows.Forms.Button();
            this.diffButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.versionGrid = new System.Windows.Forms.DataGridView();
            this.gridContextMenu = new System.Windows.Forms.ContextMenu();
            this.diffContextMenuItem = new System.Windows.Forms.MenuItem();
            this.viewContextMenuItem = new System.Windows.Forms.MenuItem();
            this.markContextMenuItem = new System.Windows.Forms.MenuItem();
            this.unmarkContextMenuItem = new System.Windows.Forms.MenuItem();
            this.comments = new System.Windows.Forms.CheckBox();
            this.showFullRepositoryPath = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.viewButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.scmSystem = new System.Windows.Forms.ComboBox();
            this.viewTraceButton = new System.Windows.Forms.Button();
            this.makeArchiveButton = new System.Windows.Forms.Button();
            this.setToolsButton = new System.Windows.Forms.Button();
            this.makeArchiveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.versionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // getFeatureHistoryEditBox
            // 
            this.getFeatureHistoryEditBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.getFeatureHistoryEditBox.Location = new System.Drawing.Point(8, 164);
            this.getFeatureHistoryEditBox.Name = "getFeatureHistoryEditBox";
            this.getFeatureHistoryEditBox.Size = new System.Drawing.Size(419, 20);
            this.getFeatureHistoryEditBox.TabIndex = 11;
            this.getFeatureHistoryEditBox.Leave += new System.EventHandler(this.DoNotUseGoForAcceptButton);
            this.getFeatureHistoryEditBox.Enter += new System.EventHandler(this.getFeatureHistoryEditBox_Enter);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "&Feature #\'s (split with commas), or feature branch";
            // 
            // getFeatureHistoryButton
            // 
            this.getFeatureHistoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.getFeatureHistoryButton.Location = new System.Drawing.Point(435, 164);
            this.getFeatureHistoryButton.Name = "getFeatureHistoryButton";
            this.getFeatureHistoryButton.Size = new System.Drawing.Size(40, 24);
            this.getFeatureHistoryButton.TabIndex = 12;
            this.getFeatureHistoryButton.Text = "&Go!";
            this.getFeatureHistoryButton.Click += new System.EventHandler(this.getFeatureHistoryButton_Click);
            // 
            // showBlame
            // 
            this.showBlame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showBlame.Location = new System.Drawing.Point(507, 286);
            this.showBlame.Name = "showBlame";
            this.showBlame.Size = new System.Drawing.Size(96, 32);
            this.showBlame.TabIndex = 17;
            this.showBlame.Text = "&Blame";
            this.showBlame.Click += new System.EventHandler(this.showBlame_Click);
            // 
            // copyForExcelButton
            // 
            this.copyForExcelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyForExcelButton.Location = new System.Drawing.Point(507, 234);
            this.copyForExcelButton.Name = "copyForExcelButton";
            this.copyForExcelButton.Size = new System.Drawing.Size(96, 32);
            this.copyForExcelButton.TabIndex = 16;
            this.copyForExcelButton.Text = "&Copy for Excel";
            this.copyForExcelButton.Click += new System.EventHandler(this.copyForExcelButton_Click);
            // 
            // diffButton
            // 
            this.diffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.diffButton.Location = new System.Drawing.Point(507, 324);
            this.diffButton.Name = "diffButton";
            this.diffButton.Size = new System.Drawing.Size(96, 32);
            this.diffButton.TabIndex = 18;
            this.diffButton.Text = "&Diff";
            this.diffButton.Click += new System.EventHandler(this.diffButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(8, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(595, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "C&onnection Settings";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // versionGrid
            // 
            this.versionGrid.AllowUserToAddRows = false;
            this.versionGrid.AllowUserToDeleteRows = false;
            this.versionGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.versionGrid.ContextMenu = this.gridContextMenu;
            this.versionGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.versionGrid.Location = new System.Drawing.Point(8, 196);
            this.versionGrid.MultiSelect = false;
            this.versionGrid.Name = "versionGrid";
            this.versionGrid.RowHeadersVisible = false;
            this.versionGrid.RowTemplate.ReadOnly = true;
            this.versionGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.versionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.versionGrid.Size = new System.Drawing.Size(485, 356);
            this.versionGrid.TabIndex = 15;
            this.versionGrid.Scroll += new System.Windows.Forms.ScrollEventHandler(this.versionGrid_Scroll);
            this.versionGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.versionGrid_Paint);
            this.versionGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.versionGrid_KeyDown);
            this.versionGrid.Resize += new System.EventHandler(this.versionGrid_Resize);
            // 
            // gridContextMenu
            // 
            this.gridContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.diffContextMenuItem,
            this.viewContextMenuItem,
            this.markContextMenuItem,
            this.unmarkContextMenuItem});
            this.gridContextMenu.Popup += new System.EventHandler(this.gridContextMenu_Popup);
            // 
            // diffContextMenuItem
            // 
            this.diffContextMenuItem.Index = 0;
            this.diffContextMenuItem.Text = "Diff";
            // 
            // viewContextMenuItem
            // 
            this.viewContextMenuItem.Index = 1;
            this.viewContextMenuItem.Text = "View";
            // 
            // markContextMenuItem
            // 
            this.markContextMenuItem.Index = 2;
            this.markContextMenuItem.Text = "Mark";
            this.markContextMenuItem.Click += new System.EventHandler(this.markContextMenuItem_Click);
            // 
            // unmarkContextMenuItem
            // 
            this.unmarkContextMenuItem.Index = 3;
            this.unmarkContextMenuItem.Text = "Unmark";
            this.unmarkContextMenuItem.Visible = false;
            this.unmarkContextMenuItem.Click += new System.EventHandler(this.markContextMenuItem_Click);
            // 
            // comments
            // 
            this.comments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comments.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.comments.Checked = true;
            this.comments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.comments.Location = new System.Drawing.Point(499, 156);
            this.comments.Name = "comments";
            this.comments.Size = new System.Drawing.Size(104, 32);
            this.comments.TabIndex = 13;
            this.comments.Text = "&Show Change Comments";
            this.comments.CheckedChanged += new System.EventHandler(this.SetCommentsRowVisibility);
            // 
            // showFullRepositoryPath
            // 
            this.showFullRepositoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showFullRepositoryPath.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.showFullRepositoryPath.Checked = true;
            this.showFullRepositoryPath.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showFullRepositoryPath.Location = new System.Drawing.Point(499, 196);
            this.showFullRepositoryPath.Name = "showFullRepositoryPath";
            this.showFullRepositoryPath.Size = new System.Drawing.Size(104, 32);
            this.showFullRepositoryPath.TabIndex = 14;
            this.showFullRepositoryPath.Text = "Show Ful&l Repository Path";
            this.showFullRepositoryPath.CheckedChanged += new System.EventHandler(this.showFullRepositoryPath_CheckedChanged);
            // 
            // viewButton
            // 
            this.viewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewButton.Location = new System.Drawing.Point(507, 362);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(96, 32);
            this.viewButton.TabIndex = 19;
            this.viewButton.Text = "&View";
            this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(280, 132);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(328, 16);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 20;
            this.progressBar1.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "SCM S&ystem:";
            // 
            // scmSystem
            // 
            this.scmSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scmSystem.FormattingEnabled = true;
            this.scmSystem.Location = new System.Drawing.Point(86, 12);
            this.scmSystem.Name = "scmSystem";
            this.scmSystem.Size = new System.Drawing.Size(178, 21);
            this.scmSystem.TabIndex = 22;
            this.scmSystem.SelectedIndexChanged += new System.EventHandler(this.scmSystem_SelectedIndexChanged);
            // 
            // viewTraceButton
            // 
            this.viewTraceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.viewTraceButton.Location = new System.Drawing.Point(507, 520);
            this.viewTraceButton.Name = "viewTraceButton";
            this.viewTraceButton.Size = new System.Drawing.Size(96, 32);
            this.viewTraceButton.TabIndex = 23;
            this.viewTraceButton.Text = "View &Trace";
            this.viewTraceButton.UseVisualStyleBackColor = true;
            this.viewTraceButton.Click += new System.EventHandler(this.viewTraceButton_Click);
            // 
            // makeArchiveButton
            // 
            this.makeArchiveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.makeArchiveButton.Location = new System.Drawing.Point(507, 400);
            this.makeArchiveButton.Name = "makeArchiveButton";
            this.makeArchiveButton.Size = new System.Drawing.Size(96, 32);
            this.makeArchiveButton.TabIndex = 24;
            this.makeArchiveButton.Text = "Make &Archive";
            this.makeArchiveButton.UseVisualStyleBackColor = true;
            this.makeArchiveButton.Click += new System.EventHandler(this.makeArchiveButton_Click);
            // 
            // setToolsButton
            // 
            this.setToolsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.setToolsButton.Location = new System.Drawing.Point(507, 482);
            this.setToolsButton.Name = "setToolsButton";
            this.setToolsButton.Size = new System.Drawing.Size(96, 32);
            this.setToolsButton.TabIndex = 25;
            this.setToolsButton.Text = "&Set Tools...";
            this.setToolsButton.UseVisualStyleBackColor = true;
            this.setToolsButton.Click += new System.EventHandler(this.setToolsButton_Click);
            // 
            // makeArchiveFileDialog
            // 
            this.makeArchiveFileDialog.DefaultExt = "zip";
            this.makeArchiveFileDialog.Filter = "Zip files|*.zip|All files|*.*";
            // 
            // FeatureReviewForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(611, 558);
            this.Controls.Add(this.setToolsButton);
            this.Controls.Add(this.makeArchiveButton);
            this.Controls.Add(this.viewTraceButton);
            this.Controls.Add(this.scmSystem);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.viewButton);
            this.Controls.Add(this.showFullRepositoryPath);
            this.Controls.Add(this.comments);
            this.Controls.Add(this.getFeatureHistoryEditBox);
            this.Controls.Add(this.versionGrid);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.diffButton);
            this.Controls.Add(this.copyForExcelButton);
            this.Controls.Add(this.showBlame);
            this.Controls.Add(this.getFeatureHistoryButton);
            this.Controls.Add(this.label1);
            this.Name = "FeatureReviewForm";
            this.Text = "SEP Feature Review Support Tool";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FeatureReviewForm_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.versionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private IScmSystemHook CurrentScmSystem
        {
            get
            {
                return (IScmSystemHook) scmSystem.SelectedItem;
            }
        }

        private void getFeatureHistoryButton_Click(object sender, System.EventArgs e)
        {
            errorProvider1.SetError( getFeatureHistoryButton, string.Empty );

            using( new WaitCursor( this ) )
            {
                progressBar1.Visible = true;
                try
                {
                    CurrentDataSet = CurrentScmSystem.GetVersions( getFeatureHistoryEditBox.Text );
                }
                catch( Exception ex )
                {
                    Debug.WriteLine( ex );
                    errorProvider1.SetError( getFeatureHistoryButton, ex.Message );
                }
                finally
                {
                    progressBar1.Visible = false;
                }
            }
        }

        private void UpdateGrid()
        {
            currentDataSet.Sort = ChangeHistoryDataSet.RepositoryPath;

            versionGrid.DataSource = currentDataSet;
            ////versionGrid.RetrieveStructure();
            ////foreach( DataColumn column in currentDataSet.Table.Columns )
            ////{
            ////    column.ReadOnly = true;
            ////    versionGrid.Tables[0].Columns[column.ColumnName].Caption = column.Caption;
            ////}

            //// Hide columns that are used for other things.
            HideColumn(ChangeHistoryDataSet.UniqueId);
            HideColumn(ChangeHistoryDataSet.RepositoryPath);
            HideColumn(ChangeHistoryDataSet.VersionNumbers);
            HideColumn(ChangeHistoryDataSet.Comments);

            foreach (DataColumn column in currentDataSet.Table.Columns)
            {
                versionGrid.Columns[column.ColumnName].HeaderText = column.Caption;
                versionGrid.Columns[column.ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            }

            versionGrid.Columns[ChangeHistoryDataSet.Marked].HeaderText = "";

            //// Set up the comments preview row
            //versionGrid.Tables[0].PreviewRowLines = 20;
            //versionGrid.Tables[0].PreviewRowMember = ChangeHistoryDataSet.Comments;

            //currentDataSet.Table.Columns[ChangeHistoryDataSet.Marked].ReadOnly = false;
            //versionGrid.Tables[0].Columns[ChangeHistoryDataSet.Marked].ActAsSelector = true;
            //InitColumnMark( versionGrid.GetRows() );

            //versionGrid.RootTable.Columns[ChangeHistoryDataSet.Marked].AggregateFunction = AggregateFunction.Sum;
            //versionGrid.RootTable.Columns[ChangeHistoryDataSet.DisplayedPath].AggregateFunction = AggregateFunction.Count;
            //versionGrid.TotalRow = InheritableBoolean.True;

            //versionGrid.AutoSizeColumns();

            // Alter the grid based on the current settings
            ShowCommentsRow();
            SetRepositoryPathExpression();
        }

        void versionGrid_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 1; i < versionGrid.Rows.Count; i += 2)
            {
                int commentPos = currentDataSet.Table.Columns.IndexOf(ChangeHistoryDataSet.DisplayedPath);
                Rectangle t1 = versionGrid.GetCellDisplayRectangle(commentPos, i, true);
                int totalWidth = 0;
                int visibleColumns = 0;
                for (int j = 0; j < versionGrid.Columns.Count; j++)
                {
                    if (versionGrid.Columns[j].Visible)
                    {
                        totalWidth += versionGrid.GetCellDisplayRectangle(j, i, true).Width;
                        visibleColumns++;
                    }
                }

                t1.Width = totalWidth - 2;
                string content = versionGrid.Rows[i].Cells[ChangeHistoryDataSet.Comments].Value.ToString();
                t1.Height = (int)e.Graphics.MeasureString(content, versionGrid.Font).Height;
                versionGrid.Rows[i].Height = t1.Height + 1;
                t1.X += 1;
                e.Graphics.FillRectangle(Brushes.White, t1);
                e.Graphics.DrawString(content, versionGrid.Font, Brushes.Black, t1);
            }
        }

        void versionGrid_Scroll(object sender, ScrollEventArgs e)
        {
            versionGrid.Refresh();
        }

        void versionGrid_Resize(object sender, EventArgs e)
        {
            versionGrid.Refresh();
        }

        private void HideColumn( string columnName )
        {
            versionGrid.Columns[columnName].Visible = false;
        }

        private void copyForExcelButton_Click(object sender, System.EventArgs e)
        {
            using( WaitCursor wc = new WaitCursor( this ) )
            {
                // assemble the string
                StringBuilder str = new StringBuilder();

                int namePos = currentDataSet.Table.Columns.IndexOf( ChangeHistoryDataSet.DisplayedPath );
                int startPos = currentDataSet.Table.Columns.IndexOf( ChangeHistoryDataSet.FirstVersion );
                int finishPos = currentDataSet.Table.Columns.IndexOf( ChangeHistoryDataSet.LastVersion );

                foreach( DataRowView row in currentDataSet )
                {
                    str.AppendFormat( "{0}\t{1}\t{2}\t{3}\t{1}\n", row[ namePos ], row[ finishPos ], row[ startPos ], GetDiffType( row[ startPos ] ) );
                }

                // paste it to the clipboard
                Clipboard.SetDataObject( str.ToString(), true );
            }
        }

        private string GetDiffType( object startVersion )
        {
            return GetDiffType( Convert.ToInt32( startVersion ) );
        }

        private string GetDiffType( int startVersion )
        {
            return (startVersion == 0 ? "all" : "diff");
        }

        private void showBlame_Click(object sender, System.EventArgs e)
        {
            using( WaitCursor wc = new WaitCursor( this ) )
            {
                CurrentScmSystem.ShowBlame( SelectedItemId, SelectedItemCurrentVersion - SelectedItemFirstVersion );
            }
        }

        private void DiffCurrentItem( )
        {
            if( CurrentRow == null )
                return;

            DiffCurrentItem( SelectedItemFirstVersion, SelectedItemLastVersion );
        }

        private void DiffCurrentItem( long firstVersion, long lastVersion )
        {
            using( WaitCursor wc = new WaitCursor( this ) )
            {
                Trace.WriteLine( string.Format( "Diffing file {0} (id={1}), from {2} to {3}", SelectedRepositoryPath, SelectedItemId, firstVersion, lastVersion ) );
                CurrentScmSystem.ShowDiff( SelectedItemId, firstVersion, lastVersion );
            }
        }

        private void ViewCurrentItem(  )
        {
            if( CurrentRow == null )
                return;

            ViewCurrentItem( SelectedItemLastVersion );
        }

        private void ViewCurrentItem( long version )
        {
            using( WaitCursor wc = new WaitCursor( this ) )
            {
                Trace.WriteLine( string.Format( "Showing file {0}, version {1}", SelectedRepositoryPath, version ) );
                CurrentScmSystem.ShowFile( SelectedItemId, version );
            }
        }
        
        private void MakeArchiveFromDataSet( )
        {
            MakeArchiveFromDataSet( CurrentDataSet );
        }
        
        private void MakeArchiveFromDataSet( DataSet dataSet )
        {
            if ( dataSet == null ) 
                return;
            
            if ( makeArchiveFileDialog.ShowDialog( ) == DialogResult.OK )
            {                                                            
                using ( WaitCursor wc = new WaitCursor( this ) )
                {
                    Core.ArchiveBuilder archiveBuilder = new Core.ArchiveBuilder( CurrentScmSystem );
                    archiveBuilder.Build( dataSet, makeArchiveFileDialog.FileName );
                }
            }
        }

        private void diffButton_Click(object sender, EventArgs e)
        {
            DiffCurrentItem( );
        }

        private void viewButton_Click(object sender, System.EventArgs e)
        {
            ViewCurrentItem( );
        }

        private void makeArchiveButton_Click(object sender, EventArgs e)
        {
            MakeArchiveFromDataSet( );
        }

        private void PrepareMarkMenuItems( )
        {
            bool isARowSelected = CurrentRow != null;
            markContextMenuItem.Enabled = isARowSelected;
            unmarkContextMenuItem.Enabled = isARowSelected;
            if( isARowSelected )
            {
                //unmarkContextMenuItem.Visible = CurrentRow.RowStyle != null;
                markContextMenuItem.Visible = ! unmarkContextMenuItem.Visible;
            }
        }

        private void ToggleMark( )
        {
            if( CurrentRow == null )
                return;

            //CurrentRow.IsChecked = ! CurrentRow.IsChecked;
        }


        private void markContextMenuItem_Click(object sender, System.EventArgs e)
        {
            ToggleMark( );
        }

        private void gridContextMenu_Popup(object sender, EventArgs e)
        {
            //if( versionGrid.SelectedRows[0].RowType == RowType.Record )
            //{
                PrepareDiffMenu( );
                PrepareViewMenu( );
                PrepareMarkMenuItems( );
            //}
            //else
            //{
            //    foreach( MenuItem m in gridContextMenu.MenuItems )
            //    {
            //        m.Enabled = false;
            //    }
            //}
        }

        #region VersionDiffer helper class

        private class VersionDiffer
        {
            FeatureReviewForm parent;
            long firstVersion;
            long lastVersion;

            public VersionDiffer( FeatureReviewForm parent, long firstVersion, long lastVersion )
            {
                this.parent = parent;
                this.firstVersion = firstVersion;
                this.lastVersion = lastVersion;
            }

            public void Go( object sender, EventArgs e )
            {
                parent.DiffCurrentItem( firstVersion, lastVersion );
            }
        }

        #endregion

        private void PrepareDiffMenu( )
        {
            diffContextMenuItem.MenuItems.Clear( );

            if( CurrentRow != null )
            {
                for( long endVersion = SelectedItemFirstVersion + 1; endVersion <= SelectedItemLastVersion; endVersion ++ )
                {
                    PrepareDiffMenu( diffContextMenuItem.MenuItems.Add( "To " + endVersion + " ..." ), endVersion );
                }

                if( SelectedItemLastVersion != SelectedItemCurrentVersion )
                {
                    PrepareDiffMenu( diffContextMenuItem.MenuItems.Add( "To current ..." ), SelectedItemCurrentVersion );
                }
            }

            diffContextMenuItem.Enabled = (diffContextMenuItem.MenuItems.Count > 0);
        }

        private void PrepareDiffMenu( MenuItem endMenuItem, long endVersion )
        {
            for( long startVersion = endVersion - 1; startVersion >= SelectedItemFirstVersion; startVersion -- )
            {
                endMenuItem.MenuItems.Add( "... from " + startVersion, new EventHandler( new VersionDiffer( this, startVersion, endVersion ).Go ) );
            }
        }

        #region VersionViewer helper class
       
        private class VersionViewer
        {
            FeatureReviewForm parent;
            long version;

            public VersionViewer( FeatureReviewForm parent, long version )
            {
                this.parent = parent;
                this.version = version;
            }

            public void Go( object sender, EventArgs e )
            {
                parent.ViewCurrentItem( version );
            }
        }

        #endregion

        private void PrepareViewMenu( )
        {
            viewContextMenuItem.MenuItems.Clear( );

            if( CurrentRow != null )
            {
                foreach( string versionNumber in GetSelectedString( ChangeHistoryDataSet.VersionNumbers ).Split( ',' ) )
                {
                    MenuItem viewItem = new MenuItem( "Version " + versionNumber );
                    viewItem.Click += new EventHandler( new VersionViewer( this, Convert.ToInt64( versionNumber ) ).Go );
                    viewContextMenuItem.MenuItems.Add( 0, viewItem );
                }
            }

            viewContextMenuItem.Enabled = (viewContextMenuItem.MenuItems.Count > 0);
        }

        private IEnumerable GetClumps( string versionNumbers )
        {
            return GetClumps( versionNumbers.Split( ',' ) );
        }

        private IEnumerable GetClumps( string [] versionNumbers )
        {
            long [] convertedVersionNumbers = new long [ versionNumbers.Length ];
            for( int i = 0; i < versionNumbers.Length; i++ )
            {
                convertedVersionNumbers[i] = Convert.ToInt64( versionNumbers[i] );
            }
            return GetClumps( convertedVersionNumbers );
        }

        private IEnumerable GetClumps( long [] versionNumbers )
        {
            ArrayList clumps = new ArrayList( );
            long start = versionNumbers[0] - 1;
            long last = start;
            for( int i = 0; i < versionNumbers.Length; i++ )
            {
                // There's a break in the versions
                if( versionNumbers[i] != last + 1 )
                {
                    clumps.Add( new VersionClump( start, last ) );
                    start = versionNumbers[i] - 1;
                }
                last = versionNumbers[i];
            }
            clumps.Add( new VersionClump( start, last ) );
            return clumps;
        }

        #region VersionClump helper class

        private class VersionClump
        {
            public readonly long Start;
            public readonly long End;

            public VersionClump( long start, long end )
            {
                this.Start = start;
                this.End = end;
            }

            public override string ToString()
            {
                return string.Format( "{0} to {1}", Start, End );
            }

        }

        #endregion

        private object GetSelectedCell( string column )
        {
            return CurrentRow[ column ];
        }

        private string GetSelectedString( string column )
        {
            return GetSelectedCell( column ).ToString( );
        }

        private long GetSelectedLong( string column )
        {
            return Convert.ToInt64( GetSelectedCell( column ) );
        }

        private void FeatureReviewForm_Activate( object sender, EventArgs e )
        {
            Activated -= new EventHandler( FeatureReviewForm_Activate );
            LoadSettings();
        }

        private void UseGoForAcceptButton(object sender, System.EventArgs e)
        {
            AcceptButton = getFeatureHistoryButton;
        }

        private void DoNotUseGoForAcceptButton(object sender, System.EventArgs e)
        {
            AcceptButton = null;
        }

        private void SetCommentsRowVisibility( object sender, System.EventArgs e )
        {
            ShowCommentsRow( );
        }

        private void showFullRepositoryPath_CheckedChanged(object sender, System.EventArgs e)
        {
            errorProvider1.SetError( (Control) sender, null );
            try
            {
                SetRepositoryPathExpression( );
                versionGrid.Refresh( );
            }
            catch( Exception ex )
            {
                errorProvider1.SetError( (Control) sender, ex.Message );
            }
        }

        private void getFeatureHistoryEditBox_Enter(object sender, System.EventArgs e)
        {
            UseGoForAcceptButton( sender, e );
            SelectAllContents( sender, e );
        }

        private void SelectAllContents( object sender, EventArgs e )
        {
            TextBox textBox = sender as TextBox;
            if( textBox == null )
                return;

            textBox.SelectAll( );
        }

        private void ShowCommentsRow( )
        {
            List<DataRow> rowsToAdd = new List<DataRow>();
            foreach (DataRow row in currentDataSet.Table.Rows)
            {
                DataRow newRow = currentDataSet.Table.NewRow();
                newRow[ChangeHistoryDataSet.UniqueId] = row[ChangeHistoryDataSet.UniqueId].ToString() + "comments";
                newRow[ChangeHistoryDataSet.RepositoryPath] = row[ChangeHistoryDataSet.RepositoryPath].ToString() + ".";
                newRow[ChangeHistoryDataSet.Comments] = row[ChangeHistoryDataSet.Comments];
                rowsToAdd.Add(newRow);
            }
            int i = 1;
            foreach (DataRow row in rowsToAdd)
            {
                currentDataSet.Table.Rows.InsertAt(row, i);
                i += 2;
            }
            
        }

        private void SetRepositoryPathExpression( )
        {
            DataColumn repositoryPathColumn = currentDataSet.Table.Columns[ ChangeHistoryDataSet.DisplayedPath ];
            if( showFullRepositoryPath.Checked )
            {
                ShowFullPath( repositoryPathColumn );
            }
            else
            {
                HideFullPath( repositoryPathColumn );
            }
        }

        private void ShowFullPath( DataColumn repositoryPathColumn )
        {
            lock( repositoryPathColumn )
            {
                if( repositoryPathColumn.ExtendedProperties.Contains( "truncate.expression" ) )
                    return;

                repositoryPathColumn.ExtendedProperties.Add( "truncate.expression", repositoryPathColumn.Expression );
                repositoryPathColumn.Expression = ChangeHistoryDataSet.RepositoryPath;
            }
        }

        private void HideFullPath( DataColumn repositoryPathColumn )
        {
            lock( repositoryPathColumn )
            {
                if( ! repositoryPathColumn.ExtendedProperties.Contains( "truncate.expression" ) )
                    return;

                repositoryPathColumn.Expression = (string) repositoryPathColumn.ExtendedProperties[ "truncate.expression" ];
                repositoryPathColumn.ExtendedProperties.Remove( "truncate.expression" );
            }
        }

        private void versionGrid_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            switch( e.KeyData )
            {
                case Keys.Control | Keys.C:
                    if( versionGrid.SelectedRows.Count == 1 )
                    {
                        Clipboard.SetDataObject(versionGrid.SelectedRows[0].Cells[ChangeHistoryDataSet.DisplayedPath].Value, true);
                    }
                    break;

                case Keys.M:
                case Keys.Space:
                    ToggleMark( );
                    break;

                case Keys.D:
                    DiffCurrentItem( );
                    break;

                case Keys.V:
                    ViewCurrentItem( );
                    break;

                case Keys.J:
                case Keys.N:
                case Keys.F:
                case Keys.H:
                    if(versionGrid.RowCount > versionGrid.SelectedRows[0].Index + 1){
                        versionGrid.Rows[versionGrid.SelectedRows[0].Index + 1].Selected = true;
                        versionGrid.SelectedRows[0].Selected = false;
                    }
                    break;

                case Keys.K:
                case Keys.P:
                case Keys.B:
                case Keys.L:
                    if (versionGrid.SelectedRows[0].Index - 1 > 0)
                    {
                        versionGrid.SelectedRows[0].Selected = false;
                        versionGrid.Rows[versionGrid.SelectedRows[0].Index - 1].Selected = true; 
                    }
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        private void FeatureReviewForm_Closing( object sender, CancelEventArgs e )
        {
            SaveSettings( e );
        }

        #region Settings

        private void SaveSettings( CancelEventArgs e )
        {
            SaveConnectionSettings( e );
            if( !e.Cancel )
            {
                Settings.Default.LastScmSystem = (CurrentScmSystem == null ? "" : CurrentScmSystem.ToString());
                Settings.Default.WindowLocation = NormalBounds.Location;
                Settings.Default.WindowSize = NormalBounds.Size;
                Settings.Default.SearchResults = GetXml( CurrentDataSet );
                Settings.Default.SearchString = getFeatureHistoryEditBox.Text;
                Settings.Default.Save();
            }
        }

        private void SaveConnectionSettings( CancelEventArgs e )
        {
            foreach( IScmSystemHook scmSystemHook in scmSystem.Items )
            {
                scmSystemHook.SaveConnectionSettings(this, e );
                if( e.Cancel )
                {
                    // Stop asking the user if stuff should be saved.
                    break;
                }
            }
        }

        private void LoadSettings()
        {
            Settings settings = Settings.Default;
            LoadScmSystem( settings );
            LoadWindowBounds( settings );
            LoadSearchResults( settings );
            getFeatureHistoryEditBox.Text = settings.SearchString;
        }

        private void LoadScmSystem( Settings settings )
        {
            string scmSystemId = settings.LastScmSystem;
            foreach( IScmSystemHook hook in scmSystem.Items )
            {
                if( hook.ToString() == scmSystemId )
                {
                    scmSystem.SelectedItem = hook;
                    return;
                }
            }
        }

        private void LoadWindowBounds( Settings settings )
        {
            try
            {
                if( settings.WindowLocation != Point.Empty && settings.WindowSize != Size.Empty )
                {
                    Bounds = new Rectangle( settings.WindowLocation, settings.WindowSize );
                }
            }
            catch( Exception )
            {
                // This is probably the first time the app has been run
            }
        }

        private void LoadSearchResults( Settings settings )
        {
            try
            {
                CurrentDataSet = ToDataSet( settings.SearchResults );
            }
            catch( Exception )
            {
                // This is probably the first time the app has been run
            }
        }

        private string GetXml( DataSet dataSet )
        {
            if( dataSet == null )
                return "";

            System.IO.StringWriter writer = new System.IO.StringWriter();
            dataSet.WriteXml( writer, XmlWriteMode.WriteSchema );
            writer.Close();
            return writer.ToString();
        }

        private DataSet ToDataSet( string serializedDataSet )
        {
            DataSet ds = new DataSet();
            ds.ReadXml( new System.IO.StringReader( serializedDataSet ) );
            return ds;
        }

        #endregion

        private void scmSystem_SelectedIndexChanged( object sender, EventArgs e )
        {
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add( CurrentScmSystem.ConnectionSettingsControl );
            CurrentScmSystem.ConnectionSettingsControl.Dock = DockStyle.Fill;
            showBlame.Enabled = CurrentScmSystem.CanShowBlame;
            makeArchiveButton.Enabled = CurrentScmSystem.CanGetFile;
        }

        private void viewTraceButton_Click( object sender, EventArgs e )
        {
            new TraceWindow().Visible = true;
        }

        private void setToolsButton_Click( object sender, EventArgs e )
        {
            ToolsSettingsWindow ts = new ToolsSettingsWindow();
            ts.ScmSystem = CurrentScmSystem;
            ts.AvailableScmSystems = scmSystem.Items;
            ts.Show();
        }
    }
}
