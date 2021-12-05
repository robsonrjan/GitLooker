namespace GitLooker.Core.Repository
{
    public interface IGitFileRepo
    {
        IEnumerable<string> Get(string chosenPath);
    }
}
