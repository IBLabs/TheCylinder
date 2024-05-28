using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using UnityEngine;

namespace Utilities
{
    public class ItaNetworkUtilities
    {
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;

            // Get the list of all network interfaces
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                // Skip interfaces that are not operational
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                    continue;

                // Get the list of unicast IP addresses associated with the network interface
                var unicastIPAddresses = networkInterface.GetIPProperties().UnicastAddresses;

                foreach (var unicastIPAddress in unicastIPAddresses)
                {
                    // Check if the IP address is IPv4 and not a loopback address
                    if (unicastIPAddress.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastIPAddress.Address))
                    {
                        localIP = unicastIPAddress.Address.ToString();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(localIP))
                    break;
            }

            if (string.IsNullOrEmpty(localIP))
            {
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

            return localIP;
        }

        public static bool ValidateIPAddress(string ipAddress)
        {
            string[] ipParts = ipAddress.Split('.');
            if (ipParts.Length != 4)
            {
                return false;
            }

            foreach (string part in ipParts)
            {
                if (!int.TryParse(part, out int ipPart))
                {
                    return false;
                }

                if (ipPart < 0 || ipPart > 255)
                {
                    return false;
                }
            }

            return true;
        }
    }
}