using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IRepoCommandProcessor
    {
        IEnumerable<string> CheckRepo(string workingDir);
        IEnumerable<string> PullRepo(string workingDir);
        IEnumerable<string> ResetRepo(string workingDi);
        string RemoteConfig(string workingDir);
        IEnumerable<string> ClonRepo(string workingDir, string repoConfig);
    }
}
