namespace GitLooker.Core.Services
{
    public interface IAppSemaphoreSlim : IDisposable
    {
        void Release();
        void Wait();
        Action<bool> OnUse { get; set; }
        Task WaitAsync();
        int CurrentCount { get; }
        void Release(int count);
        int MaxRepoProcessingCount { get; }
    }
}
