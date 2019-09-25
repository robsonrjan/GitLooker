using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private string currentRespond;
        private string branchOn = "Pull current branch";

        private delegate void SetTextCallback(string text);
        private SetTextCallback tipDelegate;

        public RepoControl(string repoPath, SemaphoreSlim semaphore, IPowersShell powerShell)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoPath;
            this.powerShell = powerShell;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            this.semaphore = semaphore;

            tipDelegate = new SetTextCallback(SetToolTipText);
        }

        private void RepoControl_Load(object sender, EventArgs e)
        {
            UpdateRepoInfo();
        }

        private void Label1_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void Label1_MouseLeave(object sender, EventArgs e)
        {
   
        }

        private void SetToolTipText(string text)
            => this.toolTip1.SetToolTip(this.button1, text);

        public void UpdateRepoInfo(bool noRespound = false)
        {
            Task.Factory.StartNew(() =>
            {
                semaphore.Wait();
                try
                {
                    this.Invoke(new Action(() => { this.label1.ForeColor = Color.DarkGreen; }), null);
                    var returnValue = CheckRepo();

                    if (returnValue.Any(rtn => rtn.StartsWith("On branch")))
                    {
                        branchOn = string.Format("Pull {0}", returnValue.FirstOrDefault(x => x.StartsWith("On branch")));
                        
                        this.Invoke(tipDelegate, new object[] { branchOn });
                    }

                    if (returnValue.Any(rtn => rtn.Contains("branch is behind")))
                    {
                        this.button2.BackgroundImage = global::GitLooker.Properties.Resources.button_cancel;
                        this.Invoke(new Action(() => { this.SendToBack(); }), null);
                    }
                    else
                    {
                        this.button2.BackgroundImage = global::GitLooker.Properties.Resources.agt_action_success;
                        this.Invoke(new Action(() => { this.BringToFront(); }), null);
                    }

                    if (!noRespound)
                        currentRespond = string.Join(Environment.NewLine, returnValue.ToArray());

                    this.Invoke(new Action(() => { this.label1.ForeColor = Color.Navy; }), null);
                }
                catch (Exception err)
                {
                    currentRespond = err.Message;
                }
                semaphore.Release();
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

        private string GenerateUpdateWithStatusCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandUpdate,
            commandStatus
        });

        private string GeneratePullCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandPull
        });

        private IEnumerable<string> CheckRepo()
        {
            var rtn = powerShell.Execute(GenerateUpdateWithStatusCommand);
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

        private void Button2_Click(object sender, EventArgs e)
        {
            var status = new Status(currentRespond);
            status.ShowDialog();
        }
    }
}
