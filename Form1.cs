using GitLooker.CommandProcessor;
using GitLooker.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class Form1 : Form
    {
        private const string repoFileConfigurationName = "repos.json";
        private const int maxPandingGitOperations = 3;
        private int repoProcessingCount;
        private string chosenPath = string.Empty;
        private SemaphoreSlim semaphore;
        private IRepoControlConfiguration repoConfiguration;
        private IPowersShell powerShell;
        private ICommandProcessor commandProcessor;
        private List<RepoControl> allReposControl;
        internal static List<string> RepoRemoteList;
        internal static List<string> ExpectedRemoteList;

        public Form1()
        {
            InitializeComponent();
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
            allReposControl = new List<RepoControl>();
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
            if (!int.TryParse(ConfigurationManager.AppSettings["repoProcessingCount"], out repoProcessingCount))
                repoProcessingCount = maxPandingGitOperations;

            semaphore = new SemaphoreSlim(repoProcessingCount);

            if (!string.IsNullOrEmpty(chosenPath))
                GenerateAndUpdateRepos();

            ReadRepositoriumConfiguration();

            this.Text += $"    ver.{AppVersion.AssemblyVersion}";
        }

        private static void ReadRepositoriumConfiguration()
        {
            if (File.Exists(repoFileConfigurationName))
            {
                var jsonserializer = new DataContractJsonSerializer(typeof(List<string>));
                using (var stream = File.OpenRead(repoFileConfigurationName))
                    ExpectedRemoteList = (List<string>)jsonserializer.ReadObject(stream);
            }
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

        private RepoControl CheckRepo(string repoDdir, string newRepo = default(string))
        {
            repoConfiguration = new RepoControlConfiguration(repoDdir, semaphore, newRepo);
            powerShell = new PowersShell();
            commandProcessor = new CommandProcessor.CommandProcessor(powerShell);
            var repo = new RepoControl(repoConfiguration, commandProcessor);
            repo.Dock = DockStyle.Top;
            allReposControl.Add(repo);
            this.panel1.Controls.Add(repo);
            return repo;
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItem1.Visible = true;
            Application.DoEvents();
            Application.DoEvents();
            checkToolStripMenuItem.Enabled = false;

            CheackAndRemovedNewRepos();
            allReposControl.ForEach(cntr => cntr.UpdateRepoInfo());
            Task.Run(() => CheckStatusProgress());
        }

        private void CheackAndRemovedNewRepos()
        {
            foreach (var ctrRepo in allReposControl.Where(repo => repo.IsNew && !ExpectedRemoteList.Contains(repo.RepoConfiguration)))
                ctrRepo.Dispose();
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
                AddMissingRepositoriums();
            }), null);
        }

        private bool NotInRepoConfig(string config) => !RepoRemoteList.Contains(config) && !allReposControl.Any(ctr => ctr.RepoConfiguration == config);
        private void AddMissingRepositoriums()
        {
            ExpectedRemoteList.Where(NotInRepoConfig).ToList()
                .ForEach(config =>
                {
                    CheckRepo(chosenPath, config);
                    toolStripMenuItem2.Visible = true;
                });
        }

        private void remoteReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = RepoRemoteList.ToArray();
            repoList.ShowDialog();
        }

        private string ToLower(string text) => text.ToLower();

        private void expectedReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = ExpectedRemoteList.ToArray();
            repoList.ShowDialog();

            if (File.Exists(repoFileConfigurationName))
                File.Delete(repoFileConfigurationName);

            ExpectedRemoteList = repoList.repoText.Lines.Select(ToLower).Distinct().ToList();
            var jsonserializer = new DataContractJsonSerializer(typeof(List<string>));

            using (var stream = File.OpenWrite(repoFileConfigurationName))
                jsonserializer.WriteObject(stream, ExpectedRemoteList);

        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var pwShell = new PowersShell();
            var commandProc = new CommandProcessor.CommandProcessor(powerShell);

            await CloneNewRepo(commandProc);

            toolStripMenuItem2.Visible = false;
        }

        private Task CloneNewRepo(CommandProcessor.CommandProcessor commandProc)
        {
            allReposControl.Where(ctr => ctr.IsNew).ToList()
                .ForEach(ctr =>
                {
                    var result = commandProc.ClonRepo(chosenPath, ctr.RepoConfiguration);
                    var repoPath = $@"{chosenPath}\{ctr.GetNewRepoName}";
                    if (Directory.Exists(repoPath))
                    {
                        var repo = CheckRepo(repoPath);
                        ctr.Dispose();
                        repo.UpdateRepoInfo();
                    }
                });
            return Task.CompletedTask;
        }
    }
}
