using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using GitLooker.Core;
using static GitLooker.Controls.RepoControl;
using System.IO;

namespace GitLooker.Controls
{
    public class TabsRepoBuilder : ITabsRepoBuilder
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAppConfiguration appConfiguration;
        private readonly IGitFileRepo gitFileRepo;
        private readonly IMainForm mainForm;

        public TabsRepoBuilder(IServiceProvider serviceProvider,
            IAppConfiguration appConfiguration,
            IGitFileRepo gitFileRepo,
            IMainForm mainForm)
        {
            this.appConfiguration = appConfiguration;
            this.serviceProvider = serviceProvider;
            this.gitFileRepo = gitFileRepo;
            this.mainForm = mainForm;
        }

        public void Build(TabControl reposCatalogs, SelectRepo onSelectRepoEvent)
        {
            foreach (var item in appConfiguration.Select((value, index) => (Index: index, Config: value)))
            {
                var newTab = serviceProvider.GetService<TabReposControl>();
                newTab.RepoIndex = item.Index;
                newTab.RepoConfiguration = item.Config;
                reposCatalogs.Controls.Add(newTab);
                CheckForGitRepo(newTab, onSelectRepoEvent);
            }
        }

        public void ReBuild(TabReposControl repoControl, SelectRepo onSelectRepoEvent) => CheckForGitRepo(repoControl, onSelectRepoEvent);

        private void CheckForGitRepo(TabReposControl repoControl, SelectRepo onSelectRepoEvent)
        {
            if (Directory.Exists(repoControl.RepoConfiguration.GitLookerPath))
                foreach (var repo in gitFileRepo.Get(repoControl.RepoConfiguration.GitLookerPath))
                    AddRepo(onSelectRepoEvent, repoControl, repo);
        }

        public void AddRepo(SelectRepo onSelectRepoEvent,
            TabReposControl repoControls,
            string repoDdir,
            string newRepo = default)
        {
            mainForm.CurrentRepoDdir = repoDdir;
            mainForm.CurrentNewRepo = newRepo;
            var repo = serviceProvider.GetService<RepoControl>();
            repo.OnSelectRepo += onSelectRepoEvent;
            repo.Dock = DockStyle.Top;
            repoControls.RepoAdd(repo);
            Application.DoEvents();
        }
    }
}
