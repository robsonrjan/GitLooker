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

        public Collection<PSObject> Execute(string command, bool closeConnectionAfter = true)
        {
            if (!isInitialized)
                InitializeConnection();

            try
            {
                powerShell.Streams.Error.Clear();
                pipeLine.Commands.Clear();

                pipeLine.Commands.AddScript(command);
                var returnValue = pipeLine.Invoke();

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
