﻿using GitLooker.Core.Services;
using System;


namespace GitLooker.Services.Configuration
{
    public class RepoControlConfiguration : IRepoControlConfiguration
    {
        public string RepoPath { get; }
        public IAppSemaphoreSlim Semaphore { get; }
        public string NewRepo { get; }
        public string MainBranch { get; }

        public RepoControlConfiguration(string repoPath, IAppSemaphoreSlim semaphore, string newRepo, string mainBranch)
        {
            if (string.IsNullOrEmpty(repoPath))
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] -> Argument {repoPath} can not be null or empty!");

            if (semaphore == null)
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] ->Argument {semaphore} can not be null!");

            this.RepoPath = repoPath;
            this.Semaphore = semaphore;
            NewRepo = newRepo;
            MainBranch = mainBranch;
        }
    }
}
