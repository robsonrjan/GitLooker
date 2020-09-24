using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitLooker.Core.Configuration
{
    public class AppConfig
    {
        public string GitLookerPath { get; set; }
        public int RepoProcessingCount { get; set; } = 3;
        public int IntervalUpdateCheckHour { get; set; }
        public string MainBranch { get; set; } = "master";
        public string Command { get; set; }
        public string Arguments { get; set; }
        public List<string> ExpectedRemoteRepos { get; set; } = Enumerable.Empty<string>().ToList();
    }
}
