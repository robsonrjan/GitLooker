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
        private WSManConnectionInfo connectionInfo;
        private PowerShell powerShell;
        private bool isInitialized;

        public PowersShell()
        {
            connectionInfo = new WSManConnectionInfo();
            connectionInfo.SkipRevocationCheck = true;
            connectionInfo.SkipCNCheck = true;
        }

        private void InitializeConnection()
        {
            powerShell = PowerShell.Create();

            if (powerShell.Runspace != null)
            {
                powerShell.Runspace.Dispose();
            }
            powerShell.Runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            powerShell.Runspace.Open();
            isInitialized = true;
        }

        private void DisposePowersShell()
        {
            if (powerShell != null)
            {
                if (powerShell.Runspace != null)
                {
                    powerShell.Runspace.Close();
                    powerShell.Runspace.Dispose();
                }
                powerShell.Dispose();
                powerShell = null;
            }
            isInitialized = false;
        }

        public Collection<PSObject> Execute(string command)
        {
            if (!isInitialized)
                InitializeConnection();

            try
            {
                powerShell.Streams.Error.Clear();
                powerShell.Commands.Clear();

                powerShell.AddScript(command);
                return powerShell.Invoke();
            }
            catch (Exception)
            {
                DisposePowersShell();
            }
            return null;
        }
    }
}
