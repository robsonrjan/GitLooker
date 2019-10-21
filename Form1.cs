using GitLooker.CommandProcessor;
using GitLooker.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class Form1 : Form
    {
        private string chosenPath = string.Empty;
        private readonly SemaphoreSlim semaphore;
        private IRepoControlConfiguration repoConfiguration;
        private IPowersShell powerShell;
        private ICommandProcessor controlConfiguration;
        private const int repoProcessingCount = 3;
        internal static List<string> RepoRemoteList;
        internal static List<string> ExpectedRemoteList;

        public Form1()
        {
            InitializeComponent();
            semaphore = new SemaphoreSlim(repoProcessingCount);
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
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
                GenerateAndUpdateRepos();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chosenPath = ConfigurationManager.AppSettings["GirLookerPath"];

            if (!string.IsNullOrEmpty(chosenPath))
                GenerateAndUpdateRepos();

            this.Text += $"    ver.{AppVersion.AssemblyVersion}";
        }

        private void GenerateAndUpdateRepos()
        {
            CheckForGitRepo(chosenPath);
            CheckToolStripMenuItem_Click(null, null);
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
            repoConfiguration = new RepoControlConfiguration(repoDdir, semaphore);
            powerShell = new PowersShell();
            controlConfiguration = new CommandProcessor.CommandProcessor(powerShell);
            var repo = new RepoControl(repoConfiguration, controlConfiguration);
            repo.Dock = DockStyle.Top;
            this.panel1.Controls.Add(repo);
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItem1.Visible = true;
            Application.DoEvents();
            Application.DoEvents();
            checkToolStripMenuItem.Enabled = false;
            foreach (var cntr in panel1.Controls)
                if (cntr is RepoControl)
                    ((RepoControl)cntr).UpdateRepoInfo();

            Task.Run(() => CheckStatusProgress());
        }

        private void CheckStatusProgress()
        {
            System.Threading.Thread.Sleep(2000);

            while (repoConfiguration.Semaphore.CurrentCount < repoProcessingCount)
                System.Threading.Thread.Sleep(50);

            this.Invoke(new Action(() =>
            {
                checkToolStripMenuItem.Enabled = true;
                toolStripMenuItem1.Visible = false;
            }), null);
        }

        private void remoteReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = RepoRemoteList.ToArray();
            repoList.ShowDialog();
        }

        private void expectedReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = ExpectedRemoteList.ToArray();
            repoList.ShowDialog();
            ExpectedRemoteList = repoList.repoText.Lines.ToList();


        }
    }
}
