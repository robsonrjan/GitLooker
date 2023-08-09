using FluentAssertions;
using GitLooker.Services.Repository;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.Repository
{
    public class ProjectFileRepoTest : IDisposable
    {
        private const string repoDir = "test-ProjectFileRepo";
        private readonly ProjectFileRepo projectFileRepo;
        private string? repoPath;

        public ProjectFileRepoTest()
            => projectFileRepo = new ProjectFileRepo();

        [Fact]
        public async Task GetAsync_all_project_upToThirdLevelFolders_files()
        {
            const string projectExtension = "sln";
            PrepareRepoData();

            var projectFiles = await projectFileRepo.GetAsync(repoPath, projectExtension);

            projectFiles.Should().NotContain($"abc.{projectExtension}").And.HaveCount(1);
        }

        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetAsync_parameter_path_NullOrWhiteSpace_throw_exception(string path)
        {
            Action testAction = () => _ = projectFileRepo.GetAsync(path, "test");

            testAction.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("path");
        }

        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
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
                catalogList.ToList().ForEach(dd =>
                {
                    int extensionLevelTwo = 0;
                    Directory.CreateDirectory($"{repoPath}\\dir-{dd}\\dir-{dd}");
                    filesList.ToList().ForEach(f => fileSafeWriter($"{repoPath}\\dir-{dd}\\dir-{dd}\\{dd}.{extensionLevelTwo++}"));
                    fileSafeWriter($"{repoPath}\\dir-{dd}\\dir-{dd}\\abc.sln");
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

        public void Dispose() => RemoveRepoData();
    }
}
