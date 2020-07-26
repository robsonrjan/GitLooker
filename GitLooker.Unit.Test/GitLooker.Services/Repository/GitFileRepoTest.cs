using FluentAssertions;
using GitLooker.Services.Repository;
using NUnit.Framework;
using System.IO;

namespace GitLooker.Unit.Test.GitLooker.Services.Repository
{
    [TestFixture]
    public class GitFileRepoTest
    {
        private const string repoDir = "test1";
        private GitFileRepo gitFileRepo;

        [SetUp]
        public void BeforeEach()
        {
            gitFileRepo = new GitFileRepo();
            PrepareDir();
        }

        [TearDown]
        public void AfterEach() => CleanDir();

        [Test]
        public void Get_check_dir_return_proper_repo_location()
        {
            var expectedValue = @$"{Directory.GetCurrentDirectory()}\{repoDir}";
            var result = gitFileRepo.Get(Directory.GetCurrentDirectory());

            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(new[] { expectedValue });
        }

        private void PrepareDir()
        {
            if (!Directory.Exists(repoDir))
                Directory.CreateDirectory(repoDir);
            if (!Directory.Exists(@$"{repoDir}\test.git"))
                Directory.CreateDirectory(@$"{repoDir}\\test.git");
        }

        private void CleanDir()
        {
            if (Directory.Exists(@$"{repoDir}\test.git"))
                Directory.Delete(@$"{repoDir}\test.git");
            if (Directory.Exists(repoDir))
                Directory.Delete(repoDir);
        }
    }
}
