using Castle.DynamicProxy;
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
                .AddSingleton<IAppConfiguration, AppConfiguration>()
                .AddSingleton<IRepoHolder, RepoHolder>()
                .AddSingleton<IGitFileRepo, GitFileRepo>()
                .AddSingleton<IProjectFileRepo, ProjectFileRepo>()
                .AddSingleton<IAppSemaphoreSlim>(service =>
                {
                    var config = service.GetService<IAppConfiguration>();
                    return new AppSemaphoreSlim(config.RepoProcessingCount);
                })
                .AddTransient<IRepoControlConfiguration>(service =>
                {
                    var repoConfig = service.GetService<IAppConfiguration>();
                    var mainForm = service.GetService<IMainForm>();
                    var semaphore = service.GetService<IAppSemaphoreSlim>();
                    return new RepoControlConfiguration(mainForm.CurrentRepoDdir, semaphore, mainForm.CurrentNewRepo, repoConfig.MainBranch);
                })
                .AddTransient<IPowersShell, PowersShell>()
                .AddTransient<IRepoCommandProcessor, RepoCommandProcessor>()
                .AddTransient<RepoCommandProcessorController>()
                .AddTransient<SemaphoreInteractionInterceptor>()
                .AddTransient(service =>
                {
                    var myInterceptedClass = service.GetService<RepoCommandProcessorController>();
                    var interceptor = service.GetService<SemaphoreInteractionInterceptor>();
                    var proxy = new ProxyGenerator();
                    return proxy.CreateInterfaceProxyWithTargetInterface<IRepoCommandProcessorController>(myInterceptedClass, interceptor);
                })
                .AddTransient<RepoControl>(service =>
                {
                    var mainForm = service.GetService<IMainForm>();
                    var repoConfig = service.GetService<IRepoControlConfiguration>();
                    var commandProcessor = service.GetService<IRepoCommandProcessorController>();
                    var repoHolder = service.GetService<IRepoHolder>();
                    return new RepoControl(repoConfig, commandProcessor, mainForm.EndControl, repoHolder);
                })
                .AddSingleton<MainForm>()
                .AddSingleton<IMainForm>(sp => sp.GetService<MainForm>());
        }
    }
}
