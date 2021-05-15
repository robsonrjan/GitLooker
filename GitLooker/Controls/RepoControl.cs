using GitLooker.Core;
using GitLooker.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        private readonly SortedDictionary<DateTime, string> allResponds;

        public RepoControl(IRepoCommandProcessorController commandProcessor)
        {
            InitializeComponent();
            this.commandProcessor = commandProcessor;
            toolTip1.SetToolTip(button1, "pull");
            button1.Enabled = false;
            allResponds = new SortedDictionary<DateTime, string>();
        }

        protected override void OnLoad(EventArgs e)
        {
            label1.Text = repoPath;
            RepoName = repoPath?.Split('\\').LastOrDefault() ?? string.Empty;
            workingDir = new DirectoryInfo(repoPath);
            label1.Text = workingDir.Name;
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

        private string GetRespond
            => string.Join(Environment.NewLine, allResponds.Select(keyPair => $"------ Sesion: {keyPair.Key.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern)}   {keyPair.Key.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern)}{Environment.NewLine}{keyPair.Value}{Environment.NewLine}").ToArray());

        private void ConfigureAsToClone()
        {
            button2.BackgroundImage = Properties.Resources.cancel;
            button1.BackgroundImage = Properties.Resources.cancel;
            button1.Enabled = false;
            button2.Enabled = false;
            toolTip1.SetToolTip(button1, "repo to clone");
            label1.Text = GetNewRepoName;
            RepoConfiguration = newRepoConfiguration;
        }

        public void UpdateRepoInfo() => InternalUpdateRepoInfo();

        private void InternalUpdateRepoInfo()
        {
            if (IsNew) return;

            if (!Directory.Exists(workingDir.FullName))
            {
                Dispose();
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

                allResponds.Add(DateTime.Now, string.Join(Environment.NewLine, returnValue.Value.SelectMany(v => v).ToArray()));
                Invoke(new Action(() => { label1.ForeColor = Color.Navy; }), default);
            }
            else
                SetErrorForRepo(returnValue.Error.FirstOrDefault());
        }

        public void HighlightLabel() => label1.ForeColor = Color.DarkGreen;

        private void SetErrorForRepo(Exception ex = default)
        {
            if (ex != default)
            {
                allResponds.Add(DateTime.Now, ex.Message);
                Invoke(new Action(() => SetErrorForRepo()), default);
                return;
            }

            label1.ForeColor = Color.Red;
            button2.BackgroundImage = Properties.Resources.agt_action_fail;
            button1.BackgroundImage = Properties.Resources.agt_action_fail;
            Application.DoEvents();
            Application.DoEvents();
            button1.Enabled = false;
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
                Invoke(new Action(() =>
                {
                    button2.BackgroundImage = Properties.Resources.agt_action_fail;
                    button1.BackgroundImage = Properties.Resources.checkmark;
                    SendToBack();
                }), default);
            }
            else if (responseValue.Any(respound => respound.ToLowerInvariant().Contains("could not resolve host:")
                || (respound.ToLowerInvariant().Contains("failed to connect to") && respound.ToLowerInvariant().Contains("timed out"))))
            {
                IsConnectionError = true;
                returnValue = false;
                Invoke(new Action(() =>
                {
                    button2.BackgroundImage = Properties.Resources.checkmark;
                    button1.BackgroundImage = Properties.Resources.networkx;
                    SendToBack();
                }), default);
            }
            return returnValue;
        }

        private void CheckStatus(IEnumerable<string> returnValue)
        {
            bool needToPush = returnValue.Any(rtn => rtn.Contains("git push") || rtn.Contains("git add") || rtn.Contains("git checkout "));
            IsMainBranch = branchOn.EndsWith(mainBranch);
            canReset = true && needToPush && IsMainBranch;
            CanPull = false;

            if (!needToPush && returnValue.Any(rtn => rtn.Contains("branch is behind")))
            {
                IsNeededUpdate = true;
                Invoke(new Action(() =>
                {
                    button2.BackgroundImage = Properties.Resources.checkmark;
                    button1.BackgroundImage = Properties.Resources.agt_update_misc;
                    button1.Enabled = true;
                    SendToBack();
                    CanPull = true;
                }), default);
            }
            else if (needToPush)
            {
                IsNeededUpdate = true;
                Invoke(new Action(() =>
                {
                    button2.BackgroundImage = Properties.Resources.move_task_up;
                    button1.BackgroundImage = Properties.Resources.agt_add_to_autorun;
                }));
            }
            else
            {
                Invoke(new Action(() =>
                {
                    button2.BackgroundImage = Properties.Resources.button_ok;
                    button1.BackgroundImage = Properties.Resources.checkedbox;
                    if (IsMainBranch)
                        BringToFront();
                }), default);
            }
        }

        private void CheckCurrentBranch(IEnumerable<string> returnValue)
        {
            if (returnValue.Any(rtn => rtn.StartsWith("on branch")))
            {
                branchOn = string.Format("Working {0}", returnValue.FirstOrDefault(x => x.StartsWith("on branch")));
                Invoke(new Action(() =>
                {
                    label2.Text = branchOn;
                }), default);
            }
        }

        public void PullRepo() => Button1_Click(default, default);

        private void Button1_Click(object sender, EventArgs e)
        {
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
            var status = new Status(GetRespond, () => DelegateResetMethod(), canReset);
            status.ShowDialog();
            MarkControl();
        }

        private void DelegateResetMethod()
            => Task.Factory.StartNew(() => ResetRepoProcess());

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

            Invoke(new Action(() => { button2.Enabled = button1.Enabled = false; }), default);
            var result = commandProcessor.Execute(commandList, parameters, () => Invoke(new Action(() => HighlightLabel()), default));

            SetStatusAfterCommandProcess(result);
            Invoke(new Action(() => { button2.Enabled = true; }), default);
            return result;
        }
    }
}
