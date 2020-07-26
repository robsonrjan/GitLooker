using FluentAssertions;
using GitLooker.Services.Services;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GitLooker.Unit.Test.GitLooker.Services.Services
{
    [TestFixture]
    public class RepoHolderTest
    {
        private RepoHolder repoHolder;

        [SetUp]
        public void BeforeEach()
        {
            repoHolder = new RepoHolder();
        }

        [Test]
        public void Check_repoRemoteList_and_expectedRemoteList_is_not_empty_afterClassInisialization()
        {
            repoHolder.RepoRemoteList.Should().NotBeNull().And.BeEmpty();
            repoHolder.ExpectedRemoteList.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public void AddRemoteRepoThreadSefe_check_if_added_safe()
        {
            const int dataCount = 100;
            Action testAction = () =>
            {
                var threadCount = Enumerable.Range(0, dataCount);

                Parallel.ForEach(threadCount, c => repoHolder.AddRemoteRepoThreadSefe(c.ToString()));
            };

            testAction.Should().NotThrow();
            repoHolder.RepoRemoteList.Should().HaveCount(dataCount);
        }
    }
}
