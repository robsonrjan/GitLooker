using GitLooker.Core;
using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using GitLooker.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class MainForm : Form, IMainForm
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IAppSemaphoreSlim semaphore;
        private readonly IServiceProvider serviceProvider;
        private readonly IRepoHolder repoHolder;
        private readonly IGitFileRepo gitFileRepo;

        private string chosenPath = string.Empty;
        private List<RepoControl> allReposControl;
        private int intervalUpdateCheckHour;
        private DateTime lastTimeUpdate;
        private bool isLoaded;
        private string mainBranch = "master";
        private RepoControl currentRepo;
        private bool updatedManual;

        public Panel EndControl => endControl;
        public string CurrentRepoDdir { get; private set; }
        public string CurrentNewRepo { get; private set; }

        public MainForm(IServiceProvider serviceProvider, IAppSemaphoreSlim appSemaphoreSlim,
            IAppConfiguration appConfiguration, IRepoHolder repoHolder, IGitFileRepo gitFileRepo)
        {
            InitializeComponent();
            this.repoHolder = repoHolder;
            allReposControl = new List<RepoControl>();
            lastTimeUpdate = DateTime.UtcNow;
            semaphore = appSemaphoreSlim;
            semaphore.OnUse += SemaphoreIsUsed;
            this.appConfiguration = appConfiguration;
            this.serviceProvider = serviceProvider;
            this.gitFileRepo = gitFileRepo;
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
                appConfiguration.GitLookerPath = chosenPath;
                appConfiguration.Save();
                GenerateAndUpdateRepos();
            }

        }

        private void Clear()
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(endControl);
            allReposControl.ForEach(r => r.Dispose());
            allReposControl.Clear();
            repoHolder.RepoRemoteList.Clear();
            toolStripMenuItem2.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chosenPath = appConfiguration.GitLookerPath;
            mainBranch = appConfiguration.MainBranch;
            intervalUpdateCheckHour = appConfiguration.IntervalUpdateCheckHour;
            toolStripTextBox2.Text = appConfiguration.Command;
            toolStripTextBox3.Text = appConfiguration.Arguments;
            SetMenueCheckerValue();
            ReadRepositoriumConfiguration();

            if (!string.IsNullOrEmpty(chosenPath))
                GenerateAndUpdateRepos();

            this.Text = $"Git branch changes looker    ver.{AppVersion.AssemblyVersion}";
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
                    endControl.Select();
                    AddMissingRepositoriums();
                    if (allReposControl.Any(c => c.IsNeededUpdate))
                        ShowCheckNotification();
                }
            }), null);

        private void ShowCheckNotification()
        {
            if (intervalUpdateCheckHour == 0) return;
            if (updatedManual) notifyIcon1.ShowBalloonTip(3000);
            updatedManual = false;
        }

        private void ReadRepositoriumConfiguration() => repoHolder.ExpectedRemoteList = appConfiguration.ExpectedRemoteRepos;


        private void GenerateAndUpdateRepos()
        {
            if (Directory.Exists(chosenPath)) CheckForGitRepo(chosenPath);
            CheckToolStripMenuItem_Click(null, null);

            if (!allReposControl.Any() && !string.IsNullOrEmpty(chosenPath))
                AddMissingRepositoriums();
        }

        private void CheckForGitRepo(string chosenPath)
        {
            foreach (var repo in gitFileRepo.Get(chosenPath))
                CheckRepo(repo);
        }

        private void CheckRepo(string repoDdir, string newRepo = default)
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
            updatedManual = true;
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
            foreach (var ctrRepo in allReposControl.Where(repo => repo.IsNew && !repoHolder.ExpectedRemoteList.Contains(repo.RepoConfiguration)))
                ctrRepo.Dispose();
        }

        private bool NotInRepoConfig(string config) => !repoHolder.RepoRemoteList.Any(r => r?.ToLower() == config?.ToLower())
            && !allReposControl.Any(ctr => ctr.RepoConfiguration?.ToLower() == config?.ToLower());
        private void AddMissingRepositoriums()
        {
            repoHolder.ExpectedRemoteList.Where(NotInRepoConfig).ToList()
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
            repoList.repoText.Lines = repoHolder.RepoRemoteList.ToArray();
            repoList.ShowDialog();
        }

        private string ToLower(string text) => text.ToLower();

        private void expectedReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = repoHolder.ExpectedRemoteList.ToArray();
            repoList.ShowDialog();

            repoHolder.ExpectedRemoteList = repoList.repoText.Lines.Select(ToLower).Distinct().ToList();
            appConfiguration.ExpectedRemoteRepos = repoHolder.ExpectedRemoteList;
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
            repoHolder.ExpectedRemoteList.Remove(ctr.RepoConfiguration);
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
            appConfiguration.IntervalUpdateCheckHour = intervalUpdateCheckHour;
            appConfiguration.Save();

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

                appConfiguration.MainBranch = mainBranch;
                appConfiguration.Save();

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

                appConfiguration.Command = toolStripTextBox2.Text;
                appConfiguration.Arguments = toolStripTextBox3.Text;
                appConfiguration.Save();
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

            if (string.IsNullOrWhiteSpace(mainBranch) || (currentRepo == default) || currentRepo.IsMainBranch)
                checkOnToolStripMenuItem.Visible = false;
            else
            {
                checkOnToolStripMenuItem.Visible = true;
                checkOnToolStripMenuItem.Text = $"Check on: {mainBranch}";
            }
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

        private void checkOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentRepo != default)
                currentRepo.CheckOutBranch(mainBranch);
        }

        private void pullAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cntr in allReposControl.Where(repo => repo.CanPull))
                cntr.PullRepo();
            updatedManual = true;
        }

        private void checkToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            pullAllToolStripMenuItem.Visible = allReposControl.Any(repo => repo.CanPull);
            checkWithConnectionErrorToolStripMenuItem.Visible = allReposControl.Any(repo => repo.IsConnectionError);
        }

        private void checkWithConnectionErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cntr in allReposControl.Where(repo => repo.IsConnectionError))
                cntr.UpdateRepoInfo();
            updatedManual = true;
        }

        private void exortConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "json files (*.json)|*.json",
                FileName = "GitLookerConfig.json"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                appConfiguration.SaveAs(saveFileDialog.FileName);
        }

        private void importConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "json files (*.json)|*.json",
                FileName = "GitLookerConfig.json",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                appConfiguration.Open(openFileDialog.FileName);
                appConfiguration.Save();
                foreach (var ctr in allReposControl)
                    ctr.Dispose();

                allReposControl.Clear();
                Form1_Load(default, default);
            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                openFileDialog1.Title = "Choose executable file to memage selected repo";
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = toolStripTextBox2.Text;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    toolStripTextBox2.Text = openFileDialog1.FileName;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
            => this.WindowState = FormWindowState.Normal;
    }
}
