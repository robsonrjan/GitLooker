using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IRepoLoader
    {
        void Load<T>(string chosenPath, List<T> allReposControl) where T : class;
        void CheckRepo<T>(string repoDdir, List<T> allReposControl, string newRepo = default(string)) where T : class;
    }
}
