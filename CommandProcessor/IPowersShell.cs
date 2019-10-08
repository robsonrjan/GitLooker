using System.Collections.Generic;

namespace GitLooker
{
    public interface IPowersShell
    {
        IEnumerable<string> Execute(string command, bool closeConnectionAfter = true);
    }
}