using GitLooker.CompositionRoot;
using GitLooker.Core.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace GitLooker
{
    static class Program
    {
        [STAThread]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "<Pending>")]
        static void Main(string[] arg)
        {
            IStartup? app;
            try
            {
                BeforeStart();

                IServiceCollection servises = new ServiceCollection();
                servises.AddApp();

                using var servisesProvider = servises.BuildServiceProvider();
                if ((app = servisesProvider?.GetService<IStartup>()) == null) throw new ArgumentNullException(nameof(servisesProvider));
                app.StartApp(arg);
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
