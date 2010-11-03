using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class ForeignCitizenData
    {
        public Schemas.Part.ForeignCitizenData ToXmlType()
        {
            return new Schemas.Part.ForeignCitizenData()
                {
                    ForeignNumber = this.ForeignNumber,
                    NationalityCountryCode = this.NationalityCountryAlpha2Code,
                    PermissionNumber = this.PermissionNumber
                };
        }
    }
}
