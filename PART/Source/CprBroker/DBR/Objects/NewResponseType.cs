using CprBroker.Providers.CPRDirect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public partial class NewResponseType
    {
        public INewResponseData Data { get; set; }

        public override string ToString()
        {
            return this.ContentsWithSeparator()
                + this.Data.ToString();
        }
    }

}
