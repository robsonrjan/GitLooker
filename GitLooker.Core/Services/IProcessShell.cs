﻿namespace GitLooker.Core.Services
{
    public interface IProcessShell
    {
        IEnumerable<string> Execute(IEnumerable<Command> commands);
    }
}
