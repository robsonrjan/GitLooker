using GitLooker.Core.Configuration;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace GitLooker.Configuration
{
    public sealed class AppConfiguration : IAppConfiguration
    {
        private const string appConfigFileName = "GitLookerConfig.json";
        private readonly string appConfigPath;
        private readonly string appConfigFullPath;
        private AppConfig appConfig;

        public AppConfiguration()
        {
            var configDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            appConfigPath = $"{configDir}\\GitLooker";
            appConfigFullPath = $"{appConfigPath}\\{appConfigFileName}";

            if (!Directory.Exists(appConfigPath))
                Directory.CreateDirectory(appConfigPath);

            if (!File.Exists(appConfigFullPath))
                SaveConfig();

            Open();
        }

        public void Open(string configFile = default)
        {
            var builder = new ConfigurationBuilder();
            appConfig = builder.AddJsonFile(configFile ?? appConfigFullPath).Build().Get<AppConfig>();
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

        private void SaveConfig(string configFile = default)
            => File.WriteAllText(configFile ?? appConfigFullPath, JsonConvert.SerializeObject(appConfig ?? new AppConfig()));

        public void SaveAs(string configFile) => SaveConfig(configFile);
    }
}
