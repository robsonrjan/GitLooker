using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace GitLooker
{
    public class PowersShell : IPowersShell
    {
        private PowerShell powerShell;
        private bool isInitialized;
        private Pipeline pipeLine;

        private void InitializeConnection()
        {
            powerShell = PowerShell.Create();
            pipeLine = powerShell.Runspace.CreatePipeline();
            isInitialized = true;
        }

        private void DisposePowersShell()
        {
            if (powerShell != null)
            {
                pipeLine.Stop();
                powerShell.Dispose();
                powerShell = null;
            }
            isInitialized = false;
        }

        public IEnumerable<string> Execute(string command, bool closeConnectionAfter = true)
        {
            if (!isInitialized)
                InitializeConnection();

            try
            {
                powerShell.Streams.Error.Clear();
                pipeLine.Commands.Clear();
                IEnumerable<string> returnValue;

                pipeLine.Commands.AddScript(command);
                returnValue = pipeLine.Invoke()?.Select(p => p.ToString());

                if (pipeLine.HadErrors)
                    returnValue = pipeLine.Error.ReadToEnd().Select(err => err.ToString());

                if (closeConnectionAfter)
                    DisposePowersShell();

                return returnValue;
            }
            catch (Exception err)
            {
                DisposePowersShell();
            }
            return null;
        }
    }
}
