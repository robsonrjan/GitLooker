using GitLooker.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GitLooker.Services.Repository
{
    public class ProjectFileRepo : IProjectFileRepo
    {
        private const int MaxLevelOfDepth = 4;
        private SemaphoreSlim listSlimLocker = new SemaphoreSlim(1, 1);

        public Task<IList<string>> GetAsync(string path, string extension)
        {
            CheckArguments(path, extension);

            IList<string> projectFileList = new List<string>();
            FindProjectFiles(path, extension, projectFileList);
            return Task.FromResult(projectFileList);
        }

        private static void CheckArguments(string path, string extension)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path), nameof(path));

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException(nameof(extension), nameof(extension));
        }

        private void FindProjectFiles(string path, string extension, IList<string> projectFileList, int levelOfDepth = 0)
        {
            var files = Directory.GetFiles(path, $"*{extension}");
            IEnumerable<string> dirs = Enumerable.Empty<string>();

            if (!(files?.Any() ?? false))
                dirs = Directory.GetDirectories(path);

            foreach (var f in files.Where(f => f.EndsWith(extension)))
                AddThreadSafeToList(projectFileList, f);

            if (++levelOfDepth >= MaxLevelOfDepth) return;

            dirs.Where(d => !d.Contains("\\."))
                .AsParallel()
                .ForAll(dir => FindProjectFiles(dir, extension, projectFileList, levelOfDepth));
        }

        private void AddThreadSafeToList(IList<string> list, string item)
        {
            listSlimLocker.Wait();
            list.Add(item);
            listSlimLocker.Release();
        }
    }
}
