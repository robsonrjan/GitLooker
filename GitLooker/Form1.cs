using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class Form1 : Form
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IAppSemaphoreSlim semaphore;
        private readonly IServiceProvider serviceProvider;

        private string chosenPath = string.Empty;
        private List<RepoControl> allReposControl;
        private int intervalUpdateCheckHour;
        private DateTime lastTimeUpdate;
        private bool isLoaded;
        private string mainBranch = "master";
        private RepoControl currentRepo;

        internal static List<string> RepoRemoteList;
        internal static List<string> ExpectedRemoteList;

        public Panel EndControl => endControl;
        public string CurrentRepoDdir { get; private set; }
        public string CurrentNewRepo { get; private set; }

        public Form1(IServiceProvider serviceProvider, IAppSemaphoreSlim appSemaphoreSlim, IAppConfiguration appConfiguration)
        {
            InitializeComponent();
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
            allReposControl = new List<RepoControl>();
            lastTimeUpdate = DateTime.UtcNow;
            semaphore = appSemaphoreSlim;
            semaphore.OnUse += SemaphoreIsUsed;
            this.appConfiguration = appConfiguration;
            this.serviceProvider = serviceProvider;
        }

        private void SetWorkingPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(chosenPath))
                folderBrowserDialog1.SelectedPath = chosenPath;

            folderBrowserDialog1.ShowDialog();
            var path = folderBrowserDialog1.SelectedPath;
            if (!string.IsNullOrEmpty(path) && (chosenPath != path))
            {
                Clear();
                chosenPath = path;
                var dataToSave = new Dictionary<string, object>();
                dataToSave.Add("GirLookerPath", chosenPath);
                appConfiguration.Save(dataToSave);
                GenerateAndUpdateRepos();
            }

        }

        private void Clear()
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(endControl);
            allReposControl.ForEach(r => r.Dispose());
            allReposControl.Clear();
            RepoRemoteList.Clear();
            toolStripMenuItem2.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chosenPath = appConfiguration.GirLookerPath;
            mainBranch = appConfiguration.MainBranch;
            intervalUpdateCheckHour = appConfiguration.IntervalUpdateCheckHour;
            toolStripTextBox2.Text = appConfiguration.Command;
            toolStripTextBox3.Text = appConfiguration.Arguments;
            SetMenueCheckerValue();
            ReadRepositoriumConfiguration();

            if (!string.IsNullOrEmpty(chosenPath))
                GenerateAndUpdateRepos();

            this.Text += $"    ver.{AppVersion.AssemblyVersion}";
            this.notifyIcon1.Text = this.Text;
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
                    ShowCheckNotification();
                }
            }), null);

        private void ShowCheckNotification()
        {
            if (intervalUpdateCheckHour == 0) return;
            notifyIcon1.ShowBalloonTip(3000);
        }

        private void ReadRepositoriumConfiguration() => ExpectedRemoteList = appConfiguration.ExpectedRemoteRepos;


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
            CurrentRepoDdir = repoDdir;
            CurrentNewRepo = newRepo;
            var repo = serviceProvider.GetService<RepoControl>();
            repo.OnSelectRepo += Repo_OnSelectRepo;
            repo.Dock = DockStyle.Top;
            allReposControl.Add(repo);
            this.panel1.Controls.Add(repo);
            Application.DoEvents();
        }

        private void Repo_OnSelectRepo(RepoControl control)
        {
            currentRepo = control;
            currentRepo.ContextMenuStrip = this.contextMenuStrip1;
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lastTimeUpdate = DateTime.UtcNow;
            toolStripMenuItem4.Text = $"Updated: {lastTimeUpdate.ToLocalTime().ToString("HH:mm dddd")}";
            CheackAndRemovedNewRepos();
            DeleteDisposedRepos();

            foreach (var cntr in allReposControl.OrderByDescending(c => c.Parent.Controls.GetChildIndex(c)))
                cntr.UpdateRepoInfo();
        }

        private void DeleteDisposedRepos()
        {
            foreach (var ctr in allReposControl.Where(c => c.Parent == null).ToList())
            {
                allReposControl.Remove(ctr);
                ctr.Dispose();
            }
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

            ExpectedRemoteList = repoList.repoText.Lines.Select(ToLower).Distinct().ToList();
            appConfiguration.ExpectedRemoteRepos = ExpectedRemoteList;
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var commandProc = serviceProvider.GetService<IRepoCommandProcessor>();

            await CloneNewRepoAsync(commandProc);
            toolStripMenuItem2.Visible = false;
        }

        private async Task CloneNewRepoAsync(IRepoCommandProcessor commandProc)
        {
            List<Task> runningClons = new List<Task>();
            try
            {
                await WaitLeaveOneAsync();
                allReposControl.Where(ctr => ctr.IsNew).ToList()
                    .ForEach(ctr => runningClons.Add(Task.Run(() => CloneRepoProcessAsync(commandProc, ctr))));

                Task.Run(() => UpdateCloneReposAsync(runningClons));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateCloneReposAsync(List<Task> runningClons)
        {
            await Task.WhenAll(runningClons);

            ReleaceAll();
            CheckToolStripMenuItem_Click(null, null);
        }

        private async Task WaitLeaveOneAsync()
        {
            while (semaphore.CurrentCount > 1)
                await semaphore.WaitAsync();
        }

        private void ReleaceAll()
        {
            while (semaphore.CurrentCount != semaphore.MaxRepoProcessingCount)
                semaphore.Release();
        }

        private async Task CloneRepoProcessAsync(IRepoCommandProcessor commandProc, RepoControl ctr)
        {
            try
            {
                await semaphore.WaitAsync();
                ctr.Invoke(new Action(() => ctr.HighlightLabel()), null);
                var result = commandProc.ClonRepo(chosenPath, ctr.RepoConfiguration);
                var repoPath = $@"{chosenPath}\{ctr.GetNewRepoName}";
                if (Directory.Exists(repoPath))
                {
                    this.Invoke(new Action(() =>
                    {
                        ctr.Dispose();                        
                        CheckRepo(repoPath);                        
                    }), null);
                }
                else
                {
                    this.Invoke(new Action(() => RemoveUnUsed(ctr)), null);
                }
            }
            catch (Exception) { }
            finally
            {
                semaphore.Release();
            }
            ctr.Dispose();
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
            switch (toolStripComboBox1.SelectedItem)
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
            var settingToSave = new Dictionary<string, object>();
            settingToSave.Add("intervalUpdateCheckHour", intervalUpdateCheckHour);
            appConfiguration.Save(settingToSave);

            fileToolStripMenuItem.HideDropDown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (intervalUpdateCheckHour == 0) return;

            if (lastTimeUpdate.AddHours(intervalUpdateCheckHour) < DateTime.UtcNow)
                CheckToolStripMenuItem_Click(null, null);
        }

        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (mainBranch == toolStripTextBox1.Text) return;
                mainBranch = toolStripTextBox1.Text;
                fileToolStripMenuItem.HideDropDown();

                var settingToSave = new Dictionary<string, object>();
                settingToSave.Add("mainBranch", mainBranch);
                appConfiguration.Save(settingToSave);

                fileToolStripMenuItem.HideDropDown();

                Clear();
                GenerateAndUpdateRepos();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (currentRepo != default)
                System.Diagnostics.Process.Start("explorer", currentRepo.RepoPath);
        }

        private void toolStripTextBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fileToolStripMenuItem.HideDropDown();

                var settingToSave = new Dictionary<string, object>();
                settingToSave.Add("command", toolStripTextBox2.Text);
                settingToSave.Add("arguments", toolStripTextBox3.Text);
                appConfiguration.Save(settingToSave);
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(toolStripTextBox2.Text))
                    System.Diagnostics.Process.Start(toolStripTextBox2.Text, $@"{toolStripTextBox3.Text} ""{currentRepo.RepoPath}""");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void updateStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentRepo != default)
                currentRepo.UpdateRepoInfo();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (toolStripMenuItem8.Visible = !string.IsNullOrWhiteSpace(toolStripTextBox2.Text))
                toolStripMenuItem8.Text = $"Execute {toolStripTextBox2.Text.Split('\\').Last()}";
        }

        private void toolStripTextBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var filterRepo = allReposControl.Where(r => r.RepoName.ToLower().Contains(toolStripTextBox4.Text.ToLower())).OrderByDescending(r => r.RepoName);
                foreach (var repo in filterRepo)
                    repo.SendToBack();
            }
        }

        private void toolStripTextBox4_Enter(object sender, EventArgs e)
        {
            if (toolStripTextBox4.Text == "repo filter")
                toolStripTextBox4.Text = "";
        }

        private void toolStripTextBox4_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toolStripTextBox4.Text))
                toolStripTextBox4.Text = "repo filter";
        }
    }
}
