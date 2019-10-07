using System.Collections.Generic;

namespace GitLooker.CommandProcessor
{
    public interface ICommandProcessor
    {
        IEnumerable<string> CheckRepo(string workingDir);
        IEnumerable<string> PullRepo(string workingDir);
        IEnumerable<string> ResetRepo(string workingDi);
    }
}