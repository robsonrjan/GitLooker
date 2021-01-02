using GitLooker.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitLooker.Configuration
{
    public class AppConfiguration : IAppConfiguration
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

        public virtual void Open(string configFile = default)
        {
            var builder = new ConfigurationBuilder();
            appConfig = builder.AddJsonFile(configFile ?? appConfigFullPath).Build().Get<AppConfig>();
        }

        public virtual string GitLookerPath
        {
            get => appConfig.GitLookerPath;
            set => appConfig.GitLookerPath = value;
        }

        public virtual string MainBranch
        {
            get => appConfig.MainBranch ?? "master";
            set => appConfig.MainBranch = value;
        }

        public virtual int RepoProcessingCount => appConfig.RepoProcessingCount;

        public virtual int IntervalUpdateCheckHour
        {
            get => appConfig.IntervalUpdateCheckHour;
            set => appConfig.IntervalUpdateCheckHour = value;
        }

        public virtual string Command
        {
            get => appConfig.Command;
            set => appConfig.Command = value;
        }

        public virtual string Arguments
        {
            get => appConfig.Arguments;
            set => appConfig.Arguments = value;
        }

        public virtual void Save() => SaveConfig();

        public virtual IList<string> ExpectedRemoteRepos
        {
            get => appConfig.ExpectedRemoteRepos;
            set
            {
                appConfig.ExpectedRemoteRepos = value.ToList();
                SaveConfig();
            }
        }

        public virtual string ProjectCommand
        {
            get => appConfig.ProjectCommand;
            set => appConfig.ProjectCommand = value;
        }

        public virtual string ProjectArguments
        {
            get => appConfig.ProjectArguments;
            set => appConfig.ProjectArguments = value;
        }

        public virtual string ProjectExtension
        {
            get => appConfig.ProjectExtension;
            set => appConfig.ProjectExtension = value;
        }

        private void SaveConfig(string configFile = default)
            => File.WriteAllText(configFile ?? appConfigFullPath, JsonConvert.SerializeObject(appConfig ?? new AppConfig()));

        public virtual void SaveAs(string configFile) => SaveConfig(configFile);
    }
}
