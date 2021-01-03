using GitLooker.Core.Repository;
using GitLooker.Core.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitLooker.Services.Services
{
    public class RepoHolder : IRepoHolder
    {
        public IList<string> RepoRemoteList { get; }
        public IList<string> ExpectedRemoteList { get; set; }

        private object addLocker = new object();
        private readonly IProjectFileRepo projectFileRepo;
        private List<Task> TaskProjectFilesHolderList;
        public ConcurrentDictionary<string, IEnumerable<string>> projectFiles;

        public RepoHolder(IProjectFileRepo projectFileRepo)
        {
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
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

        public void AddRemoteRepoThreadSefe(string repo)
        {
            lock (addLocker)
                RepoRemoteList.Add(repo);
        }

        public Task FindRepoProjectFilesAsync(string repoPath, string fileExtension)
        {
            if (!projectFiles.ContainsKey(repoPath) && !string.IsNullOrWhiteSpace(fileExtension))
                TaskProjectFilesHolderList.Add(Task.Run(async ()
                    => projectFiles.TryAdd(repoPath, await projectFileRepo.GetAsync(repoPath, fileExtension))));

            return Task.CompletedTask;
        }
    }
}
