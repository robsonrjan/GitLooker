using FluentAssertions;
using GitLooker.Services.Repository;
using System;
using System.IO;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.Repository
{
    public class GitFileRepoTest :IDisposable
    {
        private const string repoDir = "test1";
        private readonly GitFileRepo gitFileRepo;

        public GitFileRepoTest()
        {
            gitFileRepo = new GitFileRepo();
            PrepareDir();
        }

        [Fact]
        public void Get_check_dir_return_proper_repo_location()
        {
            var expectedValue = $"{Directory.GetCurrentDirectory()}\\{repoDir}";
            var result = gitFileRepo.Get(Directory.GetCurrentDirectory());

            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(new[] { expectedValue });
        }

        private void PrepareDir()
        {
            if (!Directory.Exists(repoDir))
                Directory.CreateDirectory(repoDir);
            if (!Directory.Exists($"{repoDir}\\test.git"))
                Directory.CreateDirectory($"{repoDir}\\test.git");
        }

        private void CleanDir()
        {
            if (Directory.Exists($"{repoDir}\\test.git"))
                Directory.Delete($"{repoDir}\\test.git");
            if (Directory.Exists(repoDir))
                Directory.Delete(repoDir);
        }

        public void Dispose() => CleanDir();
    }
}
