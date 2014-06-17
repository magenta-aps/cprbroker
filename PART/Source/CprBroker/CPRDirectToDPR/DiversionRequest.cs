using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Schemas.Wrappers;

namespace CprBroker.DBR
{
    public abstract class DiversionRequest : Wrapper
    {
        public static DiversionRequest Parse(byte[] message)
        {
            return Parse(Constants.DiversionEncoding.GetString(message));
        }

        public static DiversionRequest Parse(string str)
        {
            DiversionRequest ret = null;
            if (str.Length == 12)
            {
                ret = new ClassicRequestType() { Contents = str };
            }
            else if (str.Length == 40)
            {
                ret = new NewRquestType() { Contents = str };
            }

            return ret;
        }

        public virtual DiversionResponse Process()
        {
            throw new NotImplementedException();
        }
    }

}
