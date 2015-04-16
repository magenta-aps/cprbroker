using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices.Responses
{
    public partial class SearchPerson
    {
        public TilstandListeType ToTilstandListeType()
        {

            return new TilstandListeType()
            {
                CivilStatus = ToCivilStatusType(),
                LivStatus = ToLivStatusType()
            };
        }

        public CivilStatusType ToCivilStatusType()
        {
            // TODO: Fill CivilStatusType from GCTP
            return null;
        }

        public LivStatusType ToLivStatusType()
        {
            var status = GetFieldValue(_Node, "STATUS");
            var  statusDate = GetFieldValue(_Node,"STARTDATOSTATUS");
            int intStatus;

            if(!string.IsNullOrEmpty(status)&& int.TryParse(status,out intStatus))
            {
                
                return new LivStatusType()
                {
                    // Passing true even if birthdate is unknow to avoid Prenatal return
                    LivStatusKode = Schemas.Util.Enums.ToLifeStatus(intStatus, true),
                    TilstandVirkning = TilstandVirkningType.Create(
                        Utilities.Dates.ToDateTimeOrNull(
                            GetFieldValue(_Node,"STARTDATOSTATUS"),
                            "yyyyMMdd"
                        )
                    )
                };
            }
            return null;
        }
    }
}
