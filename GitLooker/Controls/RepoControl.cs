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

namespace GitLooker.Controls
{
    public partial class RepoControl : UserControl
    {
        private DirectoryInfo workingDir;
        private readonly IRepoCommandProcessorController commandProcessor;
        private string repoPath;
        private string currentRespond;
        private string branchOn = "Pull current branch";
        private string newRepoConfiguration;
        private bool canReset;
        private string newRepoName = default;
        private string mainBranch;
        public bool IsMainBranch { get; private set; }
        public bool IsConnectionError { get; private set; }
        public string RepoConfiguration { get; private set; }
        public bool CanPull { get; private set; }
        internal bool IsNew { get; private set; }
        internal bool IsNeededUpdate { get; private set; }
        public delegate void SelectRepo(RepoControl control);
        public event SelectRepo OnSelectRepo;
        public string RepoPath { get => repoPath; set => repoPath = value ?? string.Empty; }
        public string RepoName { get; private set; }
        public Control EndControl { get; set; }
        public string NewRepo { get => newRepoConfiguration; set => newRepoConfiguration = value ?? string.Empty; }
        public string MainBranch { get => mainBranch; set => mainBranch = value ?? string.Empty; }

        public RepoControl(IRepoCommandProcessorController commandProcessor)
        {
            InitializeComponent();
            this.commandProcessor = commandProcessor;
            toolTip1.SetToolTip(this.button1, "pull");
            button1.Enabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.label1.Text = this.repoPath;
            RepoName = this.repoPath?.Split('\\').LastOrDefault() ?? string.Empty;
            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            IsNew = !string.IsNullOrEmpty(newRepoConfiguration);
            if (IsNew)
                ConfigureAsToClone();
            base.OnLoad(e);
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
            IsNeededUpdate = default;

            if (returnValue.IsSuccess)
            {
                if (CheckIfExist(returnValue.Value.SelectMany(v => v)))
                {
                    CheckCurrentBranch(returnValue.Value.SelectMany(v => v));
                    CheckStatus(returnValue.Value.SelectMany(v => v));
                }

                currentRespond += string.Join(Environment.NewLine, returnValue.Value.SelectMany(v => v).ToArray());
                this.Invoke(new Action(() => { this.label1.ForeColor = Color.Navy; }), default);
            }
            else
                SetErrorForRepo(returnValue.Error.FirstOrDefault());
        }

        public void HighlightLabel() => this.label1.ForeColor = Color.DarkGreen;

        private void SetErrorForRepo(Exception ex = default)
        {
            if (ex != default)
            {
                currentRespond = ex.Message;
                this.Invoke(new Action(() => SetErrorForRepo()), default);
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
            IsConnectionError = false;
            if (responseValue.Any(respound => respound.ToLowerInvariant().Contains("repository not found")
            ||
            respound.ToLowerInvariant().Contains("does not exist or you do not have permissions")
            ||
            respound.ToLowerInvariant().Contains("fatal:")
            ||
            respound.ToLowerInvariant().Contains("received http code 403 from")))
            {
                IsConnectionError = true;
                returnValue = false;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = Properties.Resources.agt_action_fail;
                    this.button1.BackgroundImage = Properties.Resources.checkmark;
                    this.SendToBack();
                }), default);
            }
            else if (responseValue.Any(respound => respound.ToLowerInvariant().Contains("could not resolve host:")
                || (respound.ToLowerInvariant().Contains("failed to connect to") && respound.ToLowerInvariant().Contains("timed out"))))
            {
                IsConnectionError = true;
                returnValue = false;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = Properties.Resources.checkmark;
                    this.button1.BackgroundImage = Properties.Resources.networkx;
                    this.SendToBack();
                }), default);
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
                IsNeededUpdate = true;
                this.Invoke(new Action(() =>
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                    this.button1.BackgroundImage = global::GitLooker.Properties.Resources.agt_update_misc;
                    this.button1.Enabled = true;
                    this.SendToBack();
                    this.CanPull = true;
                }), default);
            }
            else if (needToPush)
            {
                IsNeededUpdate = true;
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
                }), default);
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
                }), default);
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
            int indexOfcontrol = EndControl.Parent.Controls.GetChildIndex(EndControl);
            if (indexOfcontrol > indexOfThis) indexOfThis++;
            EndControl.Parent.Controls.SetChildIndex(EndControl, indexOfThis - 1);

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
            var parameters = new[] { workingDir.FullName, mainBranch };
            foreach (var command in commnds)
                commandList.Add(commandProcessor.CommonCommandActions.FirstOrDefault(k => k.Name == command));

            this.Invoke(new Action(() => { this.button2.Enabled = this.button1.Enabled = false; }), default);
            var result = commandProcessor.Execute(commandList, parameters, () => Invoke(new Action(() => HighlightLabel()), default));

            SetStatusAfterCommandProcess(result);
            this.Invoke(new Action(() => { button2.Enabled = true; }), default);
            return result;
        }
    }
}
