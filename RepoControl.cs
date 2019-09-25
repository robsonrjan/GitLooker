using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace GitLooker
{
    public partial class RepoControl : UserControl
    {
        private const string commandUpdate = "git remote update";
        private const string commandStatus = "git status";
        private const string commandPull = "git pull";
        private const string commandPath = "cd \"{0}\"";

        private readonly string repoPath;
        private readonly DirectoryInfo workingDir;
        private readonly IPowersShell powerShell;
        private readonly SemaphoreSlim semaphore;
        private TimeSpan waitingTime = TimeSpan.FromMinutes(30);
        private DateTime lastTime;
        private string currentRespond;
        private string branchOn = "Pull current branch";

        private bool isLoaded;
        delegate void SetTextCallback(string text);

        public RepoControl(string repoPath, SemaphoreSlim semaphore, IPowersShell powerShell)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoPath;
            this.powerShell = powerShell;

            lastTime = DateTime.UtcNow;
            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            this.semaphore = semaphore;
        }

        private void RepoControl_Load(object sender, EventArgs e)
        {
            this.timer1.Interval = 300000;
            this.timer1.Enabled = isLoaded = true;
            Timer1_Tick(null, null);
           
        }

        private void Label1_MouseEnter(object sender, EventArgs e)
        {
            this.label1.BorderStyle = BorderStyle.Fixed3D;
        }

        private void Label1_MouseLeave(object sender, EventArgs e)
        {
            this.label1.BorderStyle = BorderStyle.None;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!isLoaded) return;

            if (DateTime.UtcNow > lastTime)
                lastTime = lastTime.Add(waitingTime);
            else
                return;

            UpdateRepoInfo();
        }

        private void SetToolTipText(string text)
        {
            this.toolTip1.SetToolTip(this.button1, text);
        }

        private void UpdateRepoInfo(bool noRespound = false)
        {
            Task.Factory.StartNew(() =>
            {
                var returnValue = CheckRepo();

                if (returnValue.Any(rtn => rtn.StartsWith("On branch")))
                {
                    branchOn = string.Format("Pull {0}", returnValue.FirstOrDefault(x => x.StartsWith("On branch")));
                    var tipDelegate = new SetTextCallback(SetToolTipText);
                    this.Invoke(tipDelegate, new object[] { branchOn });
                }

                if (returnValue.Any(rtn => rtn.Contains("branch is behind")))
                {
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.button_cancel;
                    this.SendToBack();
                }
                else
                    this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_success;

                if (!noRespound)
                    currentRespond = string.Join(Environment.NewLine, returnValue.ToArray());
            });
        }

        private string GenerateStatusCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandStatus
        });

        private string GenerateUpdateCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandUpdate
        });

        private string GeneratePullCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandPull
        });

        private IEnumerable<string> CheckRepo()
        {
            semaphore.Wait();
            powerShell.Execute(GenerateUpdateCommand);
            var rtn = powerShell.Execute(GenerateStatusCommand);
            semaphore.Release();
            return rtn.Select(x => x.BaseObject.ToString());            
        }

        private IEnumerable<string> PullRepo()
        {
            semaphore.Wait();
            var rtn = powerShell.Execute(GeneratePullCommand);
            semaphore.Release();
            return rtn.Select(x => x.BaseObject.ToString());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var rtn = PullRepo();
                currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                UpdateRepoInfo(true);
            });            
        }
    }
}
