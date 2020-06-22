using Castle.DynamicProxy;
using GitLooker.Core;
using GitLooker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitLooker.Services.interceptors
{
    public class SemaphoreInteractionInterceptor : IInterceptor
    {
        private readonly IAppSemaphoreSlim operationSemaphore;

        public SemaphoreInteractionInterceptor(IRepoControlConfiguration repoConfiguration) 
            => operationSemaphore = repoConfiguration?.Semaphore ?? throw new ArgumentNullException(nameof(repoConfiguration));

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
