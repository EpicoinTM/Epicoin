using System.Collections.Generic;

namespace Epicoin.Library.Tools
{
    public class Logger
    {
        private List<string> _buffer;
        private const int Bufferlenght = 20;

        public Logger()
        {
            this._buffer = new List<string>();
        }

        public void Write(string log)
        {
            this._buffer.Add(log);
            if (this._buffer.Count > Bufferlenght)
            {
                this._buffer.RemoveAt(0);
            }
        }

        public string Read()
        {
            string log = "";
            foreach (var message in this._buffer)
            {
                log += message + "\n";
            }

            return log;
        }

        public string Pop()
        {
            string text = Read();
            this._buffer = new List<string>();
            return text;
        }
    }
}