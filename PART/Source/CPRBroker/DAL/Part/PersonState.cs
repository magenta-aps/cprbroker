using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class PersonState
    {
        public Schemas.Part.PersonStates ToXmlType()
        {
            return new Schemas.Part.PersonStates()
                {
                    CivilStatus = new Schemas.Part.Effect<CPRBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = this.MaritalStatusStartDate,
                        EndDate = this.MaritalStatusEndDate,
                        // TODO: Handle null values for marital status
                        Value = MaritalStatusType.GetPartMaritalStatus(this.MaritalStatusTypeId.Value)
                    },
                    LifeStatus = new Schemas.Part.Effect<CPRBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        StartDate = this.LifeStatusStartDate,
                        EndDate = this.LifeStatusStartDate,
                        // TODO: Handle null values for life status
                        Value = LifeStatusType.GetPartLifeStatus(this.LifeStatusTypeId.Value)
                    }
                };
        }
    }
}
