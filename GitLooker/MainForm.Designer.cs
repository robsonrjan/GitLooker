namespace GitLooker
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setWorkingPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox3 = new System.Windows.Forms.ToolStripTextBox();
            this.executeAppToManageProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox5 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox6 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox7 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkWithConnectionErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pullAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteReposConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expectedReposConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exortConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox4 = new System.Windows.Forms.ToolStripTextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.updateStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.checkOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.reposCatalogs = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.reposCatalogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.checkToolStripMenuItem,
            this.toolStripMenuItem1,
            this.configurationToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripTextBox4});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(584, 29);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setWorkingPathToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem3,
            this.toolStripComboBox1,
            this.toolStripSeparator2,
            this.toolStripMenuItem5,
            this.toolStripTextBox1,
            this.toolStripSeparator3,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripTextBox2,
            this.toolStripMenuItem11,
            this.toolStripTextBox3,
            this.executeAppToManageProjectToolStripMenuItem,
            this.toolStripTextBox5,
            this.toolStripMenuItem6,
            this.toolStripTextBox6,
            this.toolStripMenuItem13,
            this.toolStripTextBox7,
            this.toolStripMenuItem12});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // setWorkingPathToolStripMenuItem
            // 
            this.setWorkingPathToolStripMenuItem.Name = "setWorkingPathToolStripMenuItem";
            this.setWorkingPathToolStripMenuItem.Size = new System.Drawing.Size(310, 24);
            this.setWorkingPathToolStripMenuItem.Text = "Set working path";
            this.setWorkingPathToolStripMenuItem.Click += new System.EventHandler(this.SetWorkingPathToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(307, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem3.Text = "Autocheck every: ";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "none",
            "1 hour",
            "2 hours",
            "3 hours",
            "4 hours"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(250, 25);
            this.toolStripComboBox1.Text = "none";
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(307, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Enabled = false;
            this.toolStripMenuItem5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem5.Text = "Main branch:";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox1.Text = "master";
            this.toolStripTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyUp);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(307, 6);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Enabled = false;
            this.toolStripMenuItem9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem9.Text = "Command configuration:";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem10.Text = "execute app to manage repo:";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox2.ToolTipText = "Choose executable file to memage selected repo,\r\nexecutable will be executed with" +
    " repo full path parameter.\r\nPress [Insert] key to choose one.";
            this.toolStripTextBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox2_KeyDown);
            this.toolStripTextBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveConfiguration);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem11.Text = "argument:";
            // 
            // toolStripTextBox3
            // 
            this.toolStripTextBox3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox3.ToolTipText = "[optional] Additional argument needed to proper pass full path parameter to execu" +
    "table.";
            this.toolStripTextBox3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveConfiguration);
            // 
            // executeAppToManageProjectToolStripMenuItem
            // 
            this.executeAppToManageProjectToolStripMenuItem.Name = "executeAppToManageProjectToolStripMenuItem";
            this.executeAppToManageProjectToolStripMenuItem.Size = new System.Drawing.Size(310, 24);
            this.executeAppToManageProjectToolStripMenuItem.Text = "execute app to manage project";
            // 
            // toolStripTextBox5
            // 
            this.toolStripTextBox5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox5.Name = "toolStripTextBox5";
            this.toolStripTextBox5.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox5.ToolTipText = "[optional] Choose executable file to memage selected project,\r\nexecutable will be" +
    " executed with repo full path parameter.\r\nPress [Insert] key to choose one.";
            this.toolStripTextBox5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox5_KeyDown);
            this.toolStripTextBox5.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveConfiguration);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem6.Text = "argument:";
            // 
            // toolStripTextBox6
            // 
            this.toolStripTextBox6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox6.Name = "toolStripTextBox6";
            this.toolStripTextBox6.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox6.ToolTipText = "[optional] Additional argument needed to proper pass full path parameter to execu" +
    "table.";
            this.toolStripTextBox6.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveConfiguration);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem13.Text = "file extension:";
            // 
            // toolStripTextBox7
            // 
            this.toolStripTextBox7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox7.Name = "toolStripTextBox7";
            this.toolStripTextBox7.Size = new System.Drawing.Size(250, 23);
            this.toolStripTextBox7.ToolTipText = "Project file extension";
            this.toolStripTextBox7.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveConfiguration);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Enabled = false;
            this.toolStripMenuItem12.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(310, 24);
            this.toolStripMenuItem12.Text = "to save press ENTER";
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.checkToolStripMenuItem.AutoToolTip = true;
            this.checkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkStatusToolStripMenuItem,
            this.checkWithConnectionErrorToolStripMenuItem,
            this.pullAllToolStripMenuItem});
            this.checkToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Padding = new System.Windows.Forms.Padding(40, 0, 4, 0);
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(75, 23);
            this.checkToolStripMenuItem.Text = "Git";
            this.checkToolStripMenuItem.ToolTipText = "Check for updates";
            this.checkToolStripMenuItem.DropDownOpening += new System.EventHandler(this.checkToolStripMenuItem_DropDownOpening);
            // 
            // checkStatusToolStripMenuItem
            // 
            this.checkStatusToolStripMenuItem.Name = "checkStatusToolStripMenuItem";
            this.checkStatusToolStripMenuItem.Size = new System.Drawing.Size(250, 24);
            this.checkStatusToolStripMenuItem.Text = "Check status";
            this.checkStatusToolStripMenuItem.Click += new System.EventHandler(this.CheckToolStripMenuItem_Click);
            // 
            // checkWithConnectionErrorToolStripMenuItem
            // 
            this.checkWithConnectionErrorToolStripMenuItem.Name = "checkWithConnectionErrorToolStripMenuItem";
            this.checkWithConnectionErrorToolStripMenuItem.Size = new System.Drawing.Size(250, 24);
            this.checkWithConnectionErrorToolStripMenuItem.Text = "Check with connection error";
            this.checkWithConnectionErrorToolStripMenuItem.Click += new System.EventHandler(this.checkWithConnectionErrorToolStripMenuItem_Click);
            // 
            // pullAllToolStripMenuItem
            // 
            this.pullAllToolStripMenuItem.Name = "pullAllToolStripMenuItem";
            this.pullAllToolStripMenuItem.Size = new System.Drawing.Size(250, 24);
            this.pullAllToolStripMenuItem.Text = "Pull all";
            this.pullAllToolStripMenuItem.Click += new System.EventHandler(this.pullAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem1.Image = global::GitLooker.Properties.Resources.uo_st___Copy;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(28, 23);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteReposConfigToolStripMenuItem,
            this.expectedReposConfigToolStripMenuItem,
            this.exortConfigurationToolStripMenuItem,
            this.importConfigurationToolStripMenuItem});
            this.configurationToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(105, 23);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // remoteReposConfigToolStripMenuItem
            // 
            this.remoteReposConfigToolStripMenuItem.Name = "remoteReposConfigToolStripMenuItem";
            this.remoteReposConfigToolStripMenuItem.Size = new System.Drawing.Size(211, 24);
            this.remoteReposConfigToolStripMenuItem.Text = "Remote repos config";
            this.remoteReposConfigToolStripMenuItem.Click += new System.EventHandler(this.remoteReposConfigToolStripMenuItem_Click);
            // 
            // expectedReposConfigToolStripMenuItem
            // 
            this.expectedReposConfigToolStripMenuItem.Name = "expectedReposConfigToolStripMenuItem";
            this.expectedReposConfigToolStripMenuItem.Size = new System.Drawing.Size(211, 24);
            this.expectedReposConfigToolStripMenuItem.Text = "Expected repos config";
            this.expectedReposConfigToolStripMenuItem.Click += new System.EventHandler(this.expectedReposConfigToolStripMenuItem_Click);
            // 
            // exortConfigurationToolStripMenuItem
            // 
            this.exortConfigurationToolStripMenuItem.Name = "exortConfigurationToolStripMenuItem";
            this.exortConfigurationToolStripMenuItem.Size = new System.Drawing.Size(211, 24);
            this.exortConfigurationToolStripMenuItem.Text = "Export configuration";
            this.exortConfigurationToolStripMenuItem.Click += new System.EventHandler(this.exortConfigurationToolStripMenuItem_Click);
            // 
            // importConfigurationToolStripMenuItem
            // 
            this.importConfigurationToolStripMenuItem.Name = "importConfigurationToolStripMenuItem";
            this.importConfigurationToolStripMenuItem.Size = new System.Drawing.Size(211, 24);
            this.importConfigurationToolStripMenuItem.Text = "Import configuration";
            this.importConfigurationToolStripMenuItem.Click += new System.EventHandler(this.importConfigurationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem2.AutoToolTip = true;
            this.toolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Padding = new System.Windows.Forms.Padding(40, 0, 4, 0);
            this.toolStripMenuItem2.Size = new System.Drawing.Size(92, 23);
            this.toolStripMenuItem2.Text = "Clone";
            this.toolStripMenuItem2.ToolTipText = "Check for updates";
            this.toolStripMenuItem2.Visible = false;
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem4.ForeColor = System.Drawing.Color.Navy;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(67, 23);
            this.toolStripMenuItem4.Text = "Last update: ";
            // 
            // toolStripTextBox4
            // 
            this.toolStripTextBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.toolStripTextBox4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.toolStripTextBox4.Name = "toolStripTextBox4";
            this.toolStripTextBox4.Size = new System.Drawing.Size(120, 23);
            this.toolStripTextBox4.Text = "repo filter";
            this.toolStripTextBox4.Enter += new System.EventHandler(this.toolStripTextBox4_Enter);
            this.toolStripTextBox4.Leave += new System.EventHandler(this.toolStripTextBox4_Leave);
            this.toolStripTextBox4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox4_KeyUp);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Some repositories needs updates.";
            this.notifyIcon1.BalloonTipTitle = "Git repo info";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem14,
            this.updateStatusToolStripMenuItem,
            this.toolStripSeparator4,
            this.checkOnToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(274, 120);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(273, 22);
            this.toolStripMenuItem7.Text = "Open folder";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(273, 22);
            this.toolStripMenuItem8.Text = "Execute command to manage repo";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(273, 22);
            this.toolStripMenuItem14.Text = "Execute command to manage project";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.toolStripMenuItem14_Click);
            // 
            // updateStatusToolStripMenuItem
            // 
            this.updateStatusToolStripMenuItem.Name = "updateStatusToolStripMenuItem";
            this.updateStatusToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.updateStatusToolStripMenuItem.Text = "Update status   [Shift+D]";
            this.updateStatusToolStripMenuItem.Click += new System.EventHandler(this.updateStatusToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(270, 6);
            // 
            // checkOnToolStripMenuItem
            // 
            this.checkOnToolStripMenuItem.Name = "checkOnToolStripMenuItem";
            this.checkOnToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.checkOnToolStripMenuItem.Text = "Check on ";
            this.checkOnToolStripMenuItem.Click += new System.EventHandler(this.checkOnToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // reposCatalogs
            // 
            this.reposCatalogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reposCatalogs.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.reposCatalogs.Location = new System.Drawing.Point(0, 29);
            this.reposCatalogs.Name = "reposCatalogs";
            this.reposCatalogs.SelectedIndex = 0;
            this.reposCatalogs.Size = new System.Drawing.Size(584, 732);
            this.reposCatalogs.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 761);
            this.Controls.Add(this.reposCatalogs);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(600, 800);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "MainForm";
            this.Text = "Git branch changes looker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.reposCatalogs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setWorkingPathToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteReposConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expectedReposConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem updateStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox4;
        private System.Windows.Forms.ToolStripMenuItem checkOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pullAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkWithConnectionErrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem exortConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem executeAppToManageProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.TabControl reposCatalogs;
    }
}

