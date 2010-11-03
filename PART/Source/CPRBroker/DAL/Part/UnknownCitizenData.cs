using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
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
    }
}
