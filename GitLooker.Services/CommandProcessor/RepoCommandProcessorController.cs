using GitLooker.Core;
using GitLooker.Core.Services;
using Microsoft.PowerShell.Cmdletization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitLooker.Services.CommandProcessor
{
    public class RepoCommandProcessorController : IRepoCommandProcessorController
    {
        private readonly IRepoCommandProcessor repoCommandProcessor;

        public Dictionary<string, MethodInfo> CommonCommandActions { get; }

        public RepoCommandProcessorController(IRepoCommandProcessor repoCommandProcessor)
        {
            if(repoCommandProcessor == default)
                throw new ArgumentNullException(nameof(repoCommandProcessor));

            this.repoCommandProcessor = repoCommandProcessor;

            CommonCommandActions = this.repoCommandProcessor
                .GetType()
                .GetMethods()
                .ToDictionary(k => k.Name, v => v);
        }

        public AppResult<IEnumerable<string>> Execute(IEnumerable<MethodInfo> commands, IEnumerable<string> options)
        {
            if ((!commands?.Any() ?? false) || (!options?.Any() ?? false))
                throw new ArgumentNullException(nameof(options));
            AppResult<IEnumerable<string>> result = default;

            foreach (var command in commands)
            {
                var rtn = command.Invoke(repoCommandProcessor, options.ToArray()) as AppResult<IEnumerable<string>>;
                (result ?? (result = rtn)).Add(rtn);
            }

            return result;
        }
    }
}
