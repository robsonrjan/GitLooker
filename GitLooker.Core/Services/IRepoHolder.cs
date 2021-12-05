namespace GitLooker.Core.Services
{
    public interface IRepoHolder
    {
        IEnumerable<string> GetProjectFiles(string repoPath);
        Task FindRepoProjectFilesAsync(string repoPath, string fileExtension);
    }
}
