using GitLooker.CompositionRoot;
using GitLooker.Core.Startup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows.Forms;

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

                IServiceCollection servises = new ServiceCollection();
                servises.AddApp();

                using var servisesProvider = servises.BuildServiceProvider();
                servisesProvider.GetService<IStartup>().StartApp(arg);
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
