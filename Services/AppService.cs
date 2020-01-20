using GitLooker.Core.Services;
using System.Windows.Forms;

namespace GitLooker.Services
{
    public class AppService : IAppService
    {
        private Form1 form;
        public AppService(Form1 form)
        {
            this.form = form;
        }

        public void StartApp(string[] arg)
        {
            Application.Run(form);
        }
    }
}
