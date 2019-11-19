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
        private readonly IRepoCommandProcessor commandProcessor;
        private readonly IAppSemaphoreSlim operationSemaphore;
        private readonly Control endControl;
        private string currentRespond;
        private string branchOn = "Pull current branch";
        private string newRepoConfiguration;
        private bool canReset;
        private bool isConfigured;
        private string newRepoName = default(string);
        private volatile bool isWaiting;

        internal bool IsNew { get; private set; }
        internal string RepoConfiguration { get; private set; }

        public RepoControl(IRepoControlConfiguration repoConfiguration, IRepoCommandProcessor commandProcessor, Control endControl)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoConfiguration?.RepoPath ?? string.Empty;
            this.commandProcessor = commandProcessor;
            this.endControl = endControl;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            operationSemaphore = repoConfiguration?.Semaphore ?? throw new ArgumentNullException(nameof(repoConfiguration));
            newRepoConfiguration = repoConfiguration?.NewRepo ?? string.Empty;
            IsNew = !string.IsNullOrEmpty(newRepoConfiguration);
            if (IsNew)
                ConfigureAsToClone();

            this.toolTip1.SetToolTip(this.button1, "pull");
            this.button1.Enabled = false;
        }

        private void Wait()
        {
            this.Invoke(new Action(() => { this.button2.Enabled = this.button1.Enabled = false; }), null);
            operationSemaphore.Wait();
            isWaiting = true;
            this.Invoke(new Action(() => this.label1.ForeColor = Color.DarkGreen), null);
        }

        private void Release()
        {
            if (isWaiting)
                operationSemaphore.Release();
            isWaiting = false;
            this.Invoke(new Action(() => { this.button2.Enabled = true; }), null);
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

            if (!Directory.Exists(workingDir.FullName))
                this.Dispose();

            Task.Factory.StartNew(() => CheckRepoProcess());
        }

        private void CheckRepoProcess()
        {
            Wait();
            try
            {
                canReset = false;
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
                this.Invoke(new Action(() => SetErrorForRepo()), null);
            }
            finally
            {
                Release();
            }            
        }

        public void HighlightLabel() => this.label1.ForeColor = Color.Green;

        private void SetErrorForRepo()
        {
            this.label1.ForeColor = Color.Red;
            this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
            this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
            Application.DoEvents();
            Application.DoEvents();
            this.button1.Enabled = false;
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
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.SendToBack();
                }), null);
            }
            else if (responseValue.Any(respound => respound.ToLower().Contains("could not resolve host:")))
            {
                returnValue = false;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.networkx;
                    this.SendToBack();
                }), null);
            }
            return returnValue;
        }

        private void CheckStatus(IEnumerable<string> returnValue)
        {
            bool needToPush = returnValue.Any(rtn => rtn.Contains("git push") || rtn.Contains("git add") || rtn.Contains("git checkout "));
            bool isMaster = branchOn.EndsWith("master");
            canReset = true && needToPush && isMaster;

            if (returnValue.Any(rtn => rtn.Contains("branch is behind")))
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_update_misc;
                    this.button1.Enabled = true;
                    this.SendToBack();
                }), null);
            }
            else if (needToPush)
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.move_task_up;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_add_to_autorun;
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.button_ok;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.checkedbox;
                    if (isMaster)
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
                    this.label2.Text = branchOn;
                }), null);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            currentRespond = string.Empty;
            var currentBranch = label1.Text;
            Task.Factory.StartNew(() => PullRepoProcess(currentBranch));
            MarkControl();
        }

        private void MarkControl()
        {
            int indexOfThis = Parent.Controls.GetChildIndex(this);
            int indexOfcontrol = endControl.Parent.Controls.GetChildIndex(endControl);
            if (indexOfcontrol > indexOfThis) indexOfThis++;
            endControl.Parent.Controls.SetChildIndex(endControl, indexOfThis - 1);
        }

        private void PullRepoProcess(string currentBranch)
        {
            Wait();
            try
            {
                List<string> rtn = new List<string>();
                if (currentBranch != "...")
                    rtn = commandProcessor.PullRepo(workingDir.FullName).ToList();
                currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                UpdateRepoInfo();
            }
            catch (Exception ex)
            {
                currentRespond = ex.Message;
                this.Invoke(new Action(() => SetErrorForRepo()), null);
            }
            finally
            {
                Release();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var status = new Status(currentRespond, () => DelegateResetMethod(), canReset);
            status.ShowDialog();
            MarkControl();
        }

        private void DelegateResetMethod()
        {
            currentRespond = string.Empty;
            Task.Factory.StartNew(() => ResetRepoProcess());
        }

        private void ResetRepoProcess()
        {
            Wait();
            try
            {
                var rtn = commandProcessor.ResetRepo(workingDir.FullName);
                currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                UpdateRepoInfo();
            }
            catch (Exception ex)
            {
                currentRespond = ex.Message;
                this.Invoke(new Action(() => SetErrorForRepo()), null);
            }
            finally
            {
                Release();
            }            
        }

        private void label1_Click(object sender, EventArgs e) => MarkControl();
    }
}
