using System.Collections.Generic;

namespace GitLooker.Core.Configuration
{
    public interface IAppConfiguration
    {
        string Arguments { get; set; }
        string Command { get; set; }
        string GitLookerPath { get; set; }
        int IntervalUpdateCheckHour { get; set; }
        string MainBranch { get; set; }
        int RepoProcessingCount { get; }
        IList<string> ExpectedRemoteRepos { get; set; }
        void Save();
    }
}
