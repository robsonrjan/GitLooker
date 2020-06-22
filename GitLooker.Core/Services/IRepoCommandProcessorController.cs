using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitLooker.Core.Services
{
    public interface IRepoCommandProcessorController
    {
        Dictionary<string, MethodInfo> CommonCommandActions { get; }
        AppResult<IEnumerable<string>> Execute(IEnumerable<MethodInfo> commands, IEnumerable<string> options);
    }
}
