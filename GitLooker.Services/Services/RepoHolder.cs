using GitLooker.Core.Repository;
using GitLooker.Core.Services;
using System.Collections.Concurrent;

namespace GitLooker.Services.Services
{
    public class RepoHolder : IRepoHolder
    {
        private readonly IProjectFileRepo projectFileRepo;
        private List<Task> TaskProjectFilesHolderList;
        public ConcurrentDictionary<string, IEnumerable<string>> projectFiles;

        public RepoHolder(IProjectFileRepo projectFileRepo)
        {
            projectFiles = new ConcurrentDictionary<string, IEnumerable<string>>();
            TaskProjectFilesHolderList = new List<Task>();

            this.projectFileRepo = projectFileRepo;
        }

        public IEnumerable<string> GetProjectFiles(string repoPath)
        {
            if (!projectFiles.ContainsKey(repoPath) && TaskProjectFilesHolderList.Any())
                Task.WaitAll(TaskProjectFilesHolderList.ToArray());
            return projectFiles[repoPath];
        }

        public Task FindRepoProjectFilesAsync(string repoPath, string fileExtension)
        {
            if (!projectFiles.ContainsKey(repoPath) && !string.IsNullOrWhiteSpace(fileExtension))
                TaskProjectFilesHolderList.Add(Task.Run(()
                    => projectFiles.TryAdd(repoPath, projectFileRepo.GetAsync(repoPath, fileExtension).GetAwaiter().GetResult())));

            return Task.CompletedTask;
        }
    }
}
