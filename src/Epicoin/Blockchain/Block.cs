﻿using System;
using System.Collections.Generic;
using Epicoin.Library.Tools;

namespace Epicoin.Library.Blockchain
{
    public class Block
    {
        public const int nb_trans = 3;

        protected int index;
        protected long timestamp;
        protected List<Transaction> data;
        protected string previousHash;
        protected string hashblock;
        public int nonce = -1;


        public int Index
        {
            get => index;
            set => index = value;
        }

        public long Timestamp
        {
            get => timestamp;
            set => timestamp = value;
        }

        public List<Transaction> Data
        {
            get => data;
            set => data = value;
        }

        public string PreviousHash
        {
            get => previousHash;
            set => previousHash = value;
        }

        public string Hashblock
        {
            get => hashblock;
            set => hashblock = value;
        }


        public Block(int index, long timestamp, List<Transaction> data = null, string previousHash = "")
        {
            this.index = index;
            this.timestamp = timestamp;
            this.data = data ?? new List<Transaction>();
            this.previousHash = previousHash;
        }

        public string CalculateHash()
        {
            string serialyzedata = "{";
            for (int i = 0; i < this.data.Count; i++)
            {
                serialyzedata += this.data[i].ToString();
                if (i < this.data.Count - 1)
                {
                    serialyzedata += " ; ";
                }
            }

            serialyzedata += "}";

            string hash = Hash.CpuGenerate(this.index.ToString() + this.timestamp + serialyzedata + this.previousHash +
                                           this.nonce);
            return hash;
        }

        public string CalculateHashGpu()
        {
            throw new NotImplementedException("Block.CalculateHashGpu: TO DO");
        }

        public void AddPreviousHash(string h)
        {
            this.previousHash = h;
        }

        public void MineBlock(int difficulty)
        {
            string hash = this.CalculateHash();
            string target = "";
            for (int i = 0; i < difficulty; i++)
            {
                target += "0";
            }

            while (hash.Substring(0, difficulty) != target)
            {
                this.nonce++;
                hash = this.CalculateHash();
            }

            this.hashblock = hash;
        }


        public void AddTransaction(Transaction t)
        {
            if (this.data.Count < nb_trans)
            {
                this.data.Add(t);
            }
        }

        public bool IsFull()
        {
            return this.data.Count == nb_trans;
        }
    }
}