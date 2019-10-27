using GitLooker.CommandProcessor;
using GitLooker.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class RepoControl : UserControl
    {
        private readonly string repoPath;
        private readonly DirectoryInfo workingDir;
        private readonly ICommandProcessor commandProcessor;
        private readonly SemaphoreSlim semaphore;
        private string currentRespond;
        private string branchOn = "Pull current branch";
        private string newRepoConfiguration;
        private bool canReset;
        private bool isConfigured;
        private string newRepoName = default(string);

        internal bool IsNew { get; private set; }
        internal string RepoConfiguration { get; private set; }

        public RepoControl(IRepoControlConfiguration repoConfiguration, ICommandProcessor commandProcessor)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoConfiguration.RepoPath;
            this.commandProcessor = commandProcessor;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            this.semaphore = repoConfiguration.Semaphore;
            newRepoConfiguration = repoConfiguration.NewRepo;
            IsNew = !string.IsNullOrEmpty(newRepoConfiguration);
            if (IsNew)
                ConfigureAsToClone();
        }

        public string GetNewRepoName
        {
            get
            {
                if (newRepoName == default(string))
                {
                    var parts = newRepoConfiguration.Split('/');
                    newRepoName = parts[parts.Length - 1].Replace(".git", "");
                }
                return newRepoName;
            }
        }

        private void ConfigureAsToClone()
        {
            this.button2.BackgroundImage = global::GitLooker.Properties.Resources.cancel;
            this.button1.BackgroundImage = global::GitLooker.Properties.Resources.cancel;
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.toolTip1.SetToolTip(this.button1, "repo to clone");
            this.label1.Text = GetNewRepoName;
            RepoConfiguration = newRepoConfiguration;
        }

        public void UpdateRepoInfo()
        {
            if (IsNew) return;

            Task.Factory.StartNew(() =>
            {
                semaphore.Wait();
                try
                {
                    canReset = false;
                    this.Invoke(new Action(() => { this.label1.ForeColor = Color.DarkGreen; }), null);
                    GetRepoConfiguraion();

                    var returnValue = commandProcessor.CheckRepo(workingDir.FullName);
                    if (CheckIfExist(returnValue))
                    {
                        CheckCurrentBranch(returnValue);
                        CheckStatus(returnValue);
                    }

                    currentRespond += string.Join(Environment.NewLine, returnValue.ToArray());
                    this.Invoke(new Action(() => { this.label1.ForeColor = Color.Navy; }), null);
                }
                catch (Exception err)
                {
                    currentRespond = err.Message;
                }
                semaphore.Release();
            });
        }

        private void GetRepoConfiguraion()
        {
            if (!isConfigured)
            {
                var repoConfig = commandProcessor.RemoteConfig(workingDir.FullName);
                if (!string.IsNullOrEmpty(repoConfig))
                    Form1.RepoRemoteList.Add((RepoConfiguration = repoConfig.ToLower()));
                isConfigured = true;
            }
        }

        private bool CheckIfExist(IEnumerable<string> responseValue)
        {
            bool returnValue = true;
            if (responseValue.Any(respound => respound.ToLower().Contains("repository not found") ||
            respound.ToLower().Contains("does not exist or you do not have permissions") ||
            respound.ToLower().Contains("fatal: repository")))
            {
                returnValue = false;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
                    this.button1.Enabled = false;
                    this.SendToBack();
                }), null);
            }
            return returnValue;
        }

        private void CheckStatus(IEnumerable<string> returnValue)
        {
            if (returnValue.Any(rtn => rtn.Contains("branch is behind")))
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.SendToBack();
                }), null);
            }
            else if (returnValue.Any(rtn => rtn.Contains("git push") || rtn.Contains("git add") || rtn.Contains("git checkout ")))
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.move_task_up;
                    this.button1.Enabled = false;
                }));
                canReset = true && branchOn.EndsWith("master");
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.button_ok;
                    this.BringToFront();
                }), null);
            }
        }

        private void CheckCurrentBranch(IEnumerable<string> returnValue)
        {
            if (returnValue.Any(rtn => rtn.StartsWith("on branch")))
            {
                branchOn = string.Format("Working {0}", returnValue.FirstOrDefault(x => x.StartsWith("on branch")));
                this.Invoke(new Action(() =>
                {
                    this.toolTip1.SetToolTip(this.button1, "pull");
                    this.label2.Text = branchOn;
                    this.button1.Enabled = true;
                }), null);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            currentRespond = string.Empty;
            this.label1.ForeColor = Color.DarkGreen;
            var currentBranch = label1.Text;
            Task.Factory.StartNew(() =>
            {
                List<string> rtn = new List<string>();
                if (currentBranch != "...")
                    rtn = commandProcessor.PullRepo(workingDir.FullName).ToList();
                currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                UpdateRepoInfo();
            });
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var status = new Status(currentRespond, () =>
            {
                currentRespond = string.Empty;
                Task.Factory.StartNew(() =>
                {
                    this.label1.ForeColor = Color.DarkGreen;
                    var rtn = commandProcessor.ResetRepo(workingDir.FullName);
                    currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                    UpdateRepoInfo();
                });
            }, canReset);
            status.ShowDialog();
        }
    }
}
