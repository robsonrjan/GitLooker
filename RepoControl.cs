using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System;

namespace GitLooker
{
    public partial class RepoControl : UserControl
    {
        private const string commandUpdate = "git remote update";
        private const string commandStatus = "git status";
        private const string commandPull = "git pull";
        private const string commandReset = "git reset --hard";
        private const string commandClean = "git clean -df";
        private const string commandPath = "cd \"{0}\"";

        private readonly string repoPath;
        private readonly DirectoryInfo workingDir;
        private readonly IPowersShell powerShell;
        private readonly SemaphoreSlim semaphore;
        private string currentRespond;
        private string branchOn = "Pull current branch";
        private bool canReset;

        public RepoControl(string repoPath, SemaphoreSlim semaphore, IPowersShell powerShell)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoPath;
            this.powerShell = powerShell;

            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
            this.semaphore = semaphore;
        }

        private void RepoControl_Load(object sender, EventArgs e)
        {
            UpdateRepoInfo();
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
                    var returnValue = CheckRepo();

                    if (returnValue.Any(rtn => rtn.StartsWith("on branch")))
                    {
                        branchOn = string.Format("Working {0}", returnValue.FirstOrDefault(x => x.StartsWith("on branch")));
                        this.Invoke(new Action(() =>
                        {
                            this.toolTip1.SetToolTip(this.button1, branchOn);
                            this.label2.Text = branchOn;
                        }), null);
                    }

                    if (returnValue.Any(rtn => rtn.Contains("branch is behind")))
                    {
                        this.button2.BackgroundImage = global::GitLooker.Properties.Resources.checkmark;
                        this.Invoke(new Action(() => { this.SendToBack(); }), null);
                    }
                    else if (returnValue.Any(rtn => rtn.Contains("git push") || rtn.Contains("git add") || rtn.Contains("git checkout ")))
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.button2.BackgroundImage = global::GitLooker.Properties.Resources.move_task_up;
                        }));
                        canReset = true;
                    }
                    else
                    {
                        this.button2.BackgroundImage = global::GitLooker.Properties.Resources.button_ok;
                        this.Invoke(new Action(() => { this.BringToFront(); }), null);
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

        private string GenerateResetCommand => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir.FullName),
            commandClean,
            commandReset
        });

        private IEnumerable<string> CheckRepo()
        {
            var rtn = powerShell.Execute(GenerateUpdateWithStatusCommand);
            return rtn.Select(x => x.BaseObject.ToString().ToLower());            
        }

        private IEnumerable<string> PullRepo()
        {
            var rtn = powerShell.Execute(GeneratePullCommand);
            return rtn.Select(x => x.BaseObject.ToString().ToLower());
        }

        private IEnumerable<string> ResetRepo()
        {
            var rtn = powerShell.Execute(GenerateResetCommand);
            return rtn.Select(x => x.BaseObject.ToString().ToLower());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            currentRespond = string.Empty;
            Task.Factory.StartNew(() =>
            {
                var rtn = PullRepo();
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
                    var rtn = ResetRepo();
                    currentRespond = string.Join(Environment.NewLine, rtn.ToArray());
                    UpdateRepoInfo();
                });
            }, canReset);
            status.ShowDialog();
        }
    }
}
