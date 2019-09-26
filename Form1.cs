using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class Form1 : Form
    {
        private string chosenPath = string.Empty;
        private readonly SemaphoreSlim semaphore;
        public Form1()
        {
            InitializeComponent();
            semaphore = new SemaphoreSlim(3);
        }

        private void SetWorkingPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(chosenPath))
                folderBrowserDialog1.SelectedPath = chosenPath;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            folderBrowserDialog1.ShowDialog();
            var path = folderBrowserDialog1.SelectedPath;
            if (!string.IsNullOrEmpty(path) && (chosenPath != path))
            {
                chosenPath = path;
                config.AppSettings.Settings.Remove("GirLookerPath");
                config.AppSettings.Settings.Add("GirLookerPath", chosenPath);
                config.Save();
                CheckForGitRepo(chosenPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chosenPath = ConfigurationManager.AppSettings["GirLookerPath"];

            if (!string.IsNullOrEmpty(chosenPath))
                CheckForGitRepo(chosenPath);

        }

        private void CheckForGitRepo(string chosenPath)
        {
            var dir = Directory.GetDirectories(chosenPath).ToList();
            if (dir.Any(d => d.EndsWith(".git")))
                CheckRepo(chosenPath);
            else
                dir.ForEach(d =>
                {
                    CheckForGitRepo(d);
                });
        }

        private void CheckRepo(string repoDdir)
        {
            var repo = new RepoControl(repoDdir, semaphore, new PowersShell());
            repo.Dock = DockStyle.Top;
            this.panel1.Controls.Add(repo);
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var cntr in panel1.Controls)
                if(cntr is RepoControl)
                    ((RepoControl)cntr).UpdateRepoInfo();
        }
    }
}
