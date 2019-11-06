using System.Collections.Generic;

namespace GitLooker.CommandProcessor
{
    public interface IRepoCommandProcessor
    {
        IEnumerable<string> CheckRepo(string workingDir);
        IEnumerable<string> PullRepo(string workingDir);
        IEnumerable<string> ResetRepo(string workingDi);
        string RemoteConfig(string workingDir);
    }
}