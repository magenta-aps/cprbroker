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

        public static ForeignCitizenData FromXmlType(Schemas.Part.ForeignCitizenData partForeignData)
        {
            // TODO: Implement ForeignCitizenData.FromXmlType()
            return new ForeignCitizenData();
        }
    }
}
