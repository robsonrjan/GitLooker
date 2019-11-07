using System;
using System.Threading.Tasks;
using static GitLooker.AppSemaphoreSlim;

namespace GitLooker
{
    public interface IAppSemaphoreSlim : IDisposable
    {
        void Release();
        void Wait();
        onSemaphoreChange OnUse { get; set; }
        Task WaitAsync();
        int CurrentCount { get; }
        void Release(int count);
    }
}