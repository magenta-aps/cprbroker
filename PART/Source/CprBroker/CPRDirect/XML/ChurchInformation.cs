using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ChurchInformationType
    {
        public bool ToFolkekirkeMedlemIndikator()
        {
            return Converters.ToFolkekirkeMedlemIndikator(this.ChurchRelationship);
        }

        public DateTime? ToChurchRelationshipDate()
        {
            return Converters.ToDateTime(this.StartDate, this.StartDateUncertainty);
        }
    }
}
