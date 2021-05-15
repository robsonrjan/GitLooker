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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepoSources));
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.White;
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(616, 170);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.Gray;
            panel2.Dock = DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(616, 1);
            panel2.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = System.Drawing.Color.WhiteSmoke;
            button1.Dock = DockStyle.Bottom;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            button1.ForeColor = System.Drawing.Color.Navy;
            button1.Location = new System.Drawing.Point(0, 170);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(616, 33);
            button1.TabIndex = 1;
            button1.Text = "Add new repo path";
            button1.UseVisualStyleBackColor = false;
            button1.Click += new System.EventHandler(button1_Click);
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            removeToolStripMenuItem});
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            removeToolStripMenuItem.Text = "Remove";
            removeToolStripMenuItem.Click += new System.EventHandler(removeToolStripMenuItem_Click);
            // 
            // button2
            // 
            button2.BackColor = System.Drawing.Color.WhiteSmoke;
            button2.Dock = DockStyle.Bottom;
            button2.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            button2.ForeColor = System.Drawing.Color.Navy;
            button2.Location = new System.Drawing.Point(0, 203);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(616, 33);
            button2.TabIndex = 2;
            button2.Text = "Remove selected repo";
            button2.UseVisualStyleBackColor = false;
            button2.Click += new System.EventHandler(removeToolStripMenuItem_Click);
            // 
            // button3
            // 
            button3.BackColor = System.Drawing.Color.WhiteSmoke;
            button3.Dock = DockStyle.Bottom;
            button3.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            button3.ForeColor = System.Drawing.Color.Navy;
            button3.Location = new System.Drawing.Point(0, 236);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(616, 33);
            button3.TabIndex = 3;
            button3.Text = "Close and save";
            button3.UseVisualStyleBackColor = false;
            button3.Click += new System.EventHandler(button3_Click);
            // 
            // RepoSources
            // 
            ClientSize = new System.Drawing.Size(616, 269);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(button3);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(900, 300);
            MinimizeBox = false;
            Name = "RepoSources";
            Text = "Repo sources ";
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(RepoSources_FormClosing);
            panel1.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                AddEntry(folderBrowserDialog.SelectedPath);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentEntry != default)
            {
                panel1.Controls.Remove(currentEntry);
                currentEntry.Dispose();
            }
        }

        private void RepoSources_FormClosing(object sender, FormClosingEventArgs e)
        {
            repoList.Clear();
            foreach (var ctr in panel1.Controls)
            {
                var entry = ctr as EntryControl;
                if (entry != default)
                    repoList.Add(entry.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e) => Close();
    }
}
