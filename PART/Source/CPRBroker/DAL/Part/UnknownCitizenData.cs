using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Part
{
    public partial class UnknownCitizenData
    {
        public Schemas.Part.UnknownCitizenData ToXmlType()
        {
            return new Schemas.Part.UnknownCitizenData()
            {
                ReplacementCprNumber = this.ReplacementCprNumber
            };
        }

        public static UnknownCitizenData FromXmlType(Schemas.Part.UnknownCitizenData partUnknownData)
        {
            // TODO: Implement UnknownCitizenData.FromXmlType()
            return new UnknownCitizenData();
        }
    }
}
