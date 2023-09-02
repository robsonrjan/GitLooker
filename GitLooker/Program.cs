using GitLooker.CompositionRoot;
using GitLooker.Core.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace GitLooker
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arg)
        {
            try
            {
                BeforeStart();

                IServiceCollection services = new ServiceCollection();
                services.AddApp();

                using var servicesProvider = services.BuildServiceProvider();
                servicesProvider?.GetRequiredService<IStartup>()
                    .StartApp(arg);
            }
            catch (Exception ex)
            {
                File.WriteAllText("LogError.log", ex.ToString());
            }
        }

        private static void BeforeStart()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
