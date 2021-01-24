using GitLooker.Core;
using GitLooker.Core.Startup;
using System.Windows.Forms;

namespace GitLooker.Startup
{
    public class Startup : IStartup
    {
        private readonly MainForm form;
        public Startup(IMainForm form)
        {
            this.form = form as MainForm;
        }

        public void StartApp(string[] arg)
        {
            Application.Run(form);
        }
    }
}
