﻿using GitLooker.Core.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace GitLooker.Services.Repository
{
    public class ProjectFileRepo : IProjectFileRepo
    {
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

        private void FindProjectFiles(string path, string extension, IList<string> projectFileList)
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            files.Where(f => f.EndsWith(extension)).ToList()
                .ForEach(f => AddThreadSafeToList(projectFileList, f));

            dirs.AsParallel()
                .ForAll(dir => FindProjectFiles(dir, extension, projectFileList));
        }

        private void AddThreadSafeToList(IList<string> list, string item)
        {
            listSlimLocker.Wait();
            list.Add(item);
            listSlimLocker.Release();
        }
    }
}
