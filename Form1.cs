using GitLooker.CommandProcessor;
using GitLooker.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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
        private IAppSemaphoreSlim semaphore;
        private IRepoControlConfiguration repoConfiguration;
        private IPowersShell powerShell;
        private IRepoCommandProcessor commandProcessor;
        private List<RepoControl> allReposControl;
        private int intervalUpdateCheckHour;
        private DateTime lastTimeUpdate;
        internal static List<string> RepoRemoteList;
        internal static List<string> ExpectedRemoteList;
        private bool isLoaded;

        public Form1()
        {
            InitializeComponent();
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
            allReposControl = new List<RepoControl>();
            lastTimeUpdate = DateTime.UtcNow;
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
                Clear();
                chosenPath = path;
                config.AppSettings.Settings.Remove("GirLookerPath");
                config.AppSettings.Settings.Add("GirLookerPath", chosenPath);
                config.Save();
                GenerateAndUpdateRepos();
            }
        }

        private void Clear()
        {
            panel1.Controls.Clear();            
            allReposControl.ForEach(r => r.Dispose());
            allReposControl.Clear();
            RepoRemoteList.Clear();
            toolStripMenuItem2.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chosenPath = ConfigurationManager.AppSettings["GirLookerPath"];
            if (!int.TryParse(ConfigurationManager.AppSettings["repoProcessingCount"], out repoProcessingCount))
                repoProcessingCount = maxPandingGitOperations;
            if (!int.TryParse(ConfigurationManager.AppSettings["intervalUpdateCheckHour"], out intervalUpdateCheckHour))
                intervalUpdateCheckHour = 0;
            SetMenueCheckerValue();

            semaphore = new AppSemaphoreSlim(repoProcessingCount);
            semaphore.OnUse += SemaphoreIsUsed;
            ReadRepositoriumConfiguration();

            if (!string.IsNullOrEmpty(chosenPath))
                GenerateAndUpdateRepos();            

            this.Text += $"    ver.{AppVersion.AssemblyVersion}";
            isLoaded = true;
        }

        private void SemaphoreIsUsed(bool isProccesing)
            => this.Invoke(new Action(() =>
            {
                toolStripMenuItem2.Enabled = checkToolStripMenuItem.Enabled = !(toolStripMenuItem1.Visible = isProccesing);
                if (!isProccesing)
                {
                    endControl.SendToBack();
                    endControl.Focus();
                    AddMissingRepositoriums();
                }
            }), null);

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

            if (!allReposControl.Any() && !string.IsNullOrEmpty(chosenPath))
                AddMissingRepositoriums();
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

        private void CheckRepo(string repoDdir, string newRepo = default(string))
        {
            repoConfiguration = new RepoControlConfiguration(repoDdir, semaphore, newRepo);
            powerShell = new PowersShell();
            commandProcessor = new CommandProcessor.RepoCommandProcessor(powerShell);
            var repo = new RepoControl(repoConfiguration, commandProcessor, endControl);
            repo.Dock = DockStyle.Top;
            allReposControl.Add(repo);
            this.panel1.Controls.Add(repo);
            Application.DoEvents();
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lastTimeUpdate = DateTime.UtcNow;
            CheackAndRemovedNewRepos();
            allReposControl.ForEach(cntr => cntr.UpdateRepoInfo());
        }

        private void CheackAndRemovedNewRepos()
        {
            foreach (var ctrRepo in allReposControl.Where(repo => repo.IsNew && !ExpectedRemoteList.Contains(repo.RepoConfiguration)))
                ctrRepo.Dispose();
        }

        private bool NotInRepoConfig(string config) => !RepoRemoteList.Any(r => r?.ToLower() == config?.ToLower()) 
            && !allReposControl.Any(ctr => ctr.RepoConfiguration?.ToLower() == config?.ToLower());
        private void AddMissingRepositoriums()
        {
            ExpectedRemoteList.Where(NotInRepoConfig).ToList()
                .ForEach(config =>
                {
                    CheckRepo(chosenPath, config);
                    Application.DoEvents();
                    Application.DoEvents();
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
            var commandProc = new CommandProcessor.RepoCommandProcessor(powerShell);

            await CloneNewRepo(commandProc);
            toolStripMenuItem2.Visible = false;
        }

        private async Task CloneNewRepo(CommandProcessor.RepoCommandProcessor commandProc)
        {
            List<Task> runningClons = new List<Task>();
            try
            {
                await WaitLeaveOne();
                allReposControl.Where(ctr => ctr.IsNew).ToList()
                    .ForEach(ctr => runningClons.Add(Task.Run(() => CloneRepoProcess(commandProc, ctr))));

                Task.Run(() => UpdateCloneRepos(runningClons));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCloneRepos(List<Task> runningClons)
        {
            while (!runningClons.All(t => t.IsCompleted))
                Thread.Sleep(100);

            ReleaceAll();
            CheckToolStripMenuItem_Click(null, null);
        }

        private async Task WaitLeaveOne()
        {
            while (semaphore.CurrentCount != 1)
                await semaphore.WaitAsync();
        }

        private void ReleaceAll() 
        {
            while (semaphore.CurrentCount != semaphore.MaxRepoProcessingCount) 
                semaphore.Release(); 
        }

        private void CloneRepoProcess(RepoCommandProcessor commandProc, RepoControl ctr)
        {            
            try
            {
                semaphore.Wait();
                ctr.Invoke(new Action(() => ctr.HighlightLabel()), null);
                var result = commandProc.ClonRepo(chosenPath, ctr.RepoConfiguration);
                var repoPath = $@"{chosenPath}\{ctr.GetNewRepoName}";
                if (Directory.Exists(repoPath))
                {
                    this.Invoke(new Action(() =>
                    {
                        CheckRepo(repoPath);
                        ctr.Dispose();
                    }), null);
                }
                else
                {
                    this.Invoke(new Action(() => RemoveUnUsed(ctr)), null);
                }
            }
            catch(Exception) { }
            finally
            {
                semaphore.Release();
            }
        }

        private void RemoveUnUsed(RepoControl ctr)
        {
            allReposControl.Remove(ctr);
            ctr.Dispose();
            ExpectedRemoteList.Remove(ctr.RepoConfiguration);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            semaphore.Dispose();
            timer1.Dispose();
        }

        private void SetMenueCheckerValue()
        {
            switch (intervalUpdateCheckHour)
            {
                case 1:
                    toolStripComboBox1.SelectedIndex = 1;
                    break;
                case 2:
                    toolStripComboBox1.SelectedIndex = 2;
                    break;
                case 3:
                    toolStripComboBox1.SelectedIndex = 3;
                    break;
                case 4:
                    toolStripComboBox1.SelectedIndex = 4;
                    break;
                default:
                    toolStripComboBox1.SelectedIndex = 0;
                    break;
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (!isLoaded) return;
            switch(toolStripComboBox1.SelectedItem)
            {
                case "1 hour":
                    intervalUpdateCheckHour = 1;
                    break;
                case "2 hours":
                    intervalUpdateCheckHour = 2;
                    break;
                case "3 hours":
                    intervalUpdateCheckHour = 3;
                    break;
                case "4 hours":
                    intervalUpdateCheckHour = 4;
                    break;
                default:
                    intervalUpdateCheckHour = 0;
                    break;
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("intervalUpdateCheckHour");
            config.AppSettings.Settings.Add("intervalUpdateCheckHour", intervalUpdateCheckHour.ToString());
            config.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (intervalUpdateCheckHour == 0) return;

            if (lastTimeUpdate.AddHours(intervalUpdateCheckHour) < DateTime.UtcNow)
                CheckToolStripMenuItem_Click(null, null);
        }
    }
}
