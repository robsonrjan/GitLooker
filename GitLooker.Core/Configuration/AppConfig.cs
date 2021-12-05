using System.Runtime.Serialization;

namespace GitLooker.Core.Configuration
{
    [DataContract]
    public class AppConfig
    {
        public const string CurrentVersion = "1.0.2";
        [DataMember]
        public string Version { get; set; } = CurrentVersion;
        [DataMember]
        public int RepoProcessingCount { get; set; } = 3;
        [DataMember]
        public string GitLocation { get; set; }
        [DataMember]
        public List<RepoConfig> RepoConfigs { get; set; }
    }
}
