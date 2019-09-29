using System.Collections.ObjectModel;
using System.Management.Automation;

namespace GitLooker
{
    public interface IPowersShell
    {
        Collection<PSObject> Execute(string command, bool closeConnectionAfter = true);
    }
}