using System.Windows.Forms;

namespace GitLooker.Core
{
    public interface IMainForm
    {
        string CurrentNewRepo { get; set; }
        string CurrentRepoDdir { get; set; }
        Control EndControl { get; }
    }
}