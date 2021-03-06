﻿using System.Collections.Generic;
using Epicoin.Library.Tools;
using Epicoin.Library.Container;

namespace Epicoin.Library.Blockchain
{
    public class Wallet
    {
        public string Name;

        public List<string> Address;

        public List<string> PrivKey;
        public List<string> PublicKey;

        public List<int> Amount;

        public Wallet(string name)
        {
            this.Name = name;
            this.Address = new List<string>();
            this.PrivKey = new List<string>();
            this.PublicKey = new List<string>();
            this.Amount = new List<int>();
        }

        public string GenNewAddress()
        {
            string[] data = Rsa.GenKey(2048);
            this.PrivKey.Add(data[0]);
            this.PublicKey.Add(data[1]);
            string address = Hash.CpuGenerate(data[1]);
            this.Address.Add(address);
            return address;
        }

        public void GetAmount()
        {
            this.Amount = new List<int>();
            Blockchain chain = Epicoin.Client.GetBlockchain();
            if (chain == null)
            {
                return;
            }

            foreach (var address in this.Address)
            {
                this.Amount.Add(chain.GetBalanceOfAddress(address));
            }
        }

        public int TotalAmount()
        {
            this.GetAmount();
            int amount = 0;
            foreach (var amountAddress in this.Amount)
            {
                amount += amountAddress;
            }

            return amount;
        }

        public List<DataTransaction> GenTransactions(int amount, string toAddress)
        {
            this.GetAmount();
            List<DataTransaction> transList = new List<DataTransaction>();
            if (amount > this.TotalAmount())
            {
                return transList;
            }

            int i = 0;
            while (amount > 0 && i < this.Address.Count)
            {
                int tmpamount;
                if (this.Amount[i] > amount)
                {
                    tmpamount = amount;
                    amount = 0;
                }
                else
                {
                    tmpamount = this.Amount[i];
                    amount -= this.Amount[i];
                }

                DataTransaction datatrans = new DataTransaction(
                    this.PublicKey[i], /*
                    Rsa.Encrypt(this.PrivKey[i], this.Address[i]), 
                    Rsa.Encrypt(this.PrivKey[i], toAddress), 
                    Rsa.Encrypt(this.PrivKey[i], tmpamount.ToString())
                    */
                    this.Address[i],
                    toAddress,
                    tmpamount.ToString()
                );
                transList.Add(datatrans);
            }

            return transList;
        }

        public string Export()
        {
            string wallet = Serialyze.Serialize(this);
            return wallet;
        }
    }
}