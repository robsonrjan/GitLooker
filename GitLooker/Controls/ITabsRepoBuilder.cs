using GitLooker.Core.Configuration;
using System.Windows.Forms;
using static GitLooker.Controls.RepoControl;

namespace GitLooker.Controls
{
    public interface ITabsRepoBuilder
    {
        void BuildRepo(RepoControl.SelectRepo onSelectRepoEvent, TabReposControl repoControls, string repoDdir, string newRepo = default);
        void BuildTabs(TabControl reposCatalogs, RepoControl.SelectRepo onSelectRepoEvent);
        void BuildTab(TabControl reposCatalogs, SelectRepo onSelectRepoEvent, RepoConfig repo);
        void BuildTab(TabControl reposCatalogs, SelectRepo onSelectRepoEvent, string repoPath);
        void ReBuildRepos(TabReposControl repoControl, RepoControl.SelectRepo onSelectRepoEvent);
    }
}