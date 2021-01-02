using FluentAssertions;
using GitLooker.Services.Repository;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GitLooker.Unit.Test.GitLooker.Services.Repository
{
    [TestFixture]
    public class ProjectFileRepoTest
    {
        private const string repoDir = "test-ProjectFileRepo";
        private ProjectFileRepo projectFileRepo;
        private string repoPath;

        [SetUp]
        public void BeforeEach()
            => projectFileRepo = new ProjectFileRepo();

        [TearDown]
        public void AfterEach() => RemoveRepoData();

        [Test]
        public async Task GetAsync_all_project_files()
        {
            const string projectExtension = "sln";
            PrepareRepoData();

            var projectFiles = await projectFileRepo.GetAsync(repoPath, projectExtension);

            projectFiles.Should().NotContain($"abc.{projectExtension}").And.HaveCount(41);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetAsync_parameter_path_NullOrWhiteSpace_throw_exception(string path)
        {
            Action testAction = () => _ = projectFileRepo.GetAsync(path, "test");

            testAction.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("path");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetAsync_parameter_extension_NullOrWhiteSpace_throw_exception(string extension)
        {
            Action testAction = () => _ = projectFileRepo.GetAsync("test", extension);

            testAction.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("extension");
        }

        private void PrepareRepoData()
        {
            object lockObj = new object();
            var catalogList = Enumerable.Range(0, 20);
            var filesList = Enumerable.Range(0, 100);
            repoPath = $"{Directory.GetCurrentDirectory()}\\{repoDir}";

            if (!Directory.Exists(repoPath))
                Directory.CreateDirectory(repoPath);

            File.WriteAllText($"{repoPath}\\abc.sln", "project");

            catalogList.ToList().AsParallel().ForAll(d =>
            {
                int extensionLevelOne = 0;
                Directory.CreateDirectory($"{repoPath}\\dir-{d}");
                filesList.ToList().ForEach(f => fileSafeWriter($"{repoPath}\\dir-{d}\\{d}.{extensionLevelOne++}"));
                fileSafeWriter($"{repoPath}\\dir-{d}\\abc.sln");
                catalogList.ToList().ForEach(d =>
                {
                    int extensionLevelTwo = 0;
                    Directory.CreateDirectory($"{repoPath}\\dir-{d}\\dir-{d}");
                    filesList.ToList().ForEach(f => fileSafeWriter($"{repoPath}\\dir-{d}\\dir-{d}\\{d}.{extensionLevelTwo++}"));
                    fileSafeWriter($"{repoPath}\\dir-{d}\\dir-{d}\\abc.sln");
                });
            });

            void fileSafeWriter(string path)
            {
                lock (lockObj)
                    File.WriteAllText(path, "text");
            };
        }

        private void RemoveRepoData()
        {
            var dir = $"{Directory.GetCurrentDirectory()}\\{repoDir}";

            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}
