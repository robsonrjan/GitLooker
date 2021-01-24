using GitLooker.Core;
using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using System;
using System.Windows.Forms;

namespace GitLooker.Services.Configuration
{
    public class RepoControlConfiguration : IRepoControlConfiguration
    {
        public string RepoPath { get; }
        public IAppSemaphoreSlim Semaphore { get; }
        public string NewRepo { get; }
        public string MainBranch { get; }
        public Control EndControl { get; }

        public RepoControlConfiguration(IMainForm mainForm, IAppSemaphoreSlim semaphore, IAppConfiguration appConfiguration)
        {
            if (string.IsNullOrWhiteSpace(mainForm.CurrentRepoDdir))
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] -> Argument {mainForm.CurrentRepoDdir} can not be null or empty!", nameof(mainForm.CurrentRepoDdir));

            if (semaphore == default)
                throw new ArgumentException($"[{nameof(RepoControlConfiguration)}] ->Argument {semaphore} can not be null!", nameof(semaphore));

            this.RepoPath = mainForm.CurrentRepoDdir;
            this.Semaphore = semaphore;
            NewRepo = mainForm.CurrentNewRepo;
            MainBranch = appConfiguration.MainBranch;
            EndControl = mainForm.EndControl;
        }
    }
}
