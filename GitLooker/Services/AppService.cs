using GitLooker.Core.Services;
using System.Windows.Forms;

namespace GitLooker.Services
{
    public class AppService : IAppService
    {
        private readonly MainForm form;
        public AppService(MainForm form)
        {
            this.form = form;
        }

        public void StartApp(string[] arg)
        {
            Application.Run(form);
        }
    }
}
