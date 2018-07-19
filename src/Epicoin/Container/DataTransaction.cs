using System;

namespace Epicoin.Library.Container
{
    [Serializable]
    public class DataTransaction
    {
        public string PublicKey;
        public string EncodeFromAddress;
        public string ToAddress;
        public string Amount;

        public DataTransaction(string publicKey, string encodeFromAddress, string toAddress, string amount)
        {
            this.PublicKey = publicKey;
            this.EncodeFromAddress = encodeFromAddress;
            this.ToAddress = toAddress;
            this.Amount = amount;
        }
    }
}