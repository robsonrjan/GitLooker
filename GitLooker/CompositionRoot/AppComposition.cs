using Castle.DynamicProxy;
using GitLooker.Controls;
using GitLooker.Core;
using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using GitLooker.Core.Services;
using GitLooker.Core.Startup;
using GitLooker.Services.CommandProcessor;
using GitLooker.Services.Configuration;
using GitLooker.Services.interceptors;
using GitLooker.Services.Repository;
using GitLooker.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using ST = GitLooker.Startup;

namespace GitLooker.CompositionRoot
{
    public static class AppComposition
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            return services
                .AddSingleton<IStartup, ST.Startup>()
                .AddSingleton<IMainForm, MainForm>()
                .AddSingleton<IAppConfiguration, AppConfiguration>()
                .AddSingleton<IRepoHolder, RepoHolder>()
                .AddSingleton<IGitFileRepo, GitFileRepo>()
                .AddSingleton<IProjectFileRepo, ProjectFileRepo>()
                .AddSingleton<ITabsRepoBuilder, TabsRepoBuilder>()
                .AddSingleton<IAppSemaphoreSlim, AppSemaphoreSlim>()
                .AddTransient<IRepoControlConfiguration, RepoControlConfiguration>()
                .AddTransient<IPowersShell, PowersShell>()
                .AddTransient<IRepoCommandProcessor, RepoCommandProcessor>()
                .AddTransient<RepoCommandProcessorController>()
                .AddTransient<SemaphoreInteractionInterceptor>()
                .AddTransient<TabReposControl>()
                .AddTransient(service =>
                {
                    var myInterceptedClass = service.GetService<RepoCommandProcessorController>();
                    var interceptor = service.GetService<SemaphoreInteractionInterceptor>();
                    var proxy = new ProxyGenerator();
                    return proxy.CreateInterfaceProxyWithTargetInterface<IRepoCommandProcessorController>(myInterceptedClass, interceptor);
                })
                .AddTransient<RepoControl>();
        }
    }
}
