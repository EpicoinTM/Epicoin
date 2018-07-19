using System;
using Epicoin.Library.Blockchain;
using Epicoin.Library.Tools;
using Epicoin.Library.Container;

namespace Epicoin.Library.Net.Server
{
    public static class RequestServer
    {
        public static Protocol Transaction(Protocol protocol)
        {
            DataTransaction transaction = protocol.Transaction;
            if (transaction == null)
            {
                return new Protocol(MessageType.Error) {Message = "Bad Transaction"};
            }

            if (transaction.EncodeFromAddress != Hash.CpuGenerate(transaction.PublicKey))
            {
                return new Protocol(MessageType.Error) {Message = "Bad sender address"};
            }

            int transAmount = 0;
            try
            {
                transAmount = int.Parse(transaction.Amount);
            }
            catch (Exception)
            {
                return new Protocol(MessageType.Error) {Message = "Bad amount"};
            }

            int amount = DataServer.Chain.GetBalanceOfAddress(transaction.EncodeFromAddress);

            if (transAmount > amount)
            {
                return new Protocol(MessageType.Error) {Message = "You do not have enough epicoins to do that!"};
            }

            Transaction newTransaction =
                new Transaction(transaction.EncodeFromAddress, transaction.ToAddress, transAmount);
            bool success = DataServer.Chain.AddTransaction(newTransaction);

            if (success)
            {
                return new Protocol(MessageType.Response) {Message = "Success"};
            }
            else
            {
                return new Protocol(MessageType.Response) {Message = "Failed to add transaction"};
            }
        }

        public static Protocol AskChain()
        {
            Protocol resp = new Protocol(MessageType.Response);
            resp.Chain = DataServer.Chain;
            return resp;
        }

        public static Protocol AskLastestBlock()
        {
            Protocol resp = new Protocol(MessageType.AskLastestBlock);
            resp.Block = DataServer.Chain.GetLatestBlock();
            return resp;
        }

        public static Protocol AskBlockNumber(Protocol prot)
        {
            if (string.IsNullOrEmpty(prot.Message))
            {
                return new Protocol(MessageType.Error) {Message = "Invalid block number"};
            }

            try
            {
                int number = int.Parse(prot.Message);
                Block b = DataServer.Chain.Chainlist[number];
                return new Protocol(MessageType.Response) {Block = b};
            }
            catch (Exception)
            {
                return new Protocol(MessageType.Error) {Message = "Invalid block number"};
            }
        }

        public static Protocol AskBlockToMine()
        {
            if (DataServer.Chain.BlockToMines.Count == 0)
            {
                return new Protocol(MessageType.Error) {Message = "No Block to Mine"};
            }

            return new Protocol(MessageType.Response)
            {
                Mine = new DataMine(DataServer.Chain.Difficulty, DataServer.Chain.BlockToMines[0], null)
            };
        }

        public static Protocol MinedBlock(Protocol prot)
        {
            if (prot.Mine == null)
            {
                return new Protocol(MessageType.Error) {Message = "Empty block"};
            }

            DataMine dataMine = prot.Mine;
            if (dataMine.Block == null)
            {
                return new Protocol(MessageType.Error) {Message = "Empty block"};
            }

            if (dataMine.Block.Data == null)
            {
                return new Protocol(MessageType.Error) {Message = "Block invalid"};
            }

            if (DataServer.Chain.BlockToMines[0].Index != dataMine.Block.Index)
            {
                return new Protocol(MessageType.Error) {Message = "Block invalid"};
            }

            if (DataServer.Chain.BlockToMines[0].Timestamp != dataMine.Block.Timestamp)
            {
                return new Protocol(MessageType.Error) {Message = "Block invalid"};
            }

            try
            {
                for (int i = 0; i < Block.nb_trans; i++)
                {
                    if (DataServer.Chain.BlockToMines[0].Data[i].Amount != dataMine.Block.Data[i].Amount)
                    {
                        return new Protocol(MessageType.Error) {Message = "Block invalid"};
                    }

                    if (DataServer.Chain.BlockToMines[0].Data[i].FromAddress != dataMine.Block.Data[i].FromAddress)
                    {
                        return new Protocol(MessageType.Error) {Message = "Block invalid"};
                    }

                    if (DataServer.Chain.BlockToMines[0].Data[i].ToAddress != dataMine.Block.Data[i].ToAddress)
                    {
                        return new Protocol(MessageType.Error) {Message = "Block invalid"};
                    }

                    if (DataServer.Chain.BlockToMines[0].Data[i].Timestamp != dataMine.Block.Data[i].Timestamp)
                    {
                        return new Protocol(MessageType.Error) {Message = "Block invalid"};
                    }
                }
            }
            catch (Exception)
            {
                return new Protocol(MessageType.Error) {Message = "Block invalid"};
            }


            if (DataServer.Chain.BlockToMines[0].PreviousHash != dataMine.Block.PreviousHash)
            {
                return new Protocol(MessageType.Error) {Message = "Block invalid"};
            }

            bool succes = DataServer.Chain.NetworkMinePendingTransaction(dataMine.Address, dataMine.Block,
                dataMine.MiningTime, dataMine.Difficulty);

            if (succes)
            {
                return new Protocol(MessageType.Response) {Message = "Sucess"};
            }
            else
            {
                return new Protocol(MessageType.Error) {Message = "failed"};
            }
        }

        public static Protocol AskPeer()
        {
            return new Protocol(MessageType.Response) {PeerList = DataServer.PeerList};
        }

        public static Protocol SendPeer(Protocol protocol)
        {
            return new Protocol(MessageType.Error) {Message = "Not Implemented"};
        }

        public static Protocol AskChainStats()
        {
            DataChainStats stats = new DataChainStats();
            stats.Valid = DataServer.Chain.IsvalidChain();
            stats.LastBlockHash = DataServer.Chain.GetLatestBlock().Hashblock;
            stats.LastIndex = DataServer.Chain.GetLatestIndex();
            stats.Lenght = DataServer.Chain.Chainlist.Count;
            stats.Pending = DataServer.Chain.Pending.Count + (DataServer.Chain.BlockToMines.Count * Block.nb_trans);
            stats.Difficulty = DataServer.Chain.Difficulty;
            stats.Name = Blockchain.Blockchain.Name;
            return new Protocol(MessageType.Response) {Stats = stats};
        }
    }
}