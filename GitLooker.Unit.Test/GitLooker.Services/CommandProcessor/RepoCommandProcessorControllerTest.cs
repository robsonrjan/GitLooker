using FluentAssertions;
using GitLooker.Core;
using GitLooker.Core.Services;
using GitLooker.Services.CommandProcessor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GitLooker.Unit.Test.GitLooker.Services.CommandProcessor
{
    [TestFixture]
    public class RepoCommandProcessorControllerTest
    {
        private RepoCommandProcessorController repoCommandProcessorController;
        private IRepoCommandProcessor repoCommandProcessor;
        private static bool hasBeenExecuted;
        private IEnumerable<MethodInfo> commands;
        private static int executionCount;

        [SetUp]
        public void BeforeEach()
        {
            repoCommandProcessor = new RepoCommandProcessorForTest();
            repoCommandProcessorController = new RepoCommandProcessorController(repoCommandProcessor);
            commands = repoCommandProcessorController.CommonCommandActions;
            hasBeenExecuted = default;
            executionCount = default;
        }

        [TestCase("CheckOutBranch", null)]
        [TestCase("CheckRepo", null)]
        [TestCase("ClonRepo", null)]
        [TestCase("PullRepo", null)]
        [TestCase("RemoteConfig", "RemoteConfig")]
        [TestCase("ResetRepo", null)]
        public void Execute_check_all_execution(string commandName, object specialValue)
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
            executionCount.Should().Be(1);
            hasInvoked.Should().BeTrue();
        }

        [Test]
        public void Execute_check_fewAtOnce_execution()
        {
            var commamdNames = new[] { "CheckOutBranch", "RemoteConfig", "ClonRepo" };
            bool hasInvoked = default;
            int reposCount = 1;
            object specialValue = "RemoteConfig";
            Action checkInvokation = () => hasInvoked = true;
            IEnumerable<string> options = new[] { default(string), default(string), default(string) };
            var commandForTest = commands.Where(c => commamdNames.Contains(c.Name));

            var result = repoCommandProcessorController.Execute(commandForTest, options, checkInvokation);

            hasBeenExecuted.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().Be(specialValue);
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { "CheckOutBranch" },
                new[] { "ClonRepo" }
            });
            executionCount.Should().Be(commamdNames.Count());
            hasInvoked.Should().BeTrue();
        }

        [Test]
        public void Constructor_for_parameter_repoCommandProcessor_isNull_throwException()
        {
            Action actionCheck = () => new RepoCommandProcessorController(default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(repoCommandProcessor));
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
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(CheckOutBranch) });
            }

            public AppResult<IEnumerable<string>> CheckRepo(string workingDir)
            {
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(CheckRepo) }); ;
            }

            public AppResult<IEnumerable<string>> ClonRepo(string workingDir, string repoConfig)
            {
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(ClonRepo) }); ;
            }

            public AppResult<IEnumerable<string>> PullRepo(string workingDir)
            {
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(PullRepo) }); ;
            }

            public AppResult<IEnumerable<string>> RemoteConfig(string workingDir)
            {
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(RemoteConfig) }); ;
            }

            public AppResult<IEnumerable<string>> ResetRepo(string workingDi)
            {
                hasBeenExecuted = true;
                executionCount++;
                return new AppResult<IEnumerable<string>>(new[] { nameof(ResetRepo) }); ;
            }
        }
    }
}
