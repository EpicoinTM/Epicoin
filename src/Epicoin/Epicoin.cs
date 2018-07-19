using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Epicoin.Library.Net.Client;
using Epicoin.Library.Net.Server;
using Epicoin.Library.Container;
using Epicoin.Library.Tools;
using Epicoin.Library.Blockchain;

namespace Epicoin.Library
{
    public class Epicoin
    {
        public static Blockchain.Blockchain Coin = null;
        
        public static bool Continue = true;

        public static string host = "epicoin.ddns.net"; //IPAddress.Loopback.ToString();
        public static readonly int port = 4248;

        public static readonly string walletfile = "wallet.epi";
        public static readonly string blockchainfile = "blockchain.epi";
        
        public static Server server = null;
        public static Client client = null;

        public static Wallet Wallet = null;

        public static Logger log;

        public static void Serveur(string namearg = null)
        {
            host = "localhost";
            ImportWallet();
            if (Wallet == null)
            {
                string name = "";
                if (namearg == null)
                {
                    Console.Write("Your name : ");
                    name = ReadLine();
                }
                else
                {
                    name = namearg;
                }

                CreateWallet(name);   
                ExportWallet();
            }
            
            
            Console.WriteLine("\nYour epicoin address : " + Wallet.Address[0] + "\n\n");
            
            
            Console.WriteLine("Init Blockchain ...");
            ImportChain();
            if (Coin == null)
            {
                Init();
                ExportChain();
            }
            
            server = new Server(port, Coin);
            
            
            Thread block = new Thread(CreateBlock) {Priority = ThreadPriority.Highest};
            Thread ThServer = new Thread(server.Start) {Priority = ThreadPriority.Highest};
            Thread saveChain = new Thread(SaveBlockchain) {Priority = ThreadPriority.Lowest};

            block.Start();
            ThServer.Start();
            saveChain.Start();
            Console.WriteLine("\nAll serveur online\n\n\n");
            
            
            
            client = new Client(host, port);
            while (true)
            {               
            }

            Continue = false;
            DataClient.Continue = false;
            DataServer.Continue = false;
        }
        
        public static void Miner(string namearg = null)
        {
            Console.WriteLine("\n\n        Blochain Epicoin Miner Client \n\n");
            
            Console.WriteLine("Host: [Default: " + host + "]\nChoice: ");
            host = ReadLine();
            Console.WriteLine("\n");
            
            client = new Client(host, port);
            
            ImportWallet();
            if (Wallet == null)
            {
                string name = "";
                if (namearg == null)
                {
                    Console.Write("Your name : ");
                    name = ReadLine();
                }
                else
                {
                    name = namearg;
                }

                CreateWallet(name);  
                ExportWallet();
            }

            Console.WriteLine("\nYour epicoin address : " + Wallet.Address[0] + "\n\n");
            
            Console.WriteLine("\n\n Enter to stop miner ...\n");
            
            Console.WriteLine("Start miner ...");

            Thread worker = new Thread(Mine) {Priority = ThreadPriority.Highest};
            worker.Start(Wallet.Address[0]);

            ReadLine();
            
            Continue = false;
            DataClient.Continue = false;
            DataServer.Continue = false;

            worker.Abort();
            worker = null;
            
            Console.WriteLine("\nbye!");
        }

        public static void User(string namearg = null)
        {
            Console.WriteLine("\n\n        Blochain Epicoin Client \n\n");
            
            Console.WriteLine("Host: [Default: " + host + "]\nChoice: ");
            host = ReadLine();
            Console.WriteLine("\n");
            
            ImportWallet();
            if (Wallet == null)
            {
                string name = "";
                if (namearg == null)
                {
                    Console.Write("Your name : ");
                    name = ReadLine();
                }
                else
                {
                    name = namearg;
                }

                CreateWallet(name);
                ExportWallet();
            }

            client = new Client(host, port);
            
            Console.WriteLine("\nYour epicoin address : " + Wallet.Address[0] + "\n\n");

            while (Continue)
            {
                Console.WriteLine("\n      MENU Client");
                Console.WriteLine();
                Console.WriteLine("1 : exit");
                Console.WriteLine("2 : Export wallet");
                Console.WriteLine("3 : Get Chain Stats");
                Console.WriteLine("4 : Get Wallet Stats");
                Console.WriteLine("5 : Generate a new address");
                Console.WriteLine("6 : Create Transaction");   
                Console.WriteLine();
                
                Console.Write("action : ");
                string action = ReadLine();
                
                Console.WriteLine();
                
                if (action == "1")
                {
                    Continue = false;
                    DataClient.Continue = false;
                    DataServer.Continue = false;
                    ExportWallet();
                    break;
                }
                else if (action == "2")
                {
                    ExportWallet();
                    Console.WriteLine("Wallet exported in " + Epicoin.walletfile);
                }
                else if (action == "3")
                {
                    DataChainStats stats = client.GetChainStats();
                    if (stats != null)
                    {
                        Console.WriteLine("     Chain " + Blockchain.Blockchain.Name);
                        Console.WriteLine("Chain is valid : " + stats.Valid);
                        Console.WriteLine("Chain lenght : " + stats.Lenght);
                        Console.WriteLine("Chain difficulty : " + stats.Difficulty);
                        Console.WriteLine("Last Block " + stats.LastIndex + " : " + stats.LastBlockHash);
                        Console.WriteLine("pending Transaction : " + stats.Pending);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }

                }
                else if (action == "5")
                {
                    string newaddress = Wallet.GenNewAddress();
                    ExportWallet();
                    Console.WriteLine("New Epicoin Address : " + newaddress);
                }
                else if (action == "4")
                {
                    Console.WriteLine("     Wallet : " + Wallet.Name);
                    Console.WriteLine("Your epicoin address : ");
                    foreach(var address in Wallet.Address)
                    {
                        Console.WriteLine(address);
                    }
                    Console.WriteLine("Epicoin amount : " + Wallet.TotalAmount());
                }
                else if (action == "6")
                {
                    Console.Write("ToAddress : ");
                    string ToAddress = ReadLine();
                    Console.Write("\nAmount : ");
                    string Samount = ReadLine();
                    int amount = 0;
                    try
                    {
                        amount = int.Parse(Samount);
                        if (amount <= 0)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid amount");
                        continue;
                    }

                    List<DataTransaction> ltrans = Wallet.GenTransactions(amount, ToAddress);
                    string display = "";
                    foreach (var trans in ltrans)
                    {
                        display += client.SendTransaction(trans) + "\n";
                    }
                    Console.WriteLine(display);
                }
                else
                {
                    Console.WriteLine("Entrée incorrect");
                }
                action = "";
            }
            Console.WriteLine("\nbye!");
        }

        public static string ReadLine()
        {
            string data = "";
            while ((data = Console.ReadLine()) == "");
            return data;
        }
        
        public static void Init()
        {
            Coin = new Blockchain.Blockchain(Wallet.Address[0]);
            Coin.AddBlock(Coin.CreateGenesisBlock());
        }        

        public static void CreateBlock()
        {
            while (Continue)
            {
                Coin.CreateBlock();
                Thread.Sleep(Coin.timebtwblock * 1000);
            }
            return;
        }

        public static void Mine(object o)
        {
            client.Mine((string)o);
        }
        
        public static void CreateWallet(string name)
        {
            ImportWallet();
            if (Wallet == null)
            {
                Wallet = new Wallet(name);
                Wallet.GenNewAddress();
            }
        }
        
        public static void ExportWallet()
        {
            string w = Wallet.Export();
            try
            {
                File.WriteAllText(Epicoin.walletfile, w);
            }
            catch (Exception)
            { }
            
        }

        public static void ImportWallet()
        {
            if (File.Exists(Epicoin.walletfile))
            {
                string wa = File.ReadAllText(Epicoin.walletfile);
                Wallet w = Serialyze.UnserializeWallet(wa);
                Wallet = w;
            }
        }

        public static void ExportChain()
        {
            string chain = Serialyze.Serialize(Coin);
            try
            {
                File.WriteAllText(Epicoin.blockchainfile, chain);
            }
            catch(Exception)
            { }
            
        }

        public static void ImportChain()
        {
            if (File.Exists(Epicoin.blockchainfile))
            {
                string chain = File.ReadAllText(Epicoin.blockchainfile);
                Blockchain.Blockchain c = Serialyze.UnserializeBlockchain(chain);
                Coin = c;
            }
        }

        public static void SaveBlockchain()
        {
            int time = 5 * 60 * 1000;
            while (Epicoin.Continue)
            {
                try
                {
                    ExportChain();
                }
                catch (Exception)
                {
                }
                
                Thread.Sleep(time);
            }
            return;
        }
    }
}
