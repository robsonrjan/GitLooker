using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static GitLooker.Controls.RepoControl;

namespace GitLooker.Controls
{
    public class TabsRepoBuilder : ITabsRepoBuilder
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAppConfiguration appConfiguration;
        private readonly IGitFileRepo gitFileRepo;

        public TabsRepoBuilder(IServiceProvider serviceProvider,
            IAppConfiguration appConfiguration,
            IGitFileRepo gitFileRepo)
        {
            this.appConfiguration = appConfiguration;
            this.serviceProvider = serviceProvider;
            this.gitFileRepo = gitFileRepo;
        }

        public void BuildTabs(TabControl reposCatalogs, SelectRepo onSelectRepoEvent)
        {
            foreach (var item in appConfiguration.Select((value, index) => (Index: index, Config: value)))
            {
                var newTab = serviceProvider.GetService<TabReposControl>();
                newTab.RepoIndex = item.Index;
                newTab.RepoConfiguration = item.Config;
                newTab.BackColor = Color.White;
                reposCatalogs.Controls.Add(newTab);
                CheckForGitRepo(newTab, onSelectRepoEvent);
            }
        }

        public void ReBuildRepos(TabReposControl repoControl, SelectRepo onSelectRepoEvent) => CheckForGitRepo(repoControl, onSelectRepoEvent);

        private void CheckForGitRepo(TabReposControl repoControl, SelectRepo onSelectRepoEvent)
        {
            if (Directory.Exists(repoControl.RepoConfiguration.GitLookerPath))
                foreach (var repo in gitFileRepo.Get(repoControl.RepoConfiguration.GitLookerPath))
                    BuildRepo(onSelectRepoEvent, repoControl, repo);
        }

        public void BuildRepo(SelectRepo onSelectRepoEvent,
            TabReposControl repoControls,
            string repoDdir,
            string newRepo = default)
        {
            var repo = serviceProvider.GetService<RepoControl>();
            repo.RepoPath = repoDdir;
            repo.NewRepo = newRepo;
            repo.MainBranch = repoControls.RepoConfiguration.MainBranch;
            repo.EndControl = repoControls.RepoEndControl;
            repo.OnSelectRepo += onSelectRepoEvent;
            repo.Dock = DockStyle.Top;
            repoControls.RepoAdd(repo);
            Application.DoEvents();
        }
    }
}
