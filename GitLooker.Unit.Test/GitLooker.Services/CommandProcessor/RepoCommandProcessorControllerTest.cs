using NUnit.Framework;
using FluentAssertions;
using GitLooker.Services.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using GitLooker.Services.Repository;
using System.IO;
using System.Linq;
using GitLooker.Services.interceptors;
using GitLooker.Core.Services;
using Moq;
using System.Reflection;
using System;
using System.Diagnostics;
using GitLooker.Core;
using GitLooker.Services.CommandProcessor;

namespace GitLooker.Unit.Test.GitLooker.Services.CommandProcessor
{
    [TestFixture]
    public class RepoCommandProcessorControllerTest
    {
        private RepoCommandProcessorController repoCommandProcessorController;
        private IRepoCommandProcessor repoCommandProcessor;
        private IRepoHolder repoHolder;
        private static bool hasBeenExecuted;
        private IEnumerable<MethodInfo> commands;
        private List<String> holderRepoList;

        [SetUp]
        public void BeforeEach()
        {
            holderRepoList = new List<string>();
            repoCommandProcessor = new RepoCommandProcessorForTest();
            repoHolder = Mock.Of<IRepoHolder>();
            Mock.Get(repoHolder).Setup(r => r.RepoRemoteList).Returns(holderRepoList);
            repoCommandProcessorController = new RepoCommandProcessorController(repoCommandProcessor, repoHolder);
            commands = repoCommandProcessorController.CommonCommandActions;
            hasBeenExecuted = default;
        }

        [TestCase("CheckOutBranch", 0, null)]
        [TestCase("CheckRepo", 0, null)]
        [TestCase("ClonRepo", 0, null)]
        [TestCase("PullRepo", 0, null)]
        [TestCase("RemoteConfig", 1, "remoteconfig")]
        [TestCase("ResetRepo", 0, null)]
        public void Execute_check_all_execution(string commandName, int reposCount, object specialValue)
        {
            bool hasInvoked = default;
            Action checkInvokation = () => hasInvoked = true;
            IEnumerable<string> options = new[] { default(string), default(string), default(string) };
            var commandForTest = commands.Where(c => c.Name == commandName);

            var result = repoCommandProcessorController.Execute(commandForTest, options, checkInvokation);

            hasBeenExecuted.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().Be(specialValue);
            result.Value.Should().BeEquivalentTo(new[] { new[] { commandName } });
            holderRepoList.Should().HaveCount(reposCount);
        }

        [Test]
        public void Constructor_for_parameter_repoCommandProcessor_isNull_throwException()
        {
            Action actionCheck = () => new RepoCommandProcessorController(default, repoHolder);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(repoCommandProcessor));
        }

        [Test]
        public void Constructor_for_parameter_repoHolder_isNull_throwException()
        {
            IRepoHolder repoHolder = default;
            Action actionCheck = () => new RepoCommandProcessorController(repoCommandProcessor, repoHolder);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(repoHolder));
        }

        [Test]
        public void Execute_for_parameter_commands_isNull_throwException()
        {
            var commands = default(IEnumerable<MethodInfo>);
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, default, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(commands));
        }

        [Test]
        public void Execute_for_parameter_commands_isEmpty_throwException()
        {
            var commands = Enumerable.Empty<MethodInfo>();
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, default, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(commands));
        }

        [Test]
        public void Execute_for_parameter_options_isNull_throwException()
        {
            var options = default(IEnumerable<string>);
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, options, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(options));
        }

        [Test]
        public void Execute_for_parameter_options_isEmpty_throwException()
        {
            var options = Enumerable.Empty<string>();
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, options, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(options));
        }

        private class RepoCommandProcessorForTest : IRepoCommandProcessor
        {
            public AppResult<IEnumerable<string>> CheckOutBranch(string workingDir, string branch)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(CheckOutBranch) }); ;
            }

            public AppResult<IEnumerable<string>> CheckRepo(string workingDir)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(CheckRepo) }); ;
            }

            public AppResult<IEnumerable<string>> ClonRepo(string workingDir, string repoConfig)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(ClonRepo) }); ;
            }

            public AppResult<IEnumerable<string>> PullRepo(string workingDir)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(PullRepo) }); ;
            }

            public AppResult<IEnumerable<string>> RemoteConfig(string workingDir)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(RemoteConfig) }); ;
            }

            public AppResult<IEnumerable<string>> ResetRepo(string workingDi)
            {
                RepoCommandProcessorControllerTest.hasBeenExecuted = true;
                return new AppResult<IEnumerable<string>>(new[] { nameof(ResetRepo) }); ;
            }
        }
    }
}
