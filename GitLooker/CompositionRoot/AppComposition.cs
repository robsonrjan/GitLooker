using GitLooker.Configuration;
using GitLooker.Core.Configuration;
using GitLooker.Core.Services;
using GitLooker.Services;
using GitLooker.Services.CommandProcessor;
using GitLooker.Services.Configuration;
using GitLooker.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GitLooker.CompositionRoot
{
    public static class AppComposition
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAppService, AppService>()
                .AddSingleton<IAppConfiguration, AppConfiguration>()
                .AddSingleton<IAppSemaphoreSlim>(service =>
                {
                    var config = service.GetService<IAppConfiguration>();
                    return new AppSemaphoreSlim(config.RepoProcessingCount);
                })
                .AddTransient<IRepoControlConfiguration>(service =>
                {
                    var repoConfig = service.GetService<IAppConfiguration>();
                    var mainForm = service.GetService<Form1>();
                    var semaphore = service.GetService<IAppSemaphoreSlim>();

                    return new RepoControlConfiguration(mainForm.CurrentRepoDdir, semaphore, mainForm.CurrentNewRepo, repoConfig.MainBranch);
                })
                .AddTransient<IPowersShell, PowersShell>()
                .AddTransient<IRepoCommandProcessor, RepoCommandProcessor>()
                .AddTransient<RepoControl>(service =>
                {
                    var mainForm = service.GetService<Form1>();
                    var repoConfig = service.GetService<IRepoControlConfiguration>();
                    var commandProcessor = service.GetService<IRepoCommandProcessor>();
                    return new RepoControl(repoConfig, commandProcessor, mainForm.EndControl);
                })
                .AddSingleton<Form1>();
        }
    }
}
