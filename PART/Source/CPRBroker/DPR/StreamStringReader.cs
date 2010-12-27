using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Utility class used to read strings as a Stream
    /// </summary>
    internal class StreamStringReader : System.IO.StringReader
    {
        public StreamStringReader(string str)
            : base(str)
        {
        }

        public string ReadNext(int length)
        {
            char[] data = new char[length];
            base.Read(data, 0, length);
            return new string(data);
        }

        public int ReadInt(int length)
        {
            string s = ReadNext(length);
            return int.Parse(s);
        }
    }
}
