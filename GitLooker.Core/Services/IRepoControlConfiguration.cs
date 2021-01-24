using System.Windows.Forms;

namespace GitLooker.Core.Services
{
    public interface IRepoControlConfiguration
    {
        string RepoPath { get; }
        IAppSemaphoreSlim Semaphore { get; }
        string NewRepo { get; }
        string MainBranch { get; }
        Control EndControl { get; }
    }
}
