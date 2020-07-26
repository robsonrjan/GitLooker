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
using GitLooker.Services.Configuration;

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
