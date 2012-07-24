using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public TilstandListeType ToTilstandListeType()
        {
            return new TilstandListeType()
            {
                CivilStatus = ToCivilStatusType(),
                LivStatus = ToLivStatusType(),
                LokalUdvidelse = ToLokalUdvidelseType()
            };
        }

        public LivStatusType ToLivStatusType()
        {
            return this.PersonInformation.ToLivStatusType();
        }

        public CivilStatusType ToCivilStatusType()
        {
            return new CivilStatusWrapper(this.CurrentCivilStatus).ToCivilStatusType(this.CurrentSeparation);
        }

    }
}
