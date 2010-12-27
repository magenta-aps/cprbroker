using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Part
{
    public partial class PersonState
    {
        public Schemas.Part.PersonStates ToXmlType()
        {
            return new Schemas.Part.PersonStates()
                /*
                    {
                        CivilStatus = new Schemas.Part.Effect<CprBroker.Schemas.Part.Enums.MaritalStatus>()
                        {
                            StartDate = this.MaritalStatusStartDate,
                            EndDate = this.MaritalStatusEndDate,
                            // TODO: Handle null values for marital status
                            Value = MaritalStatusType.GetPartMaritalStatus(this.MaritalStatusTypeId.Value)
                        },
                        LifeStatus = new Schemas.Part.Effect<CprBroker.Schemas.Part.Enums.LifeStatus>()
                        {
                            StartDate = this.LifeStatusStartDate,
                            EndDate = this.LifeStatusStartDate,
                            // TODO: Handle null values for life status
                            Value = LifeStatusType.GetPartLifeStatus(this.LifeStatusTypeId.Value)
                        }
                    }
                 */
               ;
        }

        public static PersonState FromXmlType(Schemas.Part.PersonStates partState)
        {
            // TODO: Implement PersonState.FromXmlType
            return new PersonState();
        }
    }
}
