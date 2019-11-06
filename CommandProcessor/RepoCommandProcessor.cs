using System;
using System.Collections.Generic;
using System.Linq;

namespace GitLooker.CommandProcessor
{
    public class RepoCommandProcessor : IRepoCommandProcessor
    {
        private const string commandUpdate = "git remote update";
        private const string commandStatus = "git status";
        private const string commandPull = "git pull";
        private const string commandReset = "git reset --hard";
        private const string commandClean = "git clean -df";
        private const string commandPath = "cd \"{0}\"";
        private const string commandRemoteConfig = "git remote -v";
        private const string commandCloneRepo = "git clone {0}";

        private readonly IPowersShell powerShell;

        public RepoCommandProcessor(IPowersShell powerShell)
        {
            if (powerShell == null)
                throw new ArgumentException($"[{nameof(RepoCommandProcessor)}] ->Argument {powerShell} can not be null!");

            this.powerShell = powerShell;
        }

        private static string GenerateCloneCommand(string workingDir, string repoConfig) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            string.Format(commandCloneRepo, repoConfig)
        });

        private static string GenerateUpdateWithStatusCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandUpdate,
            commandStatus
        });

        private static string GeneratePullCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandPull
        });

        private static string GenerateRemoteConfig(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandRemoteConfig
        });

        private static string GenerateResetCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandClean,
            commandReset
        });

        public IEnumerable<string> CheckRepo(string workingDir)
        {
            var rtn = powerShell.Execute(GenerateUpdateWithStatusCommand(workingDir));
            return rtn.Select(x => x.ToLower());
        }

        public IEnumerable<string> ClonRepo(string workingDir, string repoConfig)
        {
            var rtn = powerShell.Execute(GenerateCloneCommand(workingDir, repoConfig));
            return rtn.Select(x => x.ToLower());
        }

        public IEnumerable<string> PullRepo(string workingDir)
        {
            var rtn = powerShell.Execute(GeneratePullCommand(workingDir));
            return rtn.Select(x => x.ToLower());
        }

        public string RemoteConfig(string workingDir)
        {
            var rtn = powerShell.Execute(GenerateRemoteConfig(workingDir));
            var result = rtn.FirstOrDefault(x => x.ToLower().Contains("(push)")).Replace('\t', ' ').Split(' ');
            if (result.Length > 2)
                return result[result.Length - 2];
            else
                return string.Empty;
        }

        public IEnumerable<string> ResetRepo(string workingDi)
        {
            var rtn = powerShell.Execute(GenerateResetCommand(workingDi));
            return rtn.Select(x => x.ToLower());
        }
    }
}
