using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GitLooker.Controls
{
    public class RepoSources : Form
    {
        private Button button1;
        private ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem removeToolStripMenuItem;
        private Panel panel1;
        private readonly FolderBrowserDialog folderBrowserDialog;
        private readonly List<string> repoList;
        private Panel panel2;
        private Button button2;
        private Button button3;
        private EntryControl currentEntry;

        public string ChosenSolution => currentEntry.Text;

        public IEnumerable<string> RepoList => repoList;

        public RepoSources(FolderBrowserDialog folderBrowserDialog, List<string> repoList) : this()
        {
            this.folderBrowserDialog = folderBrowserDialog;
            this.repoList = repoList;

            foreach (var repo in repoList)
                AddEntry(repo);
        }

        public RepoSources(List<string> repoList) : this()
        {
            folderBrowserDialog = default;
            this.repoList = repoList;

            foreach (var repo in repoList)
                AddEntry(repo);

            Text = "Pick solution";
            button1.Visible = false;
            button2.Visible = false;
            button3.Text = "Choose and close";
        }

        private void AddEntry(string value)
        {
            var entry = new EntryControl(value);
            entry.MouseClick += Entry_Click;
            panel1.Controls.Add(entry);
            panel2.SendToBack();
        }

        private void Entry_Click(object sender, EventArgs e)
        {
            currentEntry = sender as EntryControl;
            if (folderBrowserDialog != default)
                currentEntry.ContextMenuStrip = contextMenuStrip1;
        }

        public RepoSources() => InitializeComponent();

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepoSources));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 170);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(616, 1);
            this.panel2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button1.ForeColor = System.Drawing.Color.Navy;
            this.button1.Location = new System.Drawing.Point(0, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(616, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add new repo path";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button2.ForeColor = System.Drawing.Color.Navy;
            this.button2.Location = new System.Drawing.Point(0, 203);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(616, 33);
            this.button2.TabIndex = 2;
            this.button2.Text = "Remove selected repo";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button3.ForeColor = System.Drawing.Color.Navy;
            this.button3.Location = new System.Drawing.Point(0, 236);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(616, 33);
            this.button3.TabIndex = 3;
            this.button3.Text = "Close and save";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // RepoSources
            // 
            this.ClientSize = new System.Drawing.Size(616, 269);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 300);
            this.MinimizeBox = false;
            this.Name = "RepoSources";
            this.Text = "Repo sources ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RepoSources_FormClosing);
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
                AddEntry(folderBrowserDialog.SelectedPath);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentEntry != default)
            {
                this.panel1.Controls.Remove(currentEntry);
                currentEntry.Dispose();
            }
        }

        private void RepoSources_FormClosing(object sender, FormClosingEventArgs e)
        {
            repoList.Clear();
            foreach(var ctr in panel1.Controls)
            {
                var entry = ctr as EntryControl;
                if (entry != default)
                    repoList.Add(entry.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e) => Close();
    }
}
