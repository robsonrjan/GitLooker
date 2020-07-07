using GitLooker.Core;
using GitLooker.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class RepoControl : UserControl
    {
        private readonly string repoPath;
        private readonly DirectoryInfo workingDir;
        private readonly IRepoCommandProcessorController commandProcessor;
        private readonly IRepoHolder repoHolder;
        private readonly Control endControl;
        private string currentRespond;
        private string branchOn = "Pull current branch";
        private string newRepoConfiguration;
        private bool canReset;
        private string newRepoName = default(string);
        private readonly string mainBranch;
        public bool IsMainBranch { get; private set; }
        public string RepoConfiguration { get; private set; }
        public bool CanPull { get; private set; }

        internal bool IsNew { get; private set; }

        public delegate void SelectRepo(RepoControl control);
        public event SelectRepo OnSelectRepo;
        public string RepoPath => repoPath;
        public string RepoName { get; }

        public RepoControl(IRepoControlConfiguration repoConfiguration, IRepoCommandProcessorController commandProcessor,
            Control endControl, IRepoHolder repoHolder)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoConfiguration?.RepoPath ?? string.Empty;
            this.commandProcessor = commandProcessor;
            this.endControl = endControl;
            this.repoHolder = repoHolder;
            RepoName = this.repoPath?.Split('\\').LastOrDefault() ?? string.Empty;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            newRepoConfiguration = repoConfiguration?.NewRepo ?? string.Empty;
            mainBranch = repoConfiguration.MainBranch;
            IsNew = !string.IsNullOrEmpty(newRepoConfiguration);
            if (IsNew)
                ConfigureAsToClone();

            this.toolTip1.SetToolTip(this.button1, "pull");
            this.button1.Enabled = false;
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

        public void UpdateRepoInfo() => InternalUpdateRepoInfo();

        private void InternalUpdateRepoInfo()
        {
            if (IsNew) return;

            if (!Directory.Exists(workingDir.FullName))
            {
                this.Dispose();
                return;
            }

            Task.Factory.StartNew(() => CheckRepoProcess());
        }

        private void CheckRepoProcess()
        {
            canReset = false;
            var result = RunCommands(new[] { "RemoteConfig", "CheckRepo" });

            if (string.IsNullOrWhiteSpace(RepoConfiguration))
                RepoConfiguration = result.SpecialValue.ToString();
        }

        private void SetStatusAfterCommandProcess(AppResult<IEnumerable<string>> returnValue)
        {
            if (returnValue.IsSuccess)
            {
                if (CheckIfExist(returnValue.Value.SelectMany(v => v)))
                {
                    CheckCurrentBranch(returnValue.Value.SelectMany(v => v));
                    CheckStatus(returnValue.Value.SelectMany(v => v));
                }

                currentRespond += string.Join(Environment.NewLine, returnValue.Value.SelectMany(v => v).ToArray());
                this.Invoke(new Action(() => { this.label1.ForeColor = Color.Navy; }), null);
            }
            else
                SetErrorForRepo(returnValue.Error.FirstOrDefault());
        }

        public void HighlightLabel() => this.label1.ForeColor = Color.DarkGreen;

        private void SetErrorForRepo(Exception ex = null)
        {
            if (ex != default)
            {
                currentRespond = ex.Message;
                this.Invoke(new Action(() => SetErrorForRepo()), null);
                return;
            }

            this.label1.ForeColor = Color.Red;
            this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
            this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
            Application.DoEvents();
            Application.DoEvents();
            this.button1.Enabled = false;
        }

        private bool CheckIfExist(IEnumerable<string> responseValue)
        {
            bool returnValue = true;
            if (responseValue.Any(respound => respound.ToLower().Contains("repository not found")
            ||
            respound.ToLower().Contains("does not exist or you do not have permissions")
            ||
            respound.ToLower().Contains("fatal: repository")
            ||
            respound.ToLower().Contains("received http code 403 from")))
            {
                returnValue = false;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_fail;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.SendToBack();
                }), null);
            }
            else if (responseValue.Any(respound => respound.ToLower().Contains("could not resolve host:")
                || (respound.ToLower().Contains("failed to connect to") && respound.ToLower().Contains("timed out"))))
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
            IsMainBranch = branchOn.EndsWith(mainBranch);
            canReset = true && needToPush && IsMainBranch;
            this.CanPull = false;

            if (!needToPush && returnValue.Any(rtn => rtn.Contains("branch is behind")))
            {
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_update_misc;
                    this.button1.Enabled = true;
                    this.SendToBack();
                    this.CanPull = true;
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
                    if (IsMainBranch)
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

        public void PullRepo() => Button1_Click(default, default);

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

            label1_DoubleClick(default, default);
        }

        private void PullRepoProcess(string currentBranch)
        {
            if (currentBranch != "...")
                RunCommands(new[] { "PullRepo", "CheckRepo" });
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
            => RunCommands(new[] { "ResetRepo", "CheckRepo" });


        public void CheckOutBranch(string branch)
            => RunCommands(new[] { "CheckOutBranch", "CheckRepo" });

        private void label1_Click(object sender, EventArgs e) => MarkControl();

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            if (OnSelectRepo != default)
                OnSelectRepo(this);
        }

        private AppResult<IEnumerable<string>> RunCommands(IEnumerable<string> commnds)
        {
            var commandList = new List<MethodInfo>();
            foreach (var command in commnds)
                commandList.Add(commandProcessor.CommonCommandActions.FirstOrDefault(k => k.Key == command).Value);

            this.Invoke(new Action(() => { this.button2.Enabled = this.button1.Enabled = false; }), null);
            var result = commandProcessor.Execute(commandList, new[] { workingDir.FullName, mainBranch }, () => this.Invoke(new Action(() => HighlightLabel()), null));

            SetStatusAfterCommandProcess(result);
            this.Invoke(new Action(() => { this.button2.Enabled = true; }), null);
            return result;
        }
    }
}
