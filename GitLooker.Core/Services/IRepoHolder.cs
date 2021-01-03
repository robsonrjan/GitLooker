using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitLooker.Core.Services
{
    public interface IRepoHolder
    {
        IList<string> RepoRemoteList { get; }
        IList<string> ExpectedRemoteList { get; set; }
        void AddRemoteRepoThreadSefe(string repo);
        IEnumerable<string> GetProjectFiles(string repoPath);
        Task FindRepoProjectFilesAsync(string repoPath, string fileExtension);
    }
}
