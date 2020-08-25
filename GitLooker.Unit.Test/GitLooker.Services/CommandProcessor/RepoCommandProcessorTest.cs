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
    public class RepoCommandProcessorTest
    {
        private const string workingDir = nameof(workingDir);
        private const string configRepo = nameof(configRepo);
        private IRepoCommandProcessor repoCommandProcessor;
        private IPowersShell powerShell;

        [SetUp]
        public void BeforeEach()
        {
            powerShell = Mock.Of<IPowersShell>();
            Mock.Get(powerShell).Setup(p => p.Execute(It.Is<string>(p => p.Contains(workingDir)), It.IsAny<bool>()))
                .Returns(() => new[] { workingDir });
            repoCommandProcessor = new RepoCommandProcessor(powerShell);
        }

        [Test]
        public void Constructor_for_parameter_powerShell_isNull_throwException()
        {
            Action actionCheck = () => new RepoCommandProcessor(default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(powerShell));
        }

        [Test]
        public void CheckRepo_executes()
        {
            var result = repoCommandProcessor.CheckRepo(workingDir);

            Mock.Get(powerShell).Verify(p => p.Execute(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { workingDir.ToLower() }
            });
        }

        [Test]
        public void ClonRepo_executes()
        {
            var result = repoCommandProcessor.ClonRepo(workingDir, configRepo);

            Mock.Get(powerShell).Verify(p => p.Execute(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            result.Error.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.SpecialValue.Should().BeNull();
            result.Value.Should().BeEquivalentTo(new[] {
                new[] { workingDir.ToLower() }
            });
        }
    }
}
