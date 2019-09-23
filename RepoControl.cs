using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class RepoControl : UserControl
    {
        private const string commandUpdate = "git remote update";
        private const string commandStatus = "git status";
        private const string commandPull = "git pull origin master";
        private IEnumerable<string> responseRegexPattern = new[] { "master", "->", "origin", "master" };

        private readonly string repoPath;
        private readonly DirectoryInfo workingDir;
        private readonly IPowersShell powerShell;
        private TimeSpan waitingTime = TimeSpan.FromMinutes(60);
        private DateTime lastTime;

        private bool isLoaded;

        public RepoControl(string repoPath, SemaphoreSlim semaphore, IPowersShell powerShell)
        {
            InitializeComponent();
            this.label1.Text = this.repoPath = repoPath;
            this.powerShell = powerShell;

            lastTime = DateTime.UtcNow;
            workingDir = new DirectoryInfo(repoPath);
            this.label1.Text = workingDir.Name;
        }

        private void RepoControl_Load(object sender, EventArgs e)
        {
            this.button1.BackgroundImage = null;
            this.button1.Enabled = false;
            this.button2.BackgroundImage = null;
            this.button2.Enabled = false;
            this.timer1.Interval = 300000;
            this.timer1.Enabled = true;
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
                lastTime.Add(waitingTime);
            else
                return;

            UpdateRepoInfo();
        }

        private async void UpdateRepoInfo() => await CheckRepo();

        private Task CheckRepo()
        {


            return Task.CompletedTask;
        }
    }
}
