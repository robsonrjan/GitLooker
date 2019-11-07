using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitLooker
{
    public class AppSemaphoreSlim : IAppSemaphoreSlim
    {
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly int repoProcessingCount;
        public delegate void onSemaphoreChange(bool isProcessing);
        private onSemaphoreChange isUsed;
        public onSemaphoreChange OnUse { get => isUsed; set => isUsed = value; }

        public AppSemaphoreSlim(int repoProcessingCount)
        {
            this.repoProcessingCount = repoProcessingCount;
            semaphoreSlim = new SemaphoreSlim(repoProcessingCount);
        }

        public void Wait()
        {
            semaphoreSlim.Wait();
            if (isUsed != default(onSemaphoreChange))
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public void Release()
        {
            semaphoreSlim.Release();
            if (isUsed != default(onSemaphoreChange))
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public void Release(int count)
        {
            semaphoreSlim.Release(count);
            if (isUsed != default(onSemaphoreChange))
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public async Task WaitAsync()
        {
            await semaphoreSlim.WaitAsync();
            if (isUsed != default(onSemaphoreChange))
                isUsed(semaphoreSlim.CurrentCount == repoProcessingCount);
        }

        public int CurrentCount => semaphoreSlim.CurrentCount;

        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }
    }
}
