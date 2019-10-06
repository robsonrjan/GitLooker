using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace GitLooker
{
    public interface IPowersShell
    {
        IEnumerable<string> Execute(string command, bool closeConnectionAfter = true);
    }
}