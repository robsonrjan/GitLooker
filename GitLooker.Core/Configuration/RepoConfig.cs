using System.Runtime.Serialization;

namespace GitLooker.Core.Configuration
{
    [DataContract]
    public class RepoConfig
    {
        [DataMember]
        public string GitLookerPath { get; set; }
        [DataMember]
        public int IntervalUpdateCheckHour { get; set; }
        [DataMember]
        public string MainBranch { get; set; } = "master";
        [DataMember]
        public string Command { get; set; }
        [DataMember]
        public string Arguments { get; set; }
        [DataMember]
        public List<string> ExpectedRemoteRepos { get; set; } = Enumerable.Empty<string>().ToList();
        [DataMember]
        public string ProjectCommand { get; set; }
        [DataMember]
        public string ProjectArguments { get; set; }
        [DataMember]
        public string ProjectExtension { get; set; }
    }
}
