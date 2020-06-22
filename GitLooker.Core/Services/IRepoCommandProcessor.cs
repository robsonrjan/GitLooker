using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IRepoCommandProcessor
    {
        AppResult<IEnumerable<string>> CheckRepo(string workingDir);
        AppResult<IEnumerable<string>> PullRepo(string workingDir);
        AppResult<IEnumerable<string>> ResetRepo(string workingDi);
        AppResult<IEnumerable<string>> RemoteConfig(string workingDir);
        AppResult<IEnumerable<string>> ClonRepo(string workingDir, string repoConfig);
        AppResult<IEnumerable<string>> CheckOutBranch(string workingDir, string branch);
    }
}
