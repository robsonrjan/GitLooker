using GitLooker.Core;
using GitLooker.Core.Services;
using System.Diagnostics;

namespace GitLooker.Services.CommandProcessor
{
    public class ProcessShell : IProcessShell
    {
        public IEnumerable<string> Execute(IEnumerable<Command> commands)
        {
            try
            {
                var result = new List<string>();
                foreach (var command in commands)
                {
                    var pr = new Process();
                    pr.StartInfo = new ProcessStartInfo(command.Exec, command.Args);
                    pr.StartInfo.UseShellExecute = false;
                    pr.StartInfo.RedirectStandardOutput = true;
                    pr.StartInfo.RedirectStandardError = true;
                    pr.StartInfo.CreateNoWindow = true;
                    pr.Start();

                    var reader = pr.StandardOutput;
                    var errorReader = pr.StandardError;
                    string output;

                    while ((output = reader.ReadLine()) != default)
                        result.Add(output);

                    while ((output = errorReader.ReadLine()) != default)
                        result.Add(output);

                    pr.WaitForExit();
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
