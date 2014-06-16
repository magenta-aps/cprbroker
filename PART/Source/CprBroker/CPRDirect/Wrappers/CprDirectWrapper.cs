using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Wrappers;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class CprDirectWrapper : Wrapper
    {
        public CprDirectWrapper()
        { }

        public CprDirectWrapper(int length)
            : base(length)
        { }

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }
    }
}
