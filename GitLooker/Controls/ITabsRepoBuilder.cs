using System.Windows.Forms;

namespace GitLooker.Controls
{
    public interface ITabsRepoBuilder
    {
        void BuildRepo(RepoControl.SelectRepo onSelectRepoEvent, TabReposControl repoControls, string repoDdir, string newRepo = null);
        void BuildTabs(TabControl reposCatalogs, RepoControl.SelectRepo onSelectRepoEvent);
        void ReBuildRepos(TabReposControl repoControl, RepoControl.SelectRepo onSelectRepoEvent);
    }
}