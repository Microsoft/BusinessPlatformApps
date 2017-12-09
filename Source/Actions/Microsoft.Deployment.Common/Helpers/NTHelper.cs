using System;

using Microsoft.Win32;
using System.Diagnostics;

namespace Microsoft.Deployment.Common.Helpers
{
    public class NTHelper
    {
        public static string CleanDomain(string domain)
        {
            string cleanedDomain = domain;

            if (string.IsNullOrEmpty(domain))
            {
                cleanedDomain = Environment.GetEnvironmentVariable("USERDOMAIN");
            }
            else if (domain.EqualsIgnoreCase("."))
            {
                cleanedDomain = Environment.MachineName;
            }

            return cleanedDomain;
        }

        public static string CleanUsername(string username)
        {
            string cleanedUsername = username;

            if (string.IsNullOrEmpty(username))
            {
                cleanedUsername = Environment.GetEnvironmentVariable("USERNAME");
            }

            return cleanedUsername;
        }

        private static bool IsCredentialGuardEnabledAtLocation(string registryKey)
        {
            // Guard
            if (string.IsNullOrEmpty(registryKey))
                return false;

            bool result = false;
            RegistryKey rk = null; // Use this pattern instead of nesting a try with using statement
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(registryKey);
                if (rk != null)
                {
                    object v = rk.GetValue("LsaCfgFlags");
                    int intVal = Convert.ToInt32(v); // If conversion fails, we go to the catch block
                    result = (intVal == 1 || intVal == 2);
                }
            }
            catch { /* Nothing to do */ }
            finally
            {
                rk?.Dispose();
            }

            return result;
        }


        public static bool IsCredentialGuardEnabled()
        {
            bool credentialGuardEnabled = false;
            int[] osVersion = NTHelper.OsVersionArray;

            if (osVersion[0] == 10 && osVersion[1] == 0 && osVersion[2] < 15011)
            {
                credentialGuardEnabled = IsCredentialGuardEnabledAtLocation("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard")  || 
                                         IsCredentialGuardEnabledAtLocation("SYSTEM\\CurrentControlSet\\Control\\Lsa");  // Second check won't run if GP enabled it
            }

            return credentialGuardEnabled;
        }

        public static int[] OsVersionArray
        {
            get
            {
                int[] result = new int[3];

                using (RegistryKey k = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion"))
                {
                    result[0] = Convert.ToInt32(k.GetValue("CurrentMajorVersionNumber"));
                    result[1] = Convert.ToInt32(k.GetValue("CurrentMinorVersionNumber"));
                    result[2] = Convert.ToInt32(k.GetValue("CurrentBuildNumber") ?? k.GetValue("CurrentBuild"));
                }

                return result;
            }
        }

        public static string OsVersion
        {
            get
            {
                return string.Join(".", NTHelper.OsVersionArray);
            }
        }

        public static void KillProcess(string processName)
        {
            foreach (Process p in Process.GetProcessesByName(processName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(10000);
                }
                catch { }

            }
        }
    }
}