using System;
using Epicoin.Library.Blockchain;

namespace Epicoin.Library.Container
{
    [Serializable]
    public class DataMine
    {
        public int Difficulty;
        public Block Block;
        public string Address;
        public long MiningTime;

        public DataMine(int difficulty, Block block, string address, long timeminig = 0)
        {
            this.Difficulty = difficulty;
            this.Block = block;
            this.Address = address;
            this.MiningTime = timeminig;
        }
    }
}