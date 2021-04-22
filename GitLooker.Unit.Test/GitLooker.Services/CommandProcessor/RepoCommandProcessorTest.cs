using FluentAssertions;
using GitLooker.Core;
using GitLooker.Core.Services;
using GitLooker.Services.CommandProcessor;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GitLooker.Unit.Test.GitLooker.Services.CommandProcessor
{
    [TestFixture]
    public class RepoCommandProcessorTest
    {
        private Command configRepo = new Command { Exec = "Exec", Args = "Args" };
        private IRepoCommandProcessor repoCommandProcessor;
        private IProcessShell processShell;

        [SetUp]
        public void BeforeEach()
        {
            processShell = Mock.Of<IProcessShell>();
            Mock.Get(processShell).Setup(p => p.Execute(It.IsAny<IEnumerable<Command>>()))
                .Returns(() => new[] { nameof(configRepo) });
            repoCommandProcessor = new RepoCommandProcessor(processShell);
        }

        [Test]
        public void Constructor_for_parameter_processsShell_isNull_throwException()
        {
            Action actionCheck = () => new RepoCommandProcessor(default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(processShell));
        }

        [Test]
        public void CheckRepo_executes()
        {
            var result = repoCommandProcessor.CheckRepo(nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { nameof(configRepo).ToLowerInvariant() }
            });
        }

        [Test]
        public void ClonRepo_executes()
        {
            var result = repoCommandProcessor.ClonRepo(nameof(configRepo), nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { nameof(configRepo).ToLowerInvariant() }
            });
        }

        [Test]
        public void PullRepo_executes()
        {
            var result = repoCommandProcessor.PullRepo(nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { nameof(configRepo).ToLowerInvariant() }
            });
        }

        [TestCase("test", "")]
        [TestCase("(push) 1 2 3 4", "3")]
        public void RemoteConfig_executes(string executionResult, string expectedResult)
        {
            Mock.Get(processShell).Setup(p => p.Execute(It.IsAny<IEnumerable<Command>>()))
                .Returns(() => new[] { executionResult });

            var result = repoCommandProcessor.RemoteConfig(nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { expectedResult }
            });
        }

        [Test]
        public void ResetRepo_executes()
        {
            var result = repoCommandProcessor.ResetRepo(nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { nameof(configRepo).ToLowerInvariant() }
            });
        }

        [Test]
        public void CheckOutBranch_executes()
        {
            var result = repoCommandProcessor.CheckOutBranch(nameof(configRepo), nameof(configRepo));

            Mock.Get(processShell).Verify(p => p.Execute(It.IsAny<IEnumerable<Command>>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { nameof(configRepo).ToLowerInvariant() }
            });
        }
    }
}
