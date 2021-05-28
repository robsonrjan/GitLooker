using GitLooker.Controls;
using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using GitLooker.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class MainForm : Form
    {
        private const string ThumbnailButtonTextPrefix = "Check status for ";
        private const int MillisecondsDelayToLetAllUpdatesToFinish = 500;
        private readonly IAppConfiguration appConfiguration;
        private readonly IAppSemaphoreSlim semaphore;
        private readonly ITabsRepoBuilder tabsRepoBuilder;
        private readonly IServiceProvider serviceProvider;
        private volatile bool isUpdating;
        private TabReposControl nextState;
        private bool noReposLoaded;
        private readonly List<ThumbnailToolBarButton> buttonForTaskList;

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
            buttonForTaskList = new List<ThumbnailToolBarButton>();
        }

        private void SetWorkingPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allRepos = appConfiguration.Select(c => c.GitLookerPath).ToList();
            var repoSources = new RepoSources(folderBrowserDialog1, allRepos);

            repoSources.ShowDialog();
            var newRepoList = repoSources.RepoList ?? Enumerable.Empty<string>();

            if (!newRepoList.Any()) return;

            if (noReposLoaded)
                SetMenuFunctionIfNoRepos(false);

            RemoveRepoTab(GetRepoTabs().Where(r => !newRepoList.Contains(r.RepoConfiguration.GitLookerPath)));

            foreach (var newRepo in newRepoList.Where(r => !GetRepoTabs().Any(t => t.RepoConfiguration.GitLookerPath == r)))
                tabsRepoBuilder.BuildTab(reposCatalogs, Repo_OnSelectRepo, newRepo);

            appConfiguration.Save();
            SetCurrentTab();
            GenerateAndUpdateRepos();
        }

        private void RemoveRepoTab(IEnumerable<TabReposControl> removedRepos)
        {
            foreach (var tab in removedRepos)
            {
                appConfiguration.Remove(tab.RepoConfiguration);
                reposCatalogs.TabPages.Remove(tab);
                tab.Dispose();
            }
        }

        private IEnumerable<TabReposControl> GetRepoTabs()
        {
            foreach (var tab in reposCatalogs.TabPages)
            {
                if (tab is TabReposControl repoTab)
                    yield return repoTab;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (appConfiguration.Any())
            {
                tabsRepoBuilder.BuildTabs(reposCatalogs, Repo_OnSelectRepo);
                reposCatalogs.Selected += ReposCatalogs_TabIndexChanged;
                SetCurrentTab();
                GenerateAndUpdateRepos();
                ThumbnailToolShow();
            }
            else
            {
                SetMenuFunctionIfNoRepos(true);
                MessageBox.Show("No repositories configured.");
            }

            Text = $"Git branch changes looker    ver.{AppVersion.AssemblyVersion}";
            notifyIcon1.Text = Text;
        }

        private void ThumbnailToolShow()
        {
            foreach (var tab in GetRepoTabs())
            {
                var itemTab = tab as TabReposControl;
                var buttonForTask = new ThumbnailToolBarButton(Properties.Resources.Flag_green, $"{ThumbnailButtonTextPrefix}{itemTab.Text}");
                buttonForTask.Click += UptateToolBars;
                buttonForTask.Visible = true;
                buttonForTaskList.Add(buttonForTask);
            }

            if (buttonForTaskList.Any())
                TaskbarManager.Instance.ThumbnailToolBars.AddButtons(Handle, buttonForTaskList.ToArray());
        }

        private void UptateToolBars(object sender, ThumbnailButtonClickedEventArgs e)
        {
            if (sender is ThumbnailToolBarButton barButton)
            {
                var repoTabName = barButton.Tooltip.Replace(ThumbnailButtonTextPrefix, "");
                var tab = GetRepoTabs().FirstOrDefault(t => t.Text == repoTabName);
                if (tab != default)
                    foreach (var ctr in tab)
                        ctr.UpdateRepoInfo();
            }
        }

        private void SetMenuFunctionIfNoRepos(bool disable)
        {
            noReposLoaded = disable;

            foreach (dynamic item in menuStrip1.Items)
                item.Enabled = disable ? (item.Text == "File") || (item.Text == "Configuration") : !disable;

            foreach (dynamic item in fileToolStripMenuItem.DropDownItems)
                item.Visible = disable ? (item.Name == "setWorkingPathToolStripMenuItem") : !disable;

            foreach (dynamic item in configurationToolStripMenuItem.DropDownItems)
                item.Visible = disable ? (item.Name == "importConfigurationToolStripMenuItem") : !disable;
        }

        private void ReposCatalogs_TabIndexChanged(object sender, EventArgs e) => SetCurrentTab();

        private void SetCurrentTab()
        {
            if (isUpdating)
            {
                nextState = reposCatalogs.SelectedTab as TabReposControl;
                return;
            }

            currentTabControl = reposCatalogs.SelectedTab as TabReposControl;

            if (currentTabControl == default)
                return;

            SetState();
        }

        private void SetState()
        {
            toolTip1.SetToolTip(reposCatalogs, currentTabControl.RepoConfiguration.GitLookerPath);

            SetMenueCheckerValue();
            toolStripTextBox1.Text = currentTabControl.RepoConfiguration.MainBranch;
            toolStripTextBox2.Text = currentTabControl.RepoConfiguration.Command;
            toolStripTextBox3.Text = currentTabControl.RepoConfiguration.Arguments;
            toolStripTextBox5.Text = currentTabControl.RepoConfiguration.ProjectCommand;
            toolStripTextBox6.Text = currentTabControl.RepoConfiguration.ProjectArguments;
            toolStripTextBox7.Text = currentTabControl.RepoConfiguration.ProjectExtension;
        }

        private void SemaphoreIsUsed(bool isProccesing)
            => Invoke(new Action(() =>
            {
                if (!isProccesing)
                {
                    currentTabControl.RepoEndControl.SendToBack();
                    currentTabControl.RepoEndControl.Select();
                    AddMissingRepositoriums(currentTabControl);
                    if (nextState != default)
                    {
                        currentTabControl = nextState;
                        nextState = default;
                        SetState();
                    }
                    CheckStatusAsync();
                }
                else if (!isUpdating)
                {
                    toolStripMenuItem2.Enabled = false;
                    checkToolStripMenuItem.Enabled = false;
                    toolStripMenuItem1.Visible = true;
                    isUpdating = true;
                }
            }), default);

        private async Task CheckStatusAsync()
        {
            await Task.Delay(MillisecondsDelayToLetAllUpdatesToFinish);
            UpdateTimeInfo();
            if (currentTabControl.ReposAllControl.Any(c => c.IsNeededUpdate))
                notifyIcon1.ShowBalloonTip(3000);
            isUpdating = false;
            toolStripMenuItem2.Enabled = true;
            checkToolStripMenuItem.Enabled = true;
            toolStripMenuItem1.Visible = false;
            UpdateTabButtonIcon();
            await Task.CompletedTask;
        }

        private void UpdateTabButtonIcon()
        {
            foreach (var tab in GetRepoTabs())
            {
                if (tab.Any(ctr => ctr.IsConnectionError))
                {
                    iconUpdate(tab.Text, Properties.Resources.Flag_red);
                    return;
                }

                if (tab.Any(ctr => ctr.IsNeededUpdate))
                {
                    iconUpdate(tab.Text, Properties.Resources.Flag_blue);
                    return;
                }

                iconUpdate(tab.Text, Properties.Resources.Flag_green);
            }

            void iconUpdate(string repoName, Icon icon)
            {
                var buuton = buttonForTaskList.FirstOrDefault(tab => tab.Tooltip.Replace(ThumbnailButtonTextPrefix, "") == repoName);
                if (buuton != default)
                    buuton.Icon = icon;
            }
        }

        private void GenerateAndUpdateRepos()
        {
            if (currentTabControl == default) return;
            UpdateAll();
            currentTabControl.RepoIsLoaded = true;
        }

        private void Repo_OnSelectRepo(RepoControl control)
        {
            currentRepo = control;
            currentRepo.ContextMenuStrip = contextMenuStrip1;
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e) => UpdateAll();

        private void UpdateAll(IEnumerable<string> cloneRepos = default)
        {
            CheckForRemovedRepos();

            if (!currentTabControl.Any())
                AddMissingRepositoriums(currentTabControl);
            else
                foreach (var cntr in currentTabControl.Where(r => cloneRepos?.Contains(r.RepoPath) ?? true).OrderByDescending(c => c.Parent.Controls.GetChildIndex(c)))
                    cntr.UpdateRepoInfo();
        }

        private void CheckForRemovedRepos()
        {
            var repoToRemove = currentTabControl.Where(r => r.IsNew && !currentTabControl.RepoConfiguration.ExpectedRemoteRepos.Contains(r.RepoConfiguration))
            .ToList();

            repoToRemove.AddRange(currentTabControl.Where(r => !Directory.Exists(r.RepoPath)));

            foreach (var repo in repoToRemove)
                currentTabControl.RepoRemove(repo);

            toolStripMenuItem2.Visible = currentTabControl.Any(r => r.IsNew);
        }

        private void UpdateTimeInfo()
        {
            currentTabControl.RepoLastTimeUpdate = DateTime.UtcNow;
            toolStripMenuItem4.Text = $"Updated: {currentTabControl.RepoLastTimeUpdate.ToLocalTime().ToString("HH:mm dddd")}";
        }

        private bool NotInRepoConfig(string config, TabReposControl currentTab) => !currentTab.Any(ctr => ctr.RepoConfiguration?.Equals(config, StringComparison.InvariantCultureIgnoreCase) == true);
        private void AddMissingRepositoriums(TabReposControl currentTab) //
        {
            Task.Run(() =>
            {
                Task.Delay(MillisecondsDelayToLetAllUpdatesToFinish).GetAwaiter().GetResult();
                Invoke(new Action(() =>
                {
                    currentTab.RepoConfiguration.ExpectedRemoteRepos.Where(repo => NotInRepoConfig(repo, currentTab))
                        .ToList()
                        .ForEach(config =>
                        {
                            tabsRepoBuilder.BuildRepo(Repo_OnSelectRepo, currentTabControl, currentTabControl.RepoConfiguration.GitLookerPath, config);
                            Application.DoEvents();
                            Application.DoEvents();
                            toolStripMenuItem2.Visible = true;
                        });
                }), default);
            });
        }

        private void remoteReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = currentTabControl.ReposAllControl
                .Where(r => !string.IsNullOrWhiteSpace(r.RepoConfiguration))
                .Select(r => r.RepoConfiguration).ToArray();
            repoList.ShowDialog();
        }

        private string ToLower(string text) => text.ToLowerInvariant();

        private void expectedReposConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepoList repoList = new RepoList();
            repoList.repoText.Lines = currentTabControl.RepoConfiguration.ExpectedRemoteRepos.ToArray();
            repoList.ShowDialog();

            currentTabControl.RepoConfiguration.ExpectedRemoteRepos = repoList.repoText.Lines.Select(ToLower).Distinct().Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            appConfiguration.Save();

            CheckForRemovedRepos();
            if (currentTabControl.Any() && currentTabControl.ReposAllControl.Any(r => !string.IsNullOrWhiteSpace(r.RepoConfiguration)))
                AddMissingRepositoriums(currentTabControl);
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var commandProc = serviceProvider.GetService<IRepoCommandProcessor>();

            await CloneNewRepoAsync(commandProc);
            toolStripMenuItem2.Visible = false;
        }

        private async Task CloneNewRepoAsync(IRepoCommandProcessor commandProc)
        {
            List<Task<string>> runningClons = new List<Task<string>>();
            try
            {
                await WaitLeaveOneAsync();
                var toCloneRepos = currentTabControl.Where(ctr => ctr.IsNew).ToList();
                foreach (var ctr in toCloneRepos)
                    runningClons.Add(Task.Run(() => CloneRepoProcessAsync(commandProc, ctr)));

                Task.Run(() => UpdateCloneReposAsync(runningClons));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateCloneReposAsync(List<Task<string>> runningClons)
        {
            var result = await Task.WhenAll(runningClons);
            ReleaceAll();
            UpdateAll(result);
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

        private async Task<string> CloneRepoProcessAsync(IRepoCommandProcessor commandProc, RepoControl ctr)
        {
            string clonedRepos = default;
            try
            {
                await semaphore.WaitAsync();
                ctr.Invoke(new Action(() => ctr.HighlightLabel()), default);
                var result = commandProc.ClonRepo(currentTabControl.RepoConfiguration.GitLookerPath, ctr.RepoConfiguration);
                var repoPath = $@"{currentTabControl.RepoConfiguration.GitLookerPath}\{ctr.GetNewRepoName}";
                if (Directory.Exists(repoPath))
                {
                    Invoke(new Action(() =>
                    {
                        currentTabControl.RepoRemove(ctr);
                        Application.DoEvents();
                        tabsRepoBuilder.BuildRepo(Repo_OnSelectRepo, currentTabControl, (clonedRepos = repoPath));
                        Application.DoEvents();
                    }), default);
                }
                else
                {
                    Invoke(new Action(() => RemoveUnUsed(ctr)), default);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                semaphore.Release();
            }

            return clonedRepos;
        }

        private void RemoveUnUsed(RepoControl ctr) => currentTabControl.RepoRemove(ctr);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            semaphore.Dispose();
            timer1.Dispose();
        }

        private void SetMenueCheckerValue()
        {
            toolStripComboBox1.SelectedIndex = currentTabControl.RepoConfiguration.IntervalUpdateCheckHour switch
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
            currentTabControl.RepoConfiguration.IntervalUpdateCheckHour = toolStripComboBox1.SelectedItem switch
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
            if (isUpdating || (currentTabControl == default)) return;

            foreach (var tab in reposCatalogs.TabPages)
            {
                var itemTab = tab as TabReposControl;
                if (itemTab.RepoConfiguration.IntervalUpdateCheckHour > 0)
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
                if (currentTabControl.RepoConfiguration.MainBranch == toolStripTextBox1.Text) return;
                currentTabControl.RepoConfiguration.MainBranch = toolStripTextBox1.Text;
                fileToolStripMenuItem.HideDropDown();

                appConfiguration.Save();
                fileToolStripMenuItem.HideDropDown();

                GenerateAndUpdateRepos();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (currentRepo != default)
                Process.Start("explorer", currentRepo.RepoPath);
        }

        private void SaveConfiguration(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fileToolStripMenuItem.HideDropDown();

                currentTabControl.RepoConfiguration.Command = toolStripTextBox2.Text;
                currentTabControl.RepoConfiguration.Arguments = toolStripTextBox3.Text;
                currentTabControl.RepoConfiguration.ProjectCommand = toolStripTextBox5.Text;
                currentTabControl.RepoConfiguration.ProjectArguments = toolStripTextBox6.Text;
                currentTabControl.RepoConfiguration.ProjectExtension = toolStripTextBox7.Text;
                appConfiguration.Save();
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(toolStripTextBox2.Text) && (currentRepo != default))
                    Process.Start(toolStripTextBox2.Text, $@"{PrepareArgument(toolStripTextBox3.Text)}""{currentRepo.RepoPath}""");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
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

            if (toolStripMenuItem14.Visible = !string.IsNullOrWhiteSpace(toolStripTextBox5.Text) || !string.IsNullOrWhiteSpace(toolStripTextBox7.Text))
                toolStripMenuItem14.Text = $"Manage project [Ctrl+F]";

            if (string.IsNullOrWhiteSpace(currentTabControl.RepoConfiguration.MainBranch) || (currentRepo == default) || currentRepo.IsMainBranch)
                checkOnToolStripMenuItem.Visible = false;
            else
            {
                checkOnToolStripMenuItem.Visible = true;
                checkOnToolStripMenuItem.Text = $"Check on: {currentTabControl.RepoConfiguration.MainBranch}";
            }
        }

        private void toolStripTextBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var filterRepo = currentTabControl.Where(r => r.RepoName.ToLowerInvariant().Contains(toolStripTextBox4.Text.ToLowerInvariant())).OrderByDescending(r => r.RepoName);
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
                currentRepo.CheckOutBranch(currentTabControl.RepoConfiguration.MainBranch);
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

                foreach (dynamic tab in reposCatalogs.TabPages)
                    tab.Dispose();
                reposCatalogs.TabPages.Clear();

                Form1_Load(default, default);
            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
                toolStripTextBox2.Text = GetFileName("Choose executable file to memage selected repo", toolStripTextBox2.Text);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
            => WindowState = FormWindowState.Normal;

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
                toolStripTextBox5.Text = GetFileName("Choose executable file to memage selected project", toolStripTextBox5.Text);
        }

        private string GetFileName(string titleText, string fileName)
        {
            string result = fileName;
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
                if (!string.IsNullOrWhiteSpace(extension))
                    currentTabControl.RepoHolder.FindRepoProjectFilesAsync(currentRepo.RepoPath, extension)
                        .ContinueWith(
                            task =>
                            {
                                if (task.IsCompleted)
                                    ExecuteProjectManager(toolStripTextBox5.Text, true);
                                else if (task.Exception != default)
                                    MessageBox.Show(task.Exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });
                else if (!string.IsNullOrWhiteSpace(toolStripTextBox5.Text))
                    ExecuteProjectManager(toolStripTextBox5.Text, false);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }

            return Task.CompletedTask;
        }

        private void ExecuteProjectManager(string path, bool isExtension)
        {
            string mainProjectFile = default;

            if (currentRepo == default)
                return;

            try
            {
                var projectFiles = isExtension ? currentTabControl.RepoHolder.GetProjectFiles(currentRepo.RepoPath) : Enumerable.Empty<string>();
                if (isExtension && (projectFiles?.Any() ?? false))
                {
                    if (projectFiles.Count() > 1)
                    {
                        var pickForm = new RepoSources(projectFiles.ToList());
                        pickForm.ShowDialog();
                        mainProjectFile = pickForm.ChosenSolution;
                        if (string.IsNullOrWhiteSpace(mainProjectFile))
                            mainProjectFile = projectFiles.First();
                    }
                    else
                        mainProjectFile = projectFiles.First();

                    if (!string.IsNullOrWhiteSpace(path))
                        Process.Start(path, $@"{PrepareArgument(toolStripTextBox6.Text)}""{mainProjectFile}""");
                    else
                        Process.Start($@"""{mainProjectFile}""");
                }
                else if (!isExtension && !string.IsNullOrWhiteSpace(path))
                    Process.Start(path, $@"{PrepareArgument(toolStripTextBox6.Text)}""{currentRepo.RepoPath}""");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string PrepareArgument(string argument)
            => (string.IsNullOrWhiteSpace(argument) ? string.Empty : $"{argument} ");

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
            => StartProcess("https://github.com/robsonrjan/GitLooker");

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
            => StartProcess("https://github.com/robsonrjan/GitLooker/blob/master/LICENSE");

        private void StartProcess(string processInfo)
        {
            Process wwwProcess = new Process();
            wwwProcess.StartInfo = new ProcessStartInfo(processInfo);
            wwwProcess.Start();
        }

        private void reportAProblemToolStripMenuItem_Click(object sender, EventArgs e)
            => StartProcess("https://github.com/robsonrjan/GitLooker/issues");

        private void editConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
            => StartProcess(AppConfiguration.Location);
    }
}
