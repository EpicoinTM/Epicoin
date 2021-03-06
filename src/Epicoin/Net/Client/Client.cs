﻿using System;
using System.Threading;
using Epicoin.Library.Container;
using Epicoin.Library.Blockchain;
using Epicoin.Library.Tools;

namespace Epicoin.Library.Net.Client
{
    public class Client
    {
        public Client(string address, int port)
        {
            Network.Connect(address, port);
        }

        public string SendTransaction(DataTransaction transaction)
        {
            try
            {
                return Network.SendTransaction(transaction);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void Mine(string workerAddress)
        {
            Epicoin.Log = new Logger();
            while (DataClient.Continue)
            {
                Thread.Sleep(1000);
                try
                {
                    var data = Network.AskBlockToMine();
                    if (data == null)
                    {
                        Epicoin.Log.Write("[CM] No blocks to mine");
                        Console.WriteLine("[CM] No blocks to mine");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        int difficulty = data.Difficulty;
                        Block block = data.Block;
                        Epicoin.Log.Write("[CM] Mining ...");
                        Console.WriteLine("[CM] Mining ...");
                        long start = DateTime.Now.Ticks;
                        block.MineBlock(difficulty);
                        long miningtime = DateTime.Now.Ticks - start;
                        Epicoin.Log.Write("[CM] Creating Block " + block.Index + " : " + block.Hashblock
                                          + " : difficulty " + difficulty);
                        Console.WriteLine("[CM] Creating Block " + block.Index + " : " + block.Hashblock
                                          + " : difficulty " + difficulty);
                        Epicoin.Log.Write("[CM] Sending block mined ...");
                        Console.WriteLine("[CM] Sending block mined ...");
                        DataMine send = new DataMine(difficulty, block, workerAddress, miningtime);
                        string resp = Network.SendMinedBlock(send);
                        Console.WriteLine("[M] " + resp);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[M] " + e.Message);
                    continue;
                }
            }

            return;
        }

        public Blockchain.Blockchain GetBlockchain()
        {
            try
            {
                return Network.AskChain();
            }
            catch (Exception e)
            {
                Console.WriteLine("[G] " + e.Message);
                return null;
            }
        }

        public Block GetLastestBlock()
        {
            try
            {
                return Network.AskLatestBlock();
            }
            catch (Exception e)
            {
                Console.WriteLine("[G] " + e.Message);
                return null;
            }
        }

        public Block GetBlockNumber(int index)
        {
            try
            {
                return Network.AskBlockNumber(index);
            }
            catch (Exception e)
            {
                Console.WriteLine("[G] " + e.Message);
                return null;
            }
        }

        public int GetAmount(string address)
        {
            try
            {
                Blockchain.Blockchain chain = GetBlockchain();
                return chain.GetBalanceOfAddress(address);
            }
            catch (Exception e)
            {
                Console.WriteLine("[G] " + e.Message);
                return 0;
            }
        }

        public DataChainStats GetChainStats()
        {
            try
            {
                return Network.AskChainStats();
            }
            catch (Exception e)
            {
                Console.WriteLine("[G] " + e.Message);
                return null;
            }
        }
    }
}