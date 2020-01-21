using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IPowersShell
    {
        IEnumerable<string> Execute(string command, bool closeConnectionAfter = true);
    }
}
