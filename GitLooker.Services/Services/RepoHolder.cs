using GitLooker.Core.Services;
using System.Collections.Generic;

namespace GitLooker.Services.Services
{
    public class RepoHolder : IRepoHolder
    {
        public IList<string> RepoRemoteList { get; }
        public IList<string> ExpectedRemoteList { get; set; }

        private object addLocker = new object();

        public RepoHolder()
        {
            RepoRemoteList = new List<string>();
            ExpectedRemoteList = new List<string>();
        }

        public void AddRemoteRepoThreadSefe(string repo)
        {
            lock (addLocker)
                RepoRemoteList.Add(repo);
        }
    }
}
