using System.Collections.Generic;

namespace GitLooker.Core.Configuration
{
    public interface IAppConfiguration : IEnumerable<RepoConfig>
    {
        int RepoProcessingCount { get; }
        void Add(RepoConfig config);
        IEnumerator<RepoConfig> GetEnumerator();
        void Open(string configFile = null);
        void Save();
        void SaveAs(string configFile);
        void Remove(RepoConfig config);
        string Version { get; }
        string GitVersion { get; set; }
    }
}
