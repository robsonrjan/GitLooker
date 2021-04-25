using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GitLooker.Services.Services
{
    public class AppSemaphoreSlim : IAppSemaphoreSlim
    {
        private const int DueTimeMs = 300;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly int repoProcessingCount;
        private Action<bool> isUsed;
        public Action<bool> OnUse { get => isUsed; set => isUsed = value; }
        public int MaxRepoProcessingCount => repoProcessingCount;
        private Timer timer;

        public AppSemaphoreSlim(IAppConfiguration appConfiguration)
        {
            this.repoProcessingCount = appConfiguration.RepoProcessingCount;
            semaphoreSlim = new SemaphoreSlim(repoProcessingCount);
        }

        private void PrepareCallBackAction(TimerCallback action)
        {
            if (timer == default)
                timer = new Timer(action, default, DueTimeMs, int.MaxValue);
            else
                timer.Change(DueTimeMs, int.MaxValue);
        }

        public void Wait()
        {
            semaphoreSlim.Wait();
            OnEvent();
        }

        public void Release()
        {
            semaphoreSlim.Release();
            OnEvent();
        }

        public void Release(int count)
        {
            semaphoreSlim.Release(count);
            OnEvent();
        }

        private void OnEvent()
        {
            if (isUsed != default)
                PrepareCallBackAction(obj => isUsed(semaphoreSlim.CurrentCount != repoProcessingCount));
        }

        public async Task WaitAsync()
        {
            await semaphoreSlim.WaitAsync();
            OnEvent();
        }

        public int CurrentCount => semaphoreSlim.CurrentCount;
        public void Dispose() => semaphoreSlim.Dispose();
    }
}
