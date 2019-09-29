using System.Threading;

namespace GitLooker.Configuration
{
    public interface IRepoControlConfiguration
    {
        string RepoPath { get; }
        SemaphoreSlim Semaphore { get; }
    }
}