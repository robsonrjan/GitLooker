using GitLooker.Core.Startup;
using GitLooker.Services.Services;
using Microsoft.Extensions.Logging;

namespace GitLooker.Startup
{
    public class Startup : IStartup
    {
        private readonly MainForm form;
        private readonly ILogger<Startup> loggingService;

        public Startup(MainForm form, ILogger<Startup> loggingService)
        {
            this.form = form;
            this.loggingService = loggingService;
        }

        public void StartApp(string[] arg)
        {
            loggingService.LogInformation($"[{nameof(StartApp)}] Starting App");
            Application.Run(form);
            loggingService.LogInformation($"[{nameof(StartApp)}] Ending App");
        }
    }
}
