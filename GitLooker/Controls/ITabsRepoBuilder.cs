using System.Windows.Forms;

namespace GitLooker.Controls
{
    public interface ITabsRepoBuilder
    {
        void AddRepo(RepoControl.SelectRepo onSelectRepoEvent, TabReposControl repoControls, string repoDdir, string newRepo = null);
        void Build(TabControl reposCatalogs, RepoControl.SelectRepo onSelectRepoEvent);
        void ReBuild(TabReposControl repoControl, RepoControl.SelectRepo onSelectRepoEvent);
    }
}