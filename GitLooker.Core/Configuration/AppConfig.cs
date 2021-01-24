using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitLooker.Core.Configuration
{
    [DataContract]
    public class AppConfig
    {
        [DataMember]
        public string Version { get; set; } = "1.0.1";
        [DataMember]
        public int RepoProcessingCount { get; set; } = 3;
        [DataMember]
        public List<RepoConfig> RepoConfigs { get; set; }
    }
}
