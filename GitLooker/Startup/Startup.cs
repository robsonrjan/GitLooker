using GitLooker.Core.Startup;
using GitLooker.Services.Services;

namespace GitLooker.Startup
{
    public class Startup : IStartup
    {
        private readonly MainForm form;
        private readonly ILoggingService<Startup> loggingService;

        public Startup(MainForm form, ILoggingService<Startup> loggingService)
        {
            this.form = form;
            this.loggingService = loggingService;
        }

        public void StartApp(string[] arg)
        {
            loggingService.Info($"[{nameof(StartApp)}] Starting App");
            Application.Run(form);
            loggingService.Info($"[{nameof(StartApp)}] Ending App");
        }
    }
}
