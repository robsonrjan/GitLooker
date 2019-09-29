using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitLooker.CommandProcessor
{
    public class CommandProcessor : ICommandProcessor
    {
        private const string commandUpdate = "git remote update";
        private const string commandStatus = "git status";
        private const string commandPull = "git pull";
        private const string commandReset = "git reset --hard";
        private const string commandClean = "git clean -df";
        private const string commandPath = "cd \"{0}\"";

        private readonly IPowersShell powerShell;

        public CommandProcessor(IPowersShell powerShell)
        {
            if (powerShell == null)
                throw new ArgumentException($"[{nameof(CommandProcessor)}] ->Argument {powerShell} can not be null!");

            this.powerShell = powerShell;
        }

        private string GenerateUpdateWithStatusCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandUpdate,
            commandStatus
        });

        private string GeneratePullCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandPull
        });

        private string GenerateResetCommand(string workingDir) => string.Join(Environment.NewLine, new[] {
            string.Format(commandPath, workingDir),
            commandClean,
            commandReset
        });

        public IEnumerable<string> CheckRepo(string workingDir)
        {
            var rtn = powerShell.Execute(GenerateUpdateWithStatusCommand(workingDir));
            return rtn.Select(x => x.BaseObject.ToString().ToLower());
        }

        public IEnumerable<string> PullRepo(string workingDir)
        {
            var rtn = powerShell.Execute(GeneratePullCommand(workingDir));
            return rtn.Select(x => x.BaseObject.ToString().ToLower());
        }

        public IEnumerable<string> ResetRepo(string workingDi)
        {
            var rtn = powerShell.Execute(GenerateResetCommand(workingDi));
            return rtn.Select(x => x.BaseObject.ToString().ToLower());
        }
    }
}
