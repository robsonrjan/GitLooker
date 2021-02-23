using FluentAssertions;
using GitLooker.Core.Configuration;
using GitLooker.Services.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GitLooker.Unit.Test.GitLooker.Services.Services
{
    [TestFixture]
    public class AppConfigurationTest
    {
        private AppConfiguration testedObj;
        private string appConfigPileFullPath;

        [SetUp]
        public void BeforeEach()
        {
            testedObj = new AppConfiguration(Environment.CurrentDirectory);
            appConfigPileFullPath = $"{Environment.CurrentDirectory}\\GitLooker\\GitLookerConfig.json";
        }

        [TearDown]
        public void AfterEach()
        {
            var tempConfigFilePath = $"{Environment.CurrentDirectory}\\GitLooker";

            if (Directory.Exists(tempConfigFilePath))
                Directory.Delete(tempConfigFilePath, true);
        }

        [Test]
        public void Constructor_creates_empty_config_if_not_exists()
        {
            var appConfig = testedObj.FirstOrDefault();

            appConfig.Should().BeOfType<RepoConfig>();
            appConfig.As<RepoConfig>().ExpectedRemoteRepos.Should().HaveCount(0);
            appConfig.As<RepoConfig>().MainBranch.Should().Be("master");
            appConfig.As<RepoConfig>().ProjectArguments.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().ProjectCommand.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().ProjectExtension.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().GitLookerPath.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().Command.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().Arguments.Should().Be(string.Empty);
            appConfig.As<RepoConfig>().IntervalUpdateCheckHour.Should().Be(0);
        }

        [Test]
        public void SeveConfig_and_open_should_be_same()
        {
            SetNewAppConfig();
            testedObj.Save();
            IList<string> expectedrepos = new List<string> { "ExpectedRemoteRepos" };

            testedObj.Open();

            testedObj.FirstOrDefault().Arguments.Should().Be("Arguments");
            testedObj.FirstOrDefault().Command.Should().Be("Command");
            testedObj.FirstOrDefault().ExpectedRemoteRepos.Should().BeEquivalentTo(expectedrepos);
            testedObj.FirstOrDefault().GitLookerPath.Should().Be("GitLookerPath");
            testedObj.FirstOrDefault().IntervalUpdateCheckHour.Should().Be(999);
            testedObj.FirstOrDefault().MainBranch.Should().Be("MainBranch");
            testedObj.FirstOrDefault().ProjectArguments.Should().Be("ProjectArguments");
            testedObj.FirstOrDefault().ProjectCommand.Should().Be("ProjectCommand");
            testedObj.FirstOrDefault().ProjectExtension.Should().Be("ProjectExtension");
        }

        [Test]
        public void OpenCustomAppConfigFile()
        {
            var appCustomConfigPileFullPath = $"{Environment.CurrentDirectory}\\GitLooker\\GitLookerConfigTest.json";
            File.WriteAllText(appCustomConfigPileFullPath, JsonConvert.SerializeObject(CreateNewAppConfig()));
            IList<string> expectedrepos = new List<string> { "xExpectedRemoteRepos" };

            testedObj.Open(appCustomConfigPileFullPath);

            testedObj.FirstOrDefault().Arguments.Should().Be("xArguments");
            testedObj.FirstOrDefault().Command.Should().Be("xCommand");
            testedObj.FirstOrDefault().ExpectedRemoteRepos.Should().BeEquivalentTo(expectedrepos);
            testedObj.FirstOrDefault().GitLookerPath.Should().Be("xGitLookerPath");
            testedObj.FirstOrDefault().IntervalUpdateCheckHour.Should().Be(888);
            testedObj.FirstOrDefault().MainBranch.Should().Be("xMainBranch");
            testedObj.FirstOrDefault().ProjectArguments.Should().Be("xProjectArguments");
            testedObj.FirstOrDefault().ProjectCommand.Should().Be("xProjectCommand");
            testedObj.FirstOrDefault().ProjectExtension.Should().Be("xProjectExtension");
            testedObj.RepoProcessingCount.Should().Be(99);
        }

        private void SetNewAppConfig()
        {
            testedObj.FirstOrDefault().Arguments = "Arguments";
            testedObj.FirstOrDefault().Command = "Command";
            testedObj.FirstOrDefault().ExpectedRemoteRepos = new List<string> { "ExpectedRemoteRepos" };
            testedObj.FirstOrDefault().GitLookerPath = "GitLookerPath";
            testedObj.FirstOrDefault().IntervalUpdateCheckHour = 999;
            testedObj.FirstOrDefault().MainBranch = "MainBranch";
            testedObj.FirstOrDefault().ProjectArguments = "ProjectArguments";
            testedObj.FirstOrDefault().ProjectCommand = "ProjectCommand";
            testedObj.FirstOrDefault().ProjectExtension = "ProjectExtension";
        }

        private RepoConfig CreateNewAppConfig()
            => new RepoConfig
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
            };
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
