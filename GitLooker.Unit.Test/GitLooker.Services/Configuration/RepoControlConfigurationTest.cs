using FluentAssertions;
using GitLooker.Core.Services;
using NUnit.Framework;
using System;

namespace GitLooker.Unit.Test.GitLooker.Services.Configuration
{
    [TestFixture]
    public class RepoControlConfigurationTest
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Constructor_for_parameter_repoPath_isEmptyWhiteSpaceOrNull_throwException(string repoPath)
        {
            Action actionCheck = () => new RepoControlConfiguration(repoPath, default, default, default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(repoPath));
        }

        [Test]
        public void Constructor_for_parameter_semaphore_isNull_throwException()
        {
            IAppSemaphoreSlim semaphore = default;
            Action actionCheck = () => new RepoControlConfiguration("test", semaphore, default, default);

            actionCheck.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(semaphore));
        }
    }
}
