using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IRepoHolder
    {
        IList<string> RepoRemoteList { get; }
        IList<string> ExpectedRemoteList { get; set; }
        void AddRemoteRepoThreadSefe(string repo);
    }
}
