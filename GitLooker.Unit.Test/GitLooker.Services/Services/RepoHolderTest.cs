using FluentAssertions;
using GitLooker.Core.Repository;
using GitLooker.Services.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.Services
{
    public class RepoHolderTest
    {
        private readonly RepoHolder repoHolder;
        private readonly IProjectFileRepo projectFileRepo;

        public RepoHolderTest()
        {
            projectFileRepo = Mock.Of<IProjectFileRepo>();
            repoHolder = new RepoHolder(projectFileRepo);
        }

        [Fact]
        public async Task FindRepoProjectFilesAsync_check_expected_results()
        {
            var expectedResult = new List<string>
            {
                "test1",
                "test2",
                "test3"
            };
            Mock.Get(projectFileRepo).Setup(r => r.GetAsync("test", "test"))
                .ReturnsAsync(expectedResult);

            await repoHolder.FindRepoProjectFilesAsync("test", "test");
            var result = repoHolder.GetProjectFiles("test");

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
