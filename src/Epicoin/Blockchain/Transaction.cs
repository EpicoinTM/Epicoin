﻿using System;

namespace Epicoin.Library.Blockchain
{
    public class Transaction
    {
        public string FromAddress;
        public string ToAddress;
        public int Amount;
        public string Timestamp;

        public Transaction(string fromAddress, string toAddress, int amount)
        {
            this.FromAddress = fromAddress;
            this.ToAddress = toAddress;
            this.Amount = amount;
            this.Timestamp = DateTime.Now.ToString();
        }

        public override string ToString()
        {
            return "(Transaction){ at " + this.Timestamp + " from " + (FromAddress ?? Blockchain.Name) + " ; to " +
                   this.ToAddress + " : " + this.Amount + " }";
        }
    }
}