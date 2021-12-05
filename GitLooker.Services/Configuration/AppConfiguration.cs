using GitLooker.Core.Configuration;
using GitLooker.Core.Validators;
using Newtonsoft.Json;
using System.Collections;

namespace GitLooker.Services.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        private const string appConfigFileName = "GitLookerConfig.json";
        private readonly string appConfigPath;
        private readonly string appConfigFullPath;
        private readonly IGitValidator gitValidator;

        private AppConfig appConfig;

        public static string Location { get; private set; }

        public AppConfiguration(IGitValidator gitValidator, string configFilePath = default)
        {
            var configDir = configFilePath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            appConfigPath = $"{configDir}\\GitLooker";
            appConfigFullPath = $"{appConfigPath}\\{appConfigFileName}";

            Location = appConfigFullPath;
            this.gitValidator = gitValidator;

            if (!Directory.Exists(appConfigPath))
                Directory.CreateDirectory(appConfigPath);

            if (!File.Exists(appConfigFullPath))
                SaveConfig();

            Open();
        }

        public virtual void Open(string configFile = default)
        {
            try
            {
                var configText = File.ReadAllText(configFile ?? appConfigFullPath);

                if (!CheckForOldConfigAndConvertToNew(configText))
                    appConfig = JsonConvert.DeserializeObject<AppConfig>(configText);

                if (!appConfig.Version.Equals(AppConfig.CurrentVersion))
                {
                    appConfig.Version = AppConfig.CurrentVersion;
                    SaveConfig();
                }
            }
            catch (Exception)
            {
                SaveConfig();
                throw;
            }
            Validate();

            if (gitValidator.TryToFind(appConfig.GitLocation, out var gitInfo))
            {
                if (string.IsNullOrWhiteSpace(appConfig.GitLocation))
                {
                    appConfig.GitLocation = gitInfo.Executable;
                    SaveConfig();
                }
                GitVersion = gitInfo.Version;
            }
            else
            {
                appConfig.GitLocation = default;
                SaveConfig();
            }
        }

        public string Version => appConfig.Version;

        public virtual int RepoProcessingCount => appConfig.RepoProcessingCount;

        public string GitVersion { get; set; }

        public virtual void Save() => SaveConfig();

        public virtual void SaveAs(string configFile) => SaveConfig(configFile);

        public void Remove(RepoConfig config)
            => appConfig.RepoConfigs.Remove(config);

        public void Add(RepoConfig config)
            => appConfig.RepoConfigs.Add(config);

        public IEnumerator<RepoConfig> GetEnumerator() => appConfig.RepoConfigs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => appConfig.RepoConfigs.GetEnumerator();

        private bool CheckForOldConfigAndConvertToNew(string configText)
        {
            RepoConfigOldVer oldConfig;
            if (!configText.Contains("\"Version\""))
            {
                oldConfig = JsonConvert.DeserializeObject<RepoConfigOldVer>(configText);
                appConfig = new AppConfig { RepoConfigs = new List<RepoConfig> { oldConfig } };
                appConfig.RepoProcessingCount = oldConfig.RepoProcessingCount;
                SaveConfig();
                return true;
            }
            else
                return false;
        }

        private void SaveConfig(string configFile = default)
            => File.WriteAllText(configFile ?? appConfigFullPath, JsonConvert.SerializeObject(appConfig
                ?? new AppConfig { RepoConfigs = new List<RepoConfig> { new RepoConfig() } }));

        private void Validate()
            => appConfig.RepoConfigs.RemoveAll(r => !Directory.Exists(r.GitLookerPath));
    }
}
