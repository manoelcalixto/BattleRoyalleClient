using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyalleWindowsService
{
    class MaquinaService
    {

        public async Task<string> solicitarTokenAsync()
        {
            Maquina maquina = new Maquina();

            maquina.IP = PegarIPLocal();
            maquina.Nome = PegarNomeMaquina();
            maquina.AntiVirus = PegarAntiVirusInstalado();
            maquina.Firewall = PegarFirewallHabilitado();
            maquina.VersaoWindows = PegarVersaoWindows();
            maquina.VersaoDotNeto = PegarVersaoDotNet();

            var client = new RestClient("https://localhost:44325/api");

            var request = new RestRequest("/maquina/cadastrar")
                .AddJsonBody(maquina); ;

            var response = await client.PostAsync<string>(request);

            return response;
        }


        private string PegarIPLocal()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private string PegarNomeMaquina()
        {
            return Dns.GetHostName();
        }

        private string PegarAntiVirusInstalado()
        {
            string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
            ManagementObjectCollection instances = searcher.Get();

            foreach (ManagementObject virusChecker in instances)
            {
                return virusChecker["displayName"].ToString();
            }

            return "";
        }

        private bool PegarFirewallHabilitado()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile"))
                {
                    if (key == null)
                    {
                        return false;
                    }
                    else
                    {
                        Object o = key.GetValue("EnableFirewall");
                        if (o == null)
                        {
                            return false;
                        }
                        else
                        {
                            int firewall = (int)o;
                            if (firewall == 1)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private string PegarVersaoWindows()
        {
            return RuntimeInformation.OSDescription;
        }

        private static string PegarVersaoDotNet()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    return CheckFor45DotVersion(releaseKey);
                }
            }
        }

        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 461808)
            {
                return "4.7.2 or later";
            }
            if (releaseKey >= 461308)
            {
                return "4.7.1 or later";
            }
            if (releaseKey >= 460798)
            {
                return "4.7 or later";
            }
            if (releaseKey >= 394802)
            {
                return "4.6.2 or later";
            }
            if (releaseKey >= 394254)
            {
                return "4.6.1 or later";
            }
            if (releaseKey >= 393295)
            {
                return "4.6 or later";
            }
            if (releaseKey >= 393273)
            {
                return "4.6 RC or later";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2 or later";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1 or later";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean 
            // that 4.5 or later is installed. 
            return "No 4.5 or later version detected";
        }
    }
}
