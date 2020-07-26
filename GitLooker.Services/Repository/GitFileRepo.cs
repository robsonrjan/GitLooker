using GitLooker.Core.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitLooker.Services.Repository
{
    public class GitFileRepo : IGitFileRepo
    {
        public IEnumerable<string> Get(string chosenPath)
        {
            var gitPathList = new List<string>();
            FindGitPaths(chosenPath, gitPathList);
            return gitPathList;
        }

        private static void FindGitPaths(string path, IList<string> gitPathList)
        {
            var dirs = Directory.GetDirectories(path);
            if (dirs.Any(d => d.EndsWith(".git")))
                gitPathList.Add(path);
            else
                foreach (var dir in dirs) FindGitPaths(dir, gitPathList);
        }
    }
}
