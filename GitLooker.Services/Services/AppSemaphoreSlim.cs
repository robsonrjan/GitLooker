using GitLooker.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GitLooker.Services.Services
{
    public class AppSemaphoreSlim : IAppSemaphoreSlim
    {
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly int repoProcessingCount;
        private Action<bool> isUsed;
        public Action<bool> OnUse { get => isUsed; set => isUsed = value; }
        public int MaxRepoProcessingCount => repoProcessingCount;

        public AppSemaphoreSlim(int repoProcessingCount)
        {
            this.repoProcessingCount = repoProcessingCount;
            semaphoreSlim = new SemaphoreSlim(repoProcessingCount);
        }

        public void Wait()
        {
            semaphoreSlim.Wait();
            if (isUsed != default)
                isUsed(semaphoreSlim.CurrentCount != repoProcessingCount);
        }

        public void Release()
        {
            semaphoreSlim.Release();
            if (isUsed != default)
                isUsed(semaphoreSlim.CurrentCount != repoProcessingCount);
        }

        public void Release(int count)
        {
            semaphoreSlim.Release(count);
            if (isUsed != default)
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public async Task WaitAsync()
        {
            await semaphoreSlim.WaitAsync();
            if (isUsed != default)
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public int CurrentCount => semaphoreSlim.CurrentCount;
        public void Dispose() => semaphoreSlim.Dispose();
    }
}
