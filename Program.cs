using GitLooker.CompositionRoot;
using GitLooker.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace GitLooker
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arg)
        {
            BeforeStart();

            IServiceCollection servises = new ServiceCollection();
            servises.AddApp();

            using (var servisesProvider = servises.BuildServiceProvider())
            {
                var appService = servisesProvider.GetService<IAppService>();
                appService.StartApp(arg);
            }
        }

        private static void BeforeStart()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
