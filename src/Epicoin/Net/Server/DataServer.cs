﻿using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Epicoin.Library.Net.Server
{
    public static class DataServer
    {
        public static IPAddress Address { get; set; }
        public static int Port { get; set; }

        public static List<string> PeerList { get; set; }

        public static bool Continue { get; set; }

        public static List<DataTcpClient> Clients { get; set; }
        public static Queue<DataTcpClient> Tasks { get; set; }

        public static Socket _sock { get; set; }

        public static Blockchain.Blockchain Chain { get; set; }

        public static void Initialize(IPAddress hosAddress, int port, Blockchain.Blockchain chain)
        {
            Address = hosAddress;
            Port = port;
            Continue = true;
            Clients = new List<DataTcpClient>();
            Tasks = new Queue<DataTcpClient>();
            _sock = new Socket(hosAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Chain = chain;
            PeerList = new List<string>() {"epicoin.ddns.net"};
        }
    }
}