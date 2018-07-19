using System;
using System.Collections.Generic;
using Epicoin.Library.Container;
using Epicoin.Library.Blockchain;

namespace Epicoin.Library.Net
{
    public enum MessageType
    {
        Response,
        Error,
        Transaction,
        AskChain,
        AskLastestBlock,
        AskBlocknumber,
        AskBlockToMine,
        MinedBlock,
        AskChainStats,
        AskPeer,
        SendPeer
    }

    [Serializable]
    public class Protocol
    {
        public Protocol(MessageType type)
        {
            this.Type = type;
            this.Message = "";
            this.Transaction = null;
            this.Mine = null;
            this.Block = null;
            this.Chain = null;
            this.Stats = null;
        }

        public MessageType Type { get; set; }
        public string Message { get; set; }
        public DataTransaction Transaction { get; set; }
        public DataChainStats Stats { get; set; }
        public DataMine Mine { get; set; }
        public Block Block { get; set; }
        public Blockchain.Blockchain Chain { get; set; }
        public List<string> PeerList { get; set; }
    }
}