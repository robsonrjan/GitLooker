using FluentAssertions;
using GitLooker.Core;
using GitLooker.Core.Services;
using GitLooker.Services.CommandProcessor;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.CommandProcessor
{
    public class RepoCommandProcessorTest
    {
        private readonly Command configRepo = new Command { Exec = "Exec", Args = "Args" };
        private readonly IRepoCommandProcessor repoCommandProcessor;
        private readonly IProcessShell processShell;

        public RepoCommandProcessorTest()
        {
            processShell = Mock.Of<IProcessShell>();
            Mock.Get(processShell).Setup(p => p.Execute(It.IsAny<IEnumerable<Command>>()))
                .Returns(() => new[] { nameof(configRepo) });
            repoCommandProcessor = new RepoCommandProcessor(processShell);
        }

        [Fact]
        public void Constructor_for_parameter_processsShell_isNull_throwException()
        {
            Action actionCheck = () => new RepoCommandProcessor(default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(processShell));
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [InlineData("test", "")]
        [InlineData("(push) 1 2 3 4", "3")]
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

        [Fact]
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

        [Fact]
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
