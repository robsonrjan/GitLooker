﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

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

                IEnumerable<string> errors = default(IEnumerable<string>);

                pipeLine.Commands.AddScript(command);
                IEnumerable<string> returnValue = pipeLine.Invoke()?.Select(p => p.ToString());

                if (pipeLine.HadErrors)
                    errors = pipeLine.Error.ReadToEnd().Select(err => err.ToString()).ToList(); ;

                if ((errors != default(IEnumerable<string>)) && errors.Any())
                    returnValue = returnValue.Union(errors);

                if (closeConnectionAfter)
                    DisposePowersShell();

                return returnValue;
            }
            catch (Exception)
            {
                DisposePowersShell();
            }
            return null;
        }
    }
}
