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

        [SetUp]
        public void BeforeEach()
        {
            repoCommandProcessor = Mock.Of<IRepoCommandProcessor>();
            repoHolder = Mock.Of<IRepoHolder>();
            repoCommandProcessorController = new RepoCommandProcessorController(repoCommandProcessor, repoHolder);
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
        public void Execute_for_parameter_commands_isNull_or_empty_throwException()
        {
            var commands = default(IEnumerable<MethodInfo>);
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, default, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(commands));
        }

        [Test]
        public void Execute_for_parameter_commands_isEmpty_or_empty_throwException()
        {
            var commands = Enumerable.Empty<MethodInfo>();
            Action actionCheck = () => repoCommandProcessorController.Execute(commands, default, default);

            actionCheck.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be(nameof(commands));
        }

        public class RepoCommandProcessorForTest : IRepoCommandProcessor
        {
            public AppResult<IEnumerable<string>> CheckOutBranch(string workingDir, string branch)
            {
                throw new NotImplementedException();
            }

            public AppResult<IEnumerable<string>> CheckRepo(string workingDir)
            {
                throw new NotImplementedException();
            }

            public AppResult<IEnumerable<string>> ClonRepo(string workingDir, string repoConfig)
            {
                throw new NotImplementedException();
            }

            public AppResult<IEnumerable<string>> PullRepo(string workingDir)
            {
                throw new NotImplementedException();
            }

            public AppResult<IEnumerable<string>> RemoteConfig(string workingDir)
            {
                throw new NotImplementedException();
            }

            public AppResult<IEnumerable<string>> ResetRepo(string workingDi)
            {
                throw new NotImplementedException();
            }
        }
    }
}
