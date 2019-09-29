using System;
using System.Threading;

namespace GitLooker.Configuration
{
    public class RepoControlConfiguration : IRepoControlConfiguration
    {
        public string RepoPath { get; }
        public SemaphoreSlim Semaphore { get; }

        public RepoControlConfiguration(string repoPath, SemaphoreSlim semaphore)
        {
            if (string.IsNullOrEmpty(repoPath))
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] -> Argument {repoPath} can not be null or empty!");

            if (semaphore == null)
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] ->Argument {semaphore} can not be null!");

            this.RepoPath = repoPath;
            this.Semaphore = semaphore;
        }
    }
}
