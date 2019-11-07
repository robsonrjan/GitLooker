using System.Threading;

namespace GitLooker.Configuration
{
    public interface IRepoControlConfiguration
    {
        string RepoPath { get; }
        IAppSemaphoreSlim Semaphore { get; }
        string NewRepo { get; }
    }
}