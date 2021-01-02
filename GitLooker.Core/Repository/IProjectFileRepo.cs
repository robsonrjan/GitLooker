using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitLooker.Core.Repository
{
    public interface IProjectFileRepo
    {
        Task<IList<string>> GetAsync(string path, string extension);
    }
}
