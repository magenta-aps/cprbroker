using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR
{
    public class DiversionRequest
    {
        public InquiryType inquiryType;
        public DetailType detailType;
        public string cprNumber;

        public static DiversionRequest Parse(byte[] message)
        {
            return Parse(Constants.DiversionEncoding.GetString(message));
        }

        public static DiversionRequest Parse(string str)
        {
            DiversionRequest ret = null;
            if (str.Length == 12)
            {
                ret = new DiversionRequest()
                {
                    inquiryType = (InquiryType)int.Parse(str[0].ToString()),
                    detailType = (DetailType)int.Parse(str[1].ToString()),
                    cprNumber = str.Substring(2),
                };
            }
            else if (str.Length == 40)
            {
                ret = new DiversionRequestV3()
                {
                    inquiryType = (InquiryType)int.Parse(str[0].ToString()),
                    detailType = (DetailType)int.Parse(str[1].ToString()),
                    cprNumber = str.Substring(2),
                    
                };
            }
            return ret;
        }

        public virtual string Process()
        {
            throw new NotImplementedException();
        }
    }
    
    public class DiversionRequestV3 : DiversionRequest
    {

        public override string Process()
        {
            return base.Process();
        }
    }
}
