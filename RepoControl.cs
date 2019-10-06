using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System;
using GitLooker.Configuration;
using GitLooker.CommandProcessor;

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
        private bool canReset;

        public RepoControl(IRepoControlConfiguration repoConfiguration, ICommandProcessor commandProcessor)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoConfiguration.RepoPath;
            this.commandProcessor = commandProcessor;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            this.semaphore = repoConfiguration.Semaphore;
        }

        private void RepoControl_Load(object sender, EventArgs e)
        {
            //UpdateRepoInfo();
        }

        public void UpdateRepoInfo()
        {
            Task.Factory.StartNew(() =>
            {
                semaphore.Wait();
                try
                {
                    canReset = false;
                    this.Invoke(new Action(() => { this.label1.ForeColor = Color.DarkGreen; }), null);

                    if (workingDir.FullName.EndsWith("test"))
                        currentRespond = "";

                    var returnValue = commandProcessor.CheckRemoteRepo(workingDir.FullName);
                    if (CheckIfExist(returnValue))
                    {
                        returnValue = commandProcessor.CheckRepo(workingDir.FullName);
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

        private bool CheckIfExist(IEnumerable<string> responseValue)
        {
            bool returnValue = true;
            if (responseValue.Any(respound => respound.ToLower().Contains("repository not found")))
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
                this.Invoke(new Action(() => {
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
                this.Invoke(new Action(() => {
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
                    this.toolTip1.SetToolTip(this.button1, branchOn);
                    this.label2.Text = branchOn;
                    this.button1.Enabled = true;
                }), null);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            currentRespond = string.Empty;
            this.label1.ForeColor = Color.DarkGreen;
            Task.Factory.StartNew(() =>
            {
                var rtn = commandProcessor.PullRepo(workingDir.FullName);
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
