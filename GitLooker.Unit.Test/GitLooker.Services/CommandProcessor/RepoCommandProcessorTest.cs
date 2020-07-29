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
        private const string workingDir = "test";
        private RepoCommandProcessor repoCommandProcessor;
        private IPowersShell powersShell;

        [SetUp]
        public void BeforeEach()
        {
            powersShell = Mock.Of<IPowersShell>();
            Mock.Get(powersShell).Setup(p => p.Execute(It.Is<string>(s => s.Contains(workingDir)), It.IsAny<bool>()))
                .Returns(new[] { workingDir });
            repoCommandProcessor = new RepoCommandProcessor(powersShell);
        }



    }
}
