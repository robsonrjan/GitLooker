using GitLooker.Core;
using GitLooker.Core.Services;

namespace GitLooker.Services.CommandProcessor
{
    public class RepoCommandProcessor : IRepoCommandProcessor
    {
        internal static string Executable = "git";

        private static Command commandUpdate(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\" remote update", workingDir) };
        private static Command commandStatus(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  status", workingDir) };
        private static Command commandPull(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  pull", workingDir) };
        private static Command commandReset(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  reset --hard", workingDir) };
        private static Command commandClean(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  clean -df", workingDir) };
        private static Command commandRemoteConfig(string workingDir) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  remote -v", workingDir) };
        private static Command commandCloneRepo(string workingDir, string repoConfig) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  clone {1}", workingDir, repoConfig) };
        private static Command commandCheckOut(string workingDir, string branch) => new Command { Exec = Executable, Args = string.Format("-C \"{0}\"  checkout \"{1}\"", workingDir, branch) };
        private static Command version(string executable = default) => new Command { Exec = executable ?? Executable, Args = "--version" };

        private readonly IProcessShell processShell;

        public RepoCommandProcessor(IProcessShell processShell)
        {
            if (processShell == null)
                throw new ArgumentException($"[{nameof(RepoCommandProcessor)}] ->Argument {processShell} can not be null!", nameof(processShell));

            this.processShell = processShell;
        }

        private static IEnumerable<Command> GenerateCloneCommand(string workingDir, string repoConfig) => new Command[] {
                commandCloneRepo(workingDir, repoConfig)
            };

        private static IEnumerable<Command> GenerateUpdateWithStatusCommand(string workingDir) => new Command[] {
                commandUpdate(workingDir),
                commandStatus(workingDir)
            };

        private static IEnumerable<Command> GeneratePullCommand(string workingDir) => new Command[] {
                commandPull(workingDir)
            };

        private static IEnumerable<Command> GenerateRemoteConfig(string workingDir) => new Command[] {
                commandRemoteConfig(workingDir)
            };

        private static IEnumerable<Command> GenerateResetCommand(string workingDir) => new Command[] {
                commandClean(workingDir),
                commandReset(workingDir)
            };

        private static IEnumerable<Command> GenerateCheckOutCommand(string workingDir, string branch) => new Command[] {
                commandCheckOut(workingDir, branch)
            };

        public AppResult<IEnumerable<string>> GetVersion(string executable = default)
        {
            var result = ReturnValue(processShell.Execute(new[] { version(executable) }));

            if (result.IsSuccess && !string.IsNullOrWhiteSpace(executable))
                Executable = executable;

            return result;
        }

        public AppResult<IEnumerable<string>> CheckRepo(string workingDir)
        {
            var rtn = processShell.Execute(GenerateUpdateWithStatusCommand(workingDir));
            return ReturnValue(rtn.Select(x => x.ToLowerInvariant()));
        }

        public AppResult<IEnumerable<string>> ClonRepo(string workingDir, string repoConfig)
        {
            var rtn = processShell.Execute(GenerateCloneCommand(workingDir, repoConfig));
            return ReturnValue(rtn.Select(x => x.ToLowerInvariant()));
        }

        public AppResult<IEnumerable<string>> PullRepo(string workingDir)
        {
            var rtn = processShell.Execute(GeneratePullCommand(workingDir));
            return ReturnValue(rtn.Select(x => x.ToLowerInvariant()));
        }

        public AppResult<IEnumerable<string>> RemoteConfig(string workingDir)
        {
            var rtn = processShell.Execute(GenerateRemoteConfig(workingDir));
            var result = rtn.FirstOrDefault(x => x.ToLowerInvariant().Contains("(push)"))?.Replace('\t', ' ').Split(' ');
            if ((result?.Length ?? default) > 2)
                return ReturnValue(new[] { result[result.Length - 2] }.AsEnumerable());
            else
                return ReturnValue(new[] { string.Empty }.AsEnumerable());
        }

        public AppResult<IEnumerable<string>> ResetRepo(string workingDi)
        {
            var rtn = processShell.Execute(GenerateResetCommand(workingDi));
            return ReturnValue(rtn.Select(x => x.ToLowerInvariant()));
        }

        public AppResult<IEnumerable<string>> CheckOutBranch(string workingDi, string branch)
        {
            var rtn = processShell.Execute(GenerateCheckOutCommand(workingDi, branch));
            return ReturnValue(rtn.Select(x => x.ToLowerInvariant()));
        }

        private AppResult<T> ReturnValue<T>(T value)
            => new AppResult<T>(value);
    }
}
