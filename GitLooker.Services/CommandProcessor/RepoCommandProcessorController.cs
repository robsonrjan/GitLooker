﻿using GitLooker.Core;
using GitLooker.Core.Services;
using System.Reflection;

namespace GitLooker.Services.CommandProcessor
{
    public class RepoCommandProcessorController : IRepoCommandProcessorController
    {
        private readonly IRepoCommandProcessor repoCommandProcessor;
        private bool isConfigured = default;

        public IList<MethodInfo> CommonCommandActions { get; }

        public RepoCommandProcessorController(IRepoCommandProcessor repoCommandProcessor)
        {
            if (repoCommandProcessor == default)
                throw new ArgumentNullException(nameof(repoCommandProcessor));

            this.repoCommandProcessor = repoCommandProcessor;

            CommonCommandActions = typeof(IRepoCommandProcessor)
                .GetMethods()
                .ToList();
        }

        public AppResult<IEnumerable<string>> Execute(IEnumerable<MethodInfo> commands, IEnumerable<string> options, Action beforeExecution)
        {
            AppResult<IEnumerable<string>> rtn = default;
            if (!commands?.Any() ?? true)
                throw new ArgumentNullException(nameof(commands));
            if (!options?.Any() ?? true)
                throw new ArgumentNullException(nameof(options));
            AppResult<IEnumerable<string>> result = default;
            object specialValue = default;

            if (beforeExecution != default) beforeExecution.Invoke();

            foreach (var command in commands)
            {
                if ((command.Name == nameof(RemoteConfig)) && isConfigured) continue;

                var parCount = command.GetParameters().Length;
                rtn = command.Invoke(repoCommandProcessor, options.Take(parCount).ToArray()) as AppResult<IEnumerable<string>>;

                if ((command.Name == nameof(RemoteConfig)) && rtn.IsSuccess)
                {
                    specialValue = RemoteConfig(rtn);
                    continue;
                }

                if (result == default) result = rtn;
                else result.Add(rtn);
            }
            if (result == default) result = rtn;
            result.SpecialValue = specialValue;
            return result;
        }

        private string RemoteConfig(AppResult<IEnumerable<string>> result)
        {
            var repoConfig = result.Value.SelectMany(v => v).First();
            isConfigured = true;
            return repoConfig;
        }
    }
}
