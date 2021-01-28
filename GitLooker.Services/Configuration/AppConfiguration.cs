using GitLooker.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitLooker.Services.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        private const string appConfigFileName = "GitLookerConfig.json";
        private readonly string appConfigPath;
        private readonly string appConfigFullPath;
        private AppConfig appConfig;

        public AppConfiguration(string configFilePath = default)
        {
            var configDir = configFilePath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            appConfigPath = $"{configDir}\\GitLooker";
            appConfigFullPath = $"{appConfigPath}\\{appConfigFileName}";

            if (!Directory.Exists(appConfigPath))
                Directory.CreateDirectory(appConfigPath);

            if (!File.Exists(appConfigFullPath))
                SaveConfig();

            Open();
        }

        public int CurrentIndex { get; set; } = 0;

        public RepoConfig this[int index]
        {
            get => appConfig.RepoConfigs[index];
            set => appConfig.RepoConfigs[index] = value;
        }

        public virtual void Open(string configFile = default)
        {
            try
            {
                var configText = File.ReadAllText(configFile ?? appConfigFullPath);

                if (!CheckForOldConfigAndConvertToNew(configText))
                    appConfig = JsonConvert.DeserializeObject<AppConfig>(configText);
            }
            catch (Exception)
            {
                SaveConfig();
                throw;
            }
        }

        public virtual string GitLookerPath
        {
            get => appConfig.RepoConfigs[CurrentIndex].GitLookerPath;
            set => appConfig.RepoConfigs[CurrentIndex].GitLookerPath = value;
        }

        public virtual string MainBranch
        {
            get => appConfig.RepoConfigs[CurrentIndex].MainBranch ?? "master";
            set => appConfig.RepoConfigs[CurrentIndex].MainBranch = value;
        }

        public virtual int RepoProcessingCount => appConfig.RepoProcessingCount;

        public virtual int IntervalUpdateCheckHour
        {
            get => appConfig.RepoConfigs[CurrentIndex].IntervalUpdateCheckHour;
            set => appConfig.RepoConfigs[CurrentIndex].IntervalUpdateCheckHour = value;
        }

        public virtual string Command
        {
            get => appConfig.RepoConfigs[CurrentIndex].Command;
            set => appConfig.RepoConfigs[CurrentIndex].Command = value;
        }

        public virtual string Arguments
        {
            get => appConfig.RepoConfigs[CurrentIndex].Arguments;
            set => appConfig.RepoConfigs[CurrentIndex].Arguments = value;
        }

        public virtual void Save() => SaveConfig();

        public virtual List<string> ExpectedRemoteRepos
        {
            get => appConfig.RepoConfigs[CurrentIndex].ExpectedRemoteRepos;
            set
            {
                appConfig.RepoConfigs[CurrentIndex].ExpectedRemoteRepos = value.ToList();
                SaveConfig();
            }
        }

        public virtual string ProjectCommand
        {
            get => appConfig.RepoConfigs[CurrentIndex].ProjectCommand;
            set => appConfig.RepoConfigs[CurrentIndex].ProjectCommand = value;
        }

        public virtual string ProjectArguments
        {
            get => appConfig.RepoConfigs[CurrentIndex].ProjectArguments;
            set => appConfig.RepoConfigs[CurrentIndex].ProjectArguments = value;
        }

        public virtual string ProjectExtension
        {
            get => appConfig.RepoConfigs[CurrentIndex].ProjectExtension;
            set => appConfig.RepoConfigs[CurrentIndex].ProjectExtension = value;
        }

        public virtual void SaveAs(string configFile) => SaveConfig(configFile);

        private bool CheckForOldConfigAndConvertToNew(string configText)
        {
            RepoConfig oldConfig;
            if (!configText.StartsWith("{\"Version\":\"1.0.1\""))
            {
                oldConfig = JsonConvert.DeserializeObject<RepoConfig>(configText);
                appConfig = new AppConfig { RepoConfigs = new List<RepoConfig> { oldConfig } };
                SaveConfig();
                return true;
            }
            else
                return false;
        }

        private void SaveConfig(string configFile = default)
            => File.WriteAllText(configFile ?? appConfigFullPath, JsonConvert.SerializeObject(appConfig
                ?? new AppConfig { RepoConfigs = new List<RepoConfig> { new RepoConfig() } }));

        public IEnumerator<RepoConfig> GetEnumerator() => appConfig.RepoConfigs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => appConfig.RepoConfigs.GetEnumerator();
    }
}
