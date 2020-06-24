using System;
using System.Collections.Generic;
using System.Reflection;

namespace GitLooker.Core.Services
{
    public interface IRepoCommandProcessorController
    {
        Dictionary<string, MethodInfo> CommonCommandActions { get; }
        AppResult<IEnumerable<string>> Execute(IEnumerable<MethodInfo> commands, IEnumerable<string> options, Action beforeExecution);
    }
}
