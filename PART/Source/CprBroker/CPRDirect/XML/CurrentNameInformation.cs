using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentNameInformationType
    {
        public NavnStrukturType ToNavnStrukturType()
        {
            return NavnStrukturType.Create(
                new string[]{
                    this.ToFirstName(), 
                    this.ToMiddleName(), 
                    this.ToLastName()
                },
                this.AddressingName);
        }

        public string ToFirstName()
        {
            return Converters.ToString(this.FirstName_s, this.FirstNameMarker);
        }

        public string ToMiddleName()
        {
            return Converters.ToString(this.MiddleName, this.MiddleNameMarker);
        }

        public string ToLastName()
        {
            return Converters.ToString(this.LastName, this.LastNameMarker);
        }

        public DateTime? ToNameStartDate()
        {
            return Converters.ToDateTime(this.NameStartDate, this.NameStartDateUncertainty);
        }
    }
}
