using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GitLooker.Controls
{
    public class TabReposControl : TabPage, IEnumerable<RepoControl>
    {
        private readonly Panel endControl;
        private readonly List<RepoControl> allReposControl;
        private readonly IRepoHolder repoHolder;
        private RepoConfig repoConfiguration;

        public TabReposControl(IRepoHolder repoHolder)
        {
            endControl = new Panel();
            allReposControl = new List<RepoControl>();
            this.repoHolder = repoHolder;
            RepoLastTimeUpdate = DateTime.UtcNow;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            endControl.BackColor = System.Drawing.Color.DarkSlateGray;
            endControl.Dock = DockStyle.Top;
            endControl.Location = new System.Drawing.Point(0, 0);
            endControl.Name = "endControl";
            endControl.Size = new System.Drawing.Size(576, 1);

            Dock = DockStyle.Fill;
            Location = new System.Drawing.Point(4, 22);
            Name = "repos";
            Size = new System.Drawing.Size(576, 706);
            TabIndex = 0;
            Text = "repo";
            Controls.Add(endControl);

            ResumeLayout(false);
        }

        public RepoConfig RepoConfiguration
        {
            get => repoConfiguration;
            set
            {
                if (!string.IsNullOrWhiteSpace(value?.GitLookerPath))
                    Name = Text = new DirectoryInfo(value.GitLookerPath).Name.ToLowerInvariant();

                repoConfiguration = value;
            }
        }
        public int RepoIndex { get; set; }

        public Panel RepoEndControl => endControl;

        public void RepoAdd(RepoControl control)
        {
            Controls.Add(control);
            allReposControl.Add(control);
        }

        public List<RepoControl> ReposAllControl => allReposControl;

        public IRepoHolder RepoHolder => repoHolder;

        public DateTime RepoLastTimeUpdate { get; set; }

        public bool RepoIsLoaded { get; set; }

        public void RepoClearControls()
        {
            Controls.Clear();
            Controls.Add(endControl);
            allReposControl.ForEach(r => r.Dispose());
            allReposControl.Clear();
            repoHolder.RepoRemoteList.Clear();
        }

        public void RepoRemove(RepoControl ctr)
        {
            Controls.Remove(ctr);
            allReposControl.Remove(ctr);
            repoHolder.ExpectedRemoteList.Remove(ctr.RepoConfiguration);
            ctr.Dispose();
        }

        public IEnumerator<RepoControl> GetEnumerator() => allReposControl.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => allReposControl.GetEnumerator();
    }
}
