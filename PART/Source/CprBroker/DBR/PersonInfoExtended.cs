using CprBroker.Providers.DPR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public class PersonInfoExtended : PersonInfo
    {
        public Person Person { get; set; }
        public Disappearance Disappearance { get; set; }
    }
}
