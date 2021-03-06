﻿using System.Net;
using System.Net.Sockets;

namespace Epicoin.Library.Net.Client
{
    public static class DataClient
    {
        public static string Address { get; set; }
        public static IPHostEntry IpAddressEntry { get; set; }
        public static int Port { get; set; }

        public static bool Continue { get; set; }

        public static TcpClient Client { get; set; }

        public static void Initialize(string hosAddress, int port)
        {
            Address = hosAddress;
#pragma warning disable 618
            IpAddressEntry = Dns.Resolve(hosAddress);
#pragma warning restore 618
            Port = port;
            Continue = true;
            Socket _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Client = new TcpClient() {Client = _sock};
        }
    }
}