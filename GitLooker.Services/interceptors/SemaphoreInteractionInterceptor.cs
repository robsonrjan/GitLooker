using Castle.DynamicProxy;
using GitLooker.Core;
using GitLooker.Core.Services;

namespace GitLooker.Services.interceptors
{
    public class SemaphoreInteractionInterceptor : IInterceptor
    {
        private readonly IAppSemaphoreSlim operationSemaphore;

        public SemaphoreInteractionInterceptor(IAppSemaphoreSlim appSemaphoreSlim)
            => operationSemaphore = appSemaphoreSlim ?? throw new ArgumentNullException(nameof(appSemaphoreSlim));

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name == nameof(Execute))
                Execute(invocation);
            else
                invocation.Proceed();
        }

        private void Execute(IInvocation invocation)
        {
            operationSemaphore.Wait();
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                invocation.ReturnValue = new AppResult<IEnumerable<string>>(ex);
            }
            finally
            {
                operationSemaphore.Release();
            }
        }
    }
}
