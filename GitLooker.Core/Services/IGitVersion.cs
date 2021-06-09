using System.Collections.Generic;

namespace GitLooker.Core.Services
{
    public interface IGitVersion
    {
        AppResult<IEnumerable<string>> GetVersion(string executable = default);
    }
}