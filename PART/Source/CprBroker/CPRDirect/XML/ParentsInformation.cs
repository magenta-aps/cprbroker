using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ParentsInformationType
    {
        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            return ToFatherOrMother(cpr2uuidFunc, this.ToFatherPnr(), this.ToFatherDate());
        }

        public PersonRelationType[] ToMather(Func<string, Guid> cpr2uuidFunc)
        {
            return ToFatherOrMother(cpr2uuidFunc, this.ToMotherPnr(), this.ToMotherDate());
        }

        public PersonRelationType[] ToFatherOrMother(Func<string, Guid> cpr2uuidFunc, string parentPnr, DateTime? parentDate)
        {
            if (!string.IsNullOrEmpty(parentPnr))
            {
                return PersonRelationType.CreateList(
                    cpr2uuidFunc(parentPnr),
                    parentDate,
                    null
                );
            }
            return new PersonRelationType[0];
        }

        public string ToFatherPnr()
        {
            return Converters.ToPnrStringOrNull(this.FatherPNR);
        }

        public DateTime? ToFatherDate()
        {
            return Converters.ToDateTime(this.FatherDate, this.FatherDateUncertainty);
        }

        public string ToMotherPnr()
        {
            return Converters.ToPnrStringOrNull(this.MotherPNR);
        }

        public DateTime? ToMotherDate()
        {
            return Converters.ToDateTime(this.MotherDate, this.MotherDateUncertainty);
        }
    }
}
