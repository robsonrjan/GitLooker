using GitLooker.Core.Startup;
using System.Windows.Forms;

namespace GitLooker.Startup
{
    public class Startup : IStartup
    {
        private readonly MainForm form;
        public Startup(MainForm form)
        {
            this.form = form;
        }

        public void StartApp(string[] arg)
        {
            Application.Run(form);
        }
    }
}
