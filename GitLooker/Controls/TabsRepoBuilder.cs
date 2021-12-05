using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
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
            foreach (var config in appConfiguration)
                BuildTab(reposCatalogs, onSelectRepoEvent, config);
        }

        public void BuildTab(TabControl reposCatalogs, SelectRepo onSelectRepoEvent, RepoConfig repo)
        {
            var newTab = serviceProvider.GetService<TabReposControl>();
            newTab.RepoConfiguration = repo;
            newTab.BackColor = Color.White;
            reposCatalogs.Controls.Add(newTab);
            CheckForGitRepo(newTab, onSelectRepoEvent);
        }

        public void BuildTab(TabControl reposCatalogs, SelectRepo onSelectRepoEvent, string repoPath)
        {
            if (!Directory.Exists(repoPath))
            {
                MessageBox.Show($"Path {repoPath} is not valid.", "Invalid repo path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var config = new RepoConfig { GitLookerPath = repoPath };
            appConfiguration.Add(config);
            BuildTab(reposCatalogs, onSelectRepoEvent, config);
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
