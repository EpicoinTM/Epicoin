using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Epicoin.Library.Blockchain;
using Epicoin.Library.Container;

namespace Epicoin.Library.Net.Client
{
    public static class Network
    {

        public static void Connect(string address, int port)
        {
            DataClient.Initialize(address, port);
            try
            {
                DataClient.Client.Client.Connect(DataClient.Address, DataClient.Port);
            }
            catch (Exception)
            {
                DataClient.Client.Client.Connect(DataClient.IpAddressEntry.AddressList, port);
            }
        }
        
        private static Protocol ReceiveMessage()
        {
            while (DataClient.Client.Client.Connected && DataClient.Client.Client.Available <= 1)
            {
            }

            if (!DataClient.Client.Client.Connected)
                throw new Exception("Disconnected from server.");
            var message = new List<byte>();
            var stream = DataClient.Client.GetStream();

#pragma warning disable CS0652
            while (stream.DataAvailable)
                message.Add((byte) stream.ReadByte());
#pragma warning restore CS0652

            var msg = Formatter.ToObject<Protocol>(message.ToArray());
            return msg;
        }

        public static Block AskBlockNumber(int n)
        {
            Protocol reqProtocol = new Protocol(MessageType.AskBlocknumber) {Message = n.ToString()};
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? null : receiveMessage.Block;
        }
        
        public static DataMine AskBlockToMine()
        {
            Protocol reqProtocol = new Protocol(MessageType.AskBlockToMine);
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? null : receiveMessage.Mine;
        }
        
        public static Blockchain.Blockchain AskChain()
        {
            int lenght = AskChainStats().Lenght;
            Blockchain.Blockchain chain = new Blockchain.Blockchain("");
            
            for (int i = 0; i < lenght; i++)
            {
                Block b = AskBlockNumber(i);
                if (b != null)
                {
                    chain.AddBlock(b);
                }
            }

            return chain;
        }
        
        public static Block AskLatestBlock()
        {
            Protocol reqProtocol = new Protocol(MessageType.AskLastestBlock);
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? null : receiveMessage.Block;
        }
        
        public static string SendMinedBlock(DataMine mine)
        {
            Protocol reqProtocol = new Protocol(MessageType.MinedBlock) {Mine = mine};
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? receiveMessage.Message : receiveMessage.Message;
        }

        public static string SendTransaction(DataTransaction trans)
        {
            Protocol reqProtocol = new Protocol(MessageType.Transaction) {Transaction = trans};
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? receiveMessage.Message : receiveMessage.Message;
        }

        public static DataChainStats AskChainStats()
        {
            Protocol reqProtocol = new Protocol(MessageType.AskChainStats);
            byte[] buffer = Formatter.ToByteArray(reqProtocol);
            DataClient.Client.Client.Send(buffer, SocketFlags.None);
            Protocol receiveMessage = ReceiveMessage();
            return receiveMessage.Type != MessageType.Response ? null : receiveMessage.Stats;
        }
    }
}