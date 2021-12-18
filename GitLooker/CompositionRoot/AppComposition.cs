using Castle.DynamicProxy;
using GitLooker.Controls;
using GitLooker.Core.Configuration;
using GitLooker.Core.Repository;
using GitLooker.Core.Services;
using GitLooker.Core.Startup;
using GitLooker.Core.Validators;
using GitLooker.Services.CommandProcessor;
using GitLooker.Services.Configuration;
using GitLooker.Services.interceptors;
using GitLooker.Services.Repository;
using GitLooker.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using ST = GitLooker.Startup;

namespace GitLooker.CompositionRoot
{
    public static class AppComposition
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            return services
                .AddSingleton<IStartup, ST.Startup>()
                .AddSingleton<MainForm>()
                .AddSingleton<IAppConfiguration, AppConfiguration>()
                .AddSingleton<IRepoHolder, RepoHolder>()
                .AddSingleton<IGitFileRepo, GitFileRepo>()
                .AddSingleton<IProjectFileRepo, ProjectFileRepo>()
                .AddSingleton<ITabsRepoBuilder, TabsRepoBuilder>()
                .AddSingleton<IAppSemaphoreSlim, AppSemaphoreSlim>()
                .AddSingleton<IGitVersion, RepoCommandProcessor>()
                .AddSingleton<ILoggerFactory>(sp =>
                {
                    var eventSetting = new EventLogSettings
                    {
                        Filter = (msg, level) => level > LogLevel.Debug,
                        SourceName = "GitLooker"
                    };
                    var provider = new EventLogLoggerProvider(eventSetting);
                    return new LoggerFactory(new[] { provider });
                })
                .AddLogging()
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton<IGitValidator, GitValidator>()
                .AddTransient<IProcessShell, ProcessShell>()
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
