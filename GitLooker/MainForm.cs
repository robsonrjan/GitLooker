using GitLooker.Controls;
using GitLooker.Core.Configuration;
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
    public partial class MainForm : Form
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IAppSemaphoreSlim semaphore;
        private readonly ITabsRepoBuilder tabsRepoBuilder;
        private readonly IServiceProvider serviceProvider;

        private RepoControl currentRepo;
        private TabReposControl currentTabControl;

        public Control EndControl { get; set; }
        public string CurrentRepoDdir { get; set; }
        public string CurrentNewRepo { get; set; }

        public MainForm(ITabsRepoBuilder tabsRepoBuilder,
            IAppSemaphoreSlim appSemaphoreSlim,
            IServiceProvider serviceProvider,
            IAppConfiguration appConfiguration)
        {
            InitializeComponent();
            semaphore = appSemaphoreSlim;
            semaphore.OnUse += SemaphoreIsUsed;
            this.appConfiguration = appConfiguration;
            this.tabsRepoBuilder = tabsRepoBuilder;
            this.serviceProvider = serviceProvider;
        }

        private void SetWorkingPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(appConfiguration.GitLookerPath))
                folderBrowserDialog1.SelectedPath = appConfiguration.GitLookerPath;

            folderBrowserDialog1.ShowDialog();
            var path = folderBrowserDialog1.SelectedPath;
            if (!string.IsNullOrEmpty(path) && (appConfiguration.GitLookerPath != path))
            {
                Clear();
                appConfiguration.GitLookerPath = path;
                appConfiguration.Save();
                GenerateAndUpdateRepos();
            }

        }

        private void Clear()
        {
            currentTabControl.RepoClearControls();
            toolStripMenuItem2.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabsRepoBuilder.BuildTabs(reposCatalogs, Repo_OnSelectRepo);
            reposCatalogs.TabIndexChanged += ReposCatalogs_TabIndexChanged;

            if (reposCatalogs.SelectedIndex != appConfiguration.CurrentIndex)
                reposCatalogs.SelectedIndex = appConfiguration.CurrentIndex;
            else
                ReposCatalogs_TabIndexChanged(default, default);

            GenerateAndUpdateRepos();



            SetMenueCheckerValue();
            ReadRepositoriumConfiguration();

            this.Text = $"Git branch changes looker    ver.{AppVersion.AssemblyVersion}";
            this.notifyIcon1.Text = this.Text;
        }

        private void ReposCatalogs_TabIndexChanged(object sender, EventArgs e)
        {
            currentTabControl = reposCatalogs.SelectedTab as TabReposControl;
            if (currentTabControl == default)
                throw new ArgumentNullException("Selected Tab cannot be null");




            appConfiguration.CurrentIndex = currentTabControl.RepoIndex;
            this.toolTip1.SetToolTip(this.reposCatalogs, appConfiguration.GitLookerPath);

            toolStripTextBox1.Text = appConfiguration.MainBranch;
            toolStripTextBox2.Text = appConfiguration.Command;
            toolStripTextBox3.Text = appConfiguration.Arguments;
            toolStripTextBox5.Text = appConfiguration.ProjectCommand;
            toolStripTextBox6.Text = appConfiguration.ProjectArguments;
            toolStripTextBox7.Text = appConfiguration.ProjectExtension;
        }

        private void SemaphoreIsUsed(bool isProccesing)
            => this.Invoke(new Action(() =>
            {
                toolStripMenuItem2.Enabled = checkToolStripMenuItem.Enabled = !(toolStripMenuItem1.Visible = isProccesing);
                if (!isProccesing)
                {
                    currentTabControl.RepoEndControl.SendToBack();
                    currentTabControl.RepoEndControl.Select();
                    AddMissingRepositoriums();
                    if (currentTabControl.ReposAllControl.Any(c => c.IsNeededUpdate))
                        notifyIcon1.ShowBalloonTip(3000);
                }
            }), null);

        private void ReadRepositoriumConfiguration() => currentTabControl.RepoHolder.ExpectedRemoteList = appConfiguration.ExpectedRemoteRepos;

        private void GenerateAndUpdateRepos()
        {




            CheckToolStripMenuItem_Click(null, null);

            if (!currentTabControl.ReposAllControl.Any() && !string.IsNullOrEmpty(appConfiguration.GitLookerPath))
                AddMissingRepositoriums();

            currentTabControl.RepoIsLoaded = true;
        }

        private void Repo_OnSelectRepo(RepoControl control)
        {
            currentRepo = control;
            currentRepo.ContextMenuStrip = this.contextMenuStrip1;
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTabControl.RepoLastTimeUpdate = DateTime.UtcNow;
            toolStripMenuItem4.Text = $"Updated: {currentTabControl.RepoLastTimeUpdate.ToLocalTime().ToString("HH:mm dddd")}";
            CheackAndRemovedNewRepos();
            DeleteDisposedRepos();
#if !DEBUG
            foreach (var cntr in currentTabControl.OrderByDescending(c => c.Parent.Controls.GetChildIndex(c)))
                cntr.UpdateRepoInfo();
#endif
        }

        private void DeleteDisposedRepos()
        {
            foreach (var ctr in currentTabControl.Where(c => c.Parent == default).ToList())
            {
                currentTabControl.ReposAllControl.Remove(ctr);
                ctr.Dispose();
            }
        }

        private void CheackAndRemovedNewRepos()
        {
            foreach (var ctrRepo in currentTabControl.Where(repo => repo.IsNew && !currentTabControl.RepoHolder.ExpectedRemoteList.Contains(repo.RepoConfiguration)))
                ctrRepo.Dispose();
        }

        private bool NotInRepoConfig(string config) => !currentTabControl.RepoHolder.RepoRemoteList.Any(r => r?.ToLower() == config?.ToLower())
            && !currentTabControl.Any(ctr => ctr.RepoConfiguration?.ToLower() == config?.ToLower());
        private void AddMissingRepositoriums()
        {
            currentTabControl.RepoHolder.ExpectedRemoteList.Where(NotInRepoConfig).ToList()
                .ForEach(config =>
                {
                    tabsRepoBuilder.BuildRepo(Repo_OnSelectRepo, currentTabControl, appConfiguration.GitLookerPath, config);
                    Application.DoEvents();
                    Application.DoEvents();
                    toolStripMenuItem2.Visible = true;
                });
        }

        private void remoteReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = currentTabControl.RepoHolder.RepoRemoteList.ToArray();
            repoList.ShowDialog();
        }

        private string ToLower(string text) => text.ToLower();

        private void expectedReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = currentTabControl.RepoHolder.ExpectedRemoteList.ToArray();
            repoList.ShowDialog();

            currentTabControl.RepoHolder.ExpectedRemoteList = repoList.repoText.Lines.Select(ToLower).Distinct().ToList();
            appConfiguration.ExpectedRemoteRepos = currentTabControl.RepoHolder.ExpectedRemoteList;
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
                currentTabControl.Where(ctr => ctr.IsNew).ToList()
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
                var result = commandProc.ClonRepo(appConfiguration.GitLookerPath, ctr.RepoConfiguration);
                var repoPath = $@"{appConfiguration.GitLookerPath}\{ctr.GetNewRepoName}";
                if (Directory.Exists(repoPath))
                {
                    this.Invoke(new Action(() =>
                    {
                        ctr.Dispose();
                        tabsRepoBuilder.BuildRepo(Repo_OnSelectRepo, currentTabControl, appConfiguration.GitLookerPath, repoPath);
                    }), null);
                }
                else
                {
                    this.Invoke(new Action(() => RemoveUnUsed(ctr)), null);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                semaphore.Release();
            }
            ctr.Dispose();
        }

        private void RemoveUnUsed(RepoControl ctr) => currentTabControl.RepoRemove(ctr);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            semaphore.Dispose();
            timer1.Dispose();
        }

        private void SetMenueCheckerValue()
        {
            toolStripComboBox1.SelectedIndex = appConfiguration.IntervalUpdateCheckHour switch
            {
                1 => 1,
                2 => 2,
                3 => 3,
                4 => 4,
                _ => 0
            };
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (!currentTabControl.RepoIsLoaded) return;
            appConfiguration.IntervalUpdateCheckHour = toolStripComboBox1.SelectedItem switch
            {
                "1 hour" => 1,
                "2 hours" => 2,
                "3 hours" => 3,
                "4 hours" => 4,
                _ => 0
            };
            appConfiguration.Save();
            fileToolStripMenuItem.HideDropDown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (appConfiguration.IntervalUpdateCheckHour > 0)
            {
                if (currentTabControl.RepoLastTimeUpdate.AddHours(appConfiguration.IntervalUpdateCheckHour) < DateTime.UtcNow)
                    CheckToolStripMenuItem_Click(null, null);
            }

            foreach (var tab in reposCatalogs.TabPages)
            {
                var itemTab = (TabReposControl)tab;

                if (!itemTab.Equals(currentTabControl) && (itemTab.RepoConfiguration.IntervalUpdateCheckHour > 0))
                {
                    if (itemTab.RepoLastTimeUpdate.AddHours(itemTab.RepoConfiguration.IntervalUpdateCheckHour) < DateTime.UtcNow)
                        foreach (var ctr in itemTab)
                            ctr.UpdateRepoInfo();
                }
            }
        }

        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (appConfiguration.MainBranch == toolStripTextBox1.Text) return;
                appConfiguration.MainBranch = toolStripTextBox1.Text;
                fileToolStripMenuItem.HideDropDown();

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

        private void SaveConfiguration(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fileToolStripMenuItem.HideDropDown();

                appConfiguration.Command = toolStripTextBox2.Text;
                appConfiguration.Arguments = toolStripTextBox3.Text;
                appConfiguration.ProjectCommand = toolStripTextBox5.Text;
                appConfiguration.ProjectArguments = toolStripTextBox6.Text;
                appConfiguration.ProjectExtension = toolStripTextBox7.Text;
                appConfiguration.Save();
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(toolStripTextBox2.Text) && (currentRepo != default))
                    System.Diagnostics.Process.Start(toolStripTextBox2.Text, $@"{PrepareArgument(toolStripTextBox3.Text)}""{currentRepo.RepoPath}""");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
            => ManageProjectFilesAsync(toolStripTextBox7.Text);

        private void updateStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentRepo != default)
                currentRepo.UpdateRepoInfo();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (toolStripMenuItem8.Visible = !string.IsNullOrWhiteSpace(toolStripTextBox2.Text))
                toolStripMenuItem8.Text = $"Manage repo [Ctrl+D]";

            if (toolStripMenuItem14.Visible = !string.IsNullOrWhiteSpace(toolStripTextBox7.Text))
                toolStripMenuItem14.Text = $"Manage project [Ctrl+F]";

            if (string.IsNullOrWhiteSpace(appConfiguration.MainBranch) || (currentRepo == default) || currentRepo.IsMainBranch)
                checkOnToolStripMenuItem.Visible = false;
            else
            {
                checkOnToolStripMenuItem.Visible = true;
                checkOnToolStripMenuItem.Text = $"Check on: {appConfiguration.MainBranch}";
            }
        }

        private void toolStripTextBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var filterRepo = currentTabControl.Where(r => r.RepoName.ToLower().Contains(toolStripTextBox4.Text.ToLower())).OrderByDescending(r => r.RepoName);
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
                currentRepo.CheckOutBranch(appConfiguration.MainBranch);
        }

        private void pullAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cntr in currentTabControl.Where(repo => repo.CanPull))
                cntr.PullRepo();
        }

        private void checkToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            pullAllToolStripMenuItem.Visible = currentTabControl.Any(repo => repo.CanPull);
            checkWithConnectionErrorToolStripMenuItem.Visible = currentTabControl.Any(repo => repo.IsConnectionError);
        }

        private void checkWithConnectionErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cntr in currentTabControl.Where(repo => repo.IsConnectionError))
                cntr.UpdateRepoInfo();
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
                foreach (var ctr in currentTabControl)
                    ctr.Dispose();

                currentTabControl.ReposAllControl.Clear();
                Form1_Load(default, default);
            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
                toolStripTextBox2.Text = GetFileName("Choose executable file to memage selected repo", toolStripTextBox2.Text);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
            => this.WindowState = FormWindowState.Normal;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                toolStripMenuItem8_Click(default, default);
                return default;
            }
            else if (keyData == (Keys.Shift | Keys.D))
            {
                updateStatusToolStripMenuItem_Click(default, default);
                return default;
            }
            else if (keyData == (Keys.Control | Keys.F))
            {
                toolStripMenuItem14_Click(default, default);
                return default;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void toolStripTextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
                toolStripTextBox2.Text = GetFileName("Choose executable file to memage selected project", toolStripTextBox5.Text);
        }

        private string GetFileName(string titleText, string fileName)
        {
            string result = default;
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            openFileDialog1.Title = titleText;
            openFileDialog1.Multiselect = false;
            openFileDialog1.FileName = fileName;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                result = openFileDialog1.FileName;
            return result;
        }

        private Task ManageProjectFilesAsync(string extension)
        {
            try
            {
                currentTabControl.RepoHolder.FindRepoProjectFilesAsync(currentRepo.RepoPath, extension)
                    .ContinueWith(
                        task =>
                        {
                            if (task.IsCompleted)
                                ExecuteProjectManager(toolStripTextBox5.Text);
                            else if (task.Exception != default)
                                MessageBox.Show(task.Exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }

            return Task.CompletedTask;
        }

        private void ExecuteProjectManager(string path)
        {
            string mainProjectFile = default;
            try
            {
                var projectFiles = currentTabControl.RepoHolder.GetProjectFiles(currentRepo.RepoPath);
                if (projectFiles?.Any() ?? false)
                {
                    if (projectFiles.Count() > 1)
                    {
                        // display user to choose one
                    }
                    mainProjectFile = projectFiles.First();

                    if (currentRepo != default)
                    {
                        if (!string.IsNullOrWhiteSpace(path))
                            System.Diagnostics.Process.Start(path, $@"{PrepareArgument(toolStripTextBox6.Text)}""{mainProjectFile}""");
                        else
                            System.Diagnostics.Process.Start($@"""{mainProjectFile}""");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string PrepareArgument(string argument)
            => (string.IsNullOrWhiteSpace(argument) ? string.Empty : $"{argument} ");
    }
}
