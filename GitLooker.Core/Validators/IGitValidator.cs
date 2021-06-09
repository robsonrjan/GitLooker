namespace GitLooker.Core.Validators
{
    public interface IGitValidator
    {
        bool TryToFind(string configPath, out GitConfigInfo gitInfo);
    }
}
