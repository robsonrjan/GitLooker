using System.Windows.Forms;

namespace GitLooker.Core
{
    public interface IMainForm
    {
        string CurrentNewRepo { get; }
        string CurrentRepoDdir { get; }
        Panel EndControl { get; }
    }
}