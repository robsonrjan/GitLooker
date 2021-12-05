using GitLooker.Core;
using GitLooker.Core.Services;
using GitLooker.Core.Validators;
using GitLooker.Services.CommandProcessor;

namespace GitLooker.Services.Configuration
{
    public class GitValidator : IGitValidator
    {
        private const string GitSetupPath = @"C:\Program Files\Git\bin\git.exe";
        private readonly IGitVersion gitVersion;

        public GitValidator(IGitVersion gitVersion)
        {
            this.gitVersion = gitVersion;
        }

        public bool TryToFind(string configPath, out GitConfigInfo gitInfo)
        {
            gitInfo = default;
            GitConfigInfo info;

            if (TryToCheck(configPath, out info))
            {
                gitInfo = info;
                return true;
            }

            if (TryToCheck(GitSetupPath, out info))
            {
                gitInfo = info;
                return true;
            }

            return false;
        }

        private bool TryToCheck(string setupPath, out GitConfigInfo gitInfo)
        {
            gitInfo = default;
            try
            {
                var result = gitVersion.GetVersion(setupPath);
                var version = result.Value?.FirstOrDefault()?.FirstOrDefault();
                gitInfo = new GitConfigInfo { Version = version, Executable = RepoCommandProcessor.Executable };
                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
