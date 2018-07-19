using System;
using Mono.Nat;

namespace Epicoin.Library.Net.Server
{
    public static class UPnP
    {
        private static int[] _port;

        public static void init(int[] port)
        {
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.DeviceLost += DeviceLost;
            NatUtility.StartDiscovery();
            UPnP._port = port;
        }

        private static void DeviceFound(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;
            foreach (var port in UPnP._port)
            {
                device.CreatePortMap(new Mapping(Mono.Nat.Protocol.Tcp, port, port));
            }

            foreach (Mapping portMap in device.GetAllMappings())
            {
                Console.WriteLine(portMap.ToString());
            }

            Console.WriteLine(device.GetExternalIP().ToString());
        }

        private static void DeviceLost(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;
            foreach (var port in UPnP._port)
            {
                device.DeletePortMap(new Mapping(Mono.Nat.Protocol.Tcp, port, port));
            }
        }
    }
}