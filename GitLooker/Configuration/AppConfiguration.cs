using GitLooker.Core.Configuration;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Forms;

namespace GitLooker.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        private const string appConfigFileName = "GitLookerConfig.json";
        private readonly string appConfigPath;
        private readonly string appConfigFullPath;
        private readonly AppConfig appConfig;

        public AppConfiguration()
        {
            var builder = new ConfigurationBuilder();
            var configDir = Application.StartupPath;
            appConfigPath = $"{configDir}\\Config";
            appConfigFullPath = $"{appConfigPath}\\{appConfigFileName}";

            if (!Directory.Exists(appConfigPath))
                Directory.CreateDirectory(appConfigPath);

            if (!File.Exists(appConfigFullPath))
                SaveConfig();

            appConfig = builder.AddJsonFile(appConfigFullPath).Build().Get<AppConfig>();
        }

        public string GitLookerPath
        {
            get => appConfig.GitLookerPath;
            set => appConfig.GitLookerPath = value;
        }

        public string MainBranch
        {
            get => appConfig.MainBranch ?? "master";
            set => appConfig.MainBranch = value;
        }

        public int RepoProcessingCount => appConfig.RepoProcessingCount;

        public int IntervalUpdateCheckHour
        {
            get => appConfig.IntervalUpdateCheckHour;
            set => appConfig.IntervalUpdateCheckHour = value;
        }

        public string Command
        {
            get => appConfig.Command;
            set => appConfig.Command = value;
        }

        public string Arguments
        {
            get => appConfig.Arguments;
            set => appConfig.Arguments = value;
        }

        public void Save() => SaveConfig();

        public IList<string> ExpectedRemoteRepos
        {
            get => appConfig.ExpectedRemoteRepos;
            set
            {
                appConfig.ExpectedRemoteRepos = value.ToList();
                SaveConfig();
            }
        }

        private void SaveConfig()
            => File.WriteAllText(appConfigFullPath, JsonConvert.SerializeObject(appConfig ?? new AppConfig()));
    }
}
