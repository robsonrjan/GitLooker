using FluentAssertions;
using GitLooker.Core.Configuration;
using GitLooker.Core.Validators;
using GitLooker.Services.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.Services
{
    public class AppConfigurationTest : IDisposable
    {
        private  readonly AppConfiguration testedObj;
        private readonly string appConfigPileFullPath;
        private readonly IGitValidator gitValidator;

        public AppConfigurationTest()
        {
            gitValidator = Mock.Of<IGitValidator>();
            testedObj = new AppConfiguration(gitValidator, Environment.CurrentDirectory);
            appConfigPileFullPath = $"{Environment.CurrentDirectory}\\GitLooker\\GitLookerConfig.json";
        }

        [Fact]
        public void Constructor_creates_empty_config_if_not_exists()
        {
            var appConfig = testedObj.FirstOrDefault();

            appConfig.Should().BeNull();
            testedObj.RepoProcessingCount.Should().Be(3);
            testedObj.Version.Should().Be("1.0.2");
        }

        [Fact]
        public void OpenOldVersionAppConfigFileShouldBeToNewOne()
        {
            var appCustomConfigPileFullPath = $"{Environment.CurrentDirectory}\\GitLooker\\GitLookerConfigTest.json";
            if (File.Exists(appCustomConfigPileFullPath))
                File.Delete(appCustomConfigPileFullPath);
            File.WriteAllText(appCustomConfigPileFullPath, JsonConvert.SerializeObject(CreateNewAppConfig()));

            testedObj.Open(appCustomConfigPileFullPath);
            var appConfig = testedObj.FirstOrDefault();

            appConfig.Should().BeNull();
            testedObj.RepoProcessingCount.Should().Be(99);
            testedObj.Version.Should().Be("1.0.2");
        }

        private RepoConfigOldVer CreateNewAppConfig()
            => new RepoConfigOldVer
            {
                Arguments = "xArguments",
                Command = "xCommand",
                ExpectedRemoteRepos = new List<string> { "xExpectedRemoteRepos" },
                GitLookerPath = "xGitLookerPath",
                IntervalUpdateCheckHour = 888,
                MainBranch = "xMainBranch",
                ProjectArguments = "xProjectArguments",
                ProjectCommand = "xProjectCommand",
                ProjectExtension = "xProjectExtension",
                RepoProcessingCount = 99
            };

        public void Dispose()
        {
            var tempConfigFilePath = $"{Environment.CurrentDirectory}\\GitLooker";

            if (Directory.Exists(tempConfigFilePath))
                Directory.Delete(tempConfigFilePath, true);
        }
    }

    internal static class TestExtensions
    {
        internal static object GetValueOfPrivateField(this AppConfiguration configuration, string name)
        {
            var appConfigField = configuration.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return appConfigField.GetValue(configuration);
        }
    }
}
