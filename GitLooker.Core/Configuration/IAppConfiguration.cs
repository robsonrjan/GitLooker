using System.Collections.Generic;

namespace GitLooker.Core.Configuration
{
    public interface IAppConfiguration
    {
        string Arguments { get; }
        string Command { get; }
        string GirLookerPath { get; }
        int IntervalUpdateCheckHour { get; }
        string MainBranch { get; }
        int RepoProcessingCount { get; }
        List<string> ExpectedRemoteRepos { get; set; }

        void Save(Dictionary<string, object> configuration);
    }
}
