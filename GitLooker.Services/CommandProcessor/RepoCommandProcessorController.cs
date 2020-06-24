using GitLooker.Core;
using GitLooker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GitLooker.Services.CommandProcessor
{
    public class RepoCommandProcessorController : IRepoCommandProcessorController
    {
        private const string CommandForRepoCheck = "RemoteConfig";

        private readonly IRepoCommandProcessor repoCommandProcessor;
        private readonly IRepoHolder repoHolder;
        private bool isConfigured = default;

        public Dictionary<string, MethodInfo> CommonCommandActions { get; }

        public RepoCommandProcessorController(IRepoCommandProcessor repoCommandProcessor, IRepoHolder repoHolder)
        {
            if (repoCommandProcessor == default)
                throw new ArgumentNullException(nameof(repoCommandProcessor));

            if (repoHolder == default)
                throw new ArgumentNullException(nameof(repoHolder));

            this.repoCommandProcessor = repoCommandProcessor;
            this.repoHolder = repoHolder;

            CommonCommandActions = this.repoCommandProcessor
                .GetType()
                .GetMethods()
                .ToDictionary(k => k.Name, v => v);
        }

        public AppResult<IEnumerable<string>> Execute(IEnumerable<MethodInfo> commands, IEnumerable<string> options, Action beforeExecution)
        {
            if ((!commands?.Any() ?? false) || (!options?.Any() ?? false))
                throw new ArgumentNullException(nameof(options));
            AppResult<IEnumerable<string>> result = default;
            object specialValue = default;

            if (beforeExecution != default) beforeExecution.Invoke();

            foreach (var command in commands)
            {
                if ((command.Name == CommandForRepoCheck) && isConfigured) continue;

                var parCount = command.GetParameters().Length;
                var rtn = command.Invoke(repoCommandProcessor, options.Take(parCount).ToArray()) as AppResult<IEnumerable<string>>;

                if ((command.Name == CommandForRepoCheck) && rtn.IsSuccess)
                {
                    specialValue = SetRepoConfiguraion(rtn);
                    continue;
                }

                if (result == default)
                {
                    result = rtn;
                    result.SpecialValue = specialValue;
                }
                else result.Add(rtn);
            }

            return result;
        }

        private string SetRepoConfiguraion(AppResult<IEnumerable<string>> result)
        {
            string repoConfiguration = default;
            var repoConfig = result.Value.SelectMany(v => v).First();
            if (!string.IsNullOrEmpty(repoConfig))
                repoHolder.RepoRemoteList.Add(repoConfiguration = repoConfig.ToLower());
            isConfigured = true;
            return repoConfiguration;
        }
    }
}
