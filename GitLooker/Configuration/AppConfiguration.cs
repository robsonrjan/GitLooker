using GitLooker.Core.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Json;

namespace GitLooker.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        private const int maxPandingGitOperations = 3;
        private const string repoFileConfigurationName = "repos.json";

        public string GirLookerPath => ConfigurationManager.AppSettings["GirLookerPath"];
        public int RepoProcessingCount
        {
            get
            {
                int repoProcessingCount;
                if (!int.TryParse(ConfigurationManager.AppSettings["repoProcessingCount"], out repoProcessingCount))
                    repoProcessingCount = maxPandingGitOperations;
                return repoProcessingCount;
            }
        }
        public string MainBranch => ConfigurationManager.AppSettings["mainBranch"] ?? "master";
        public int IntervalUpdateCheckHour
        {
            get
            {
                int intervalUpdateCheckHour;
                if (!int.TryParse(ConfigurationManager.AppSettings["intervalUpdateCheckHour"], out intervalUpdateCheckHour))
                    intervalUpdateCheckHour = 0;
                return intervalUpdateCheckHour;
            }
        }
        public string Command => ConfigurationManager.AppSettings["command"];
        public string Arguments => ConfigurationManager.AppSettings["arguments"];
        public void Save(Dictionary<string, object> configuration)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var configItem in configuration ?? new Dictionary<string, object>())
            {
                config.AppSettings.Settings.Remove(configItem.Key);
                config.AppSettings.Settings.Add(configItem.Key, configItem.Value.ToString());
            }
            config.Save();
        }
        public List<string> ExpectedRemoteRepos
        {
            get
            {
                var expectedRemoteList = new List<string>();

                if (File.Exists(repoFileConfigurationName))
                {
                    var jsonserializer = new DataContractJsonSerializer(typeof(List<string>));
                    using (var stream = File.OpenRead(repoFileConfigurationName))
                        expectedRemoteList = (List<string>)jsonserializer.ReadObject(stream);
                }

                return expectedRemoteList;
            }
            set
            {
                if (File.Exists(repoFileConfigurationName))
                    File.Delete(repoFileConfigurationName);

                var jsonserializer = new DataContractJsonSerializer(typeof(List<string>));

                using (var stream = File.OpenWrite(repoFileConfigurationName))
                    jsonserializer.WriteObject(stream, value);
            }
        }
    }
}
