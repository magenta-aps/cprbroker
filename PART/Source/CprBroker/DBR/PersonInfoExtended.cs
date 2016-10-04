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

        public PersonInfoExtended Load(DPRDataContext dataContext, decimal pnr)
        {
            var personTotal = dataContext.PersonTotals.SingleOrDefault(t => t.PNR == pnr);
            var ret = personTotal.ToPersonInfo<PersonInfoExtended>();

            ret.Person = dataContext.Persons.SingleOrDefault(p => p.PNR == pnr);
            ret.Disappearance = dataContext.Disappearances.SingleOrDefault(d => d.PNR == pnr && d.CorrectionMarker == null);

            return ret;
        }
    }
}
