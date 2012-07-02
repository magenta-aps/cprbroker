using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public class LineWrapper
    {
        public string Contents { get; set; }

        public LineWrapper(string contents)
        {
            Contents = contents;
        }

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }

        public string PNR
        {
            get { return Contents.Substring(3, 10); }
        }
    }
}
