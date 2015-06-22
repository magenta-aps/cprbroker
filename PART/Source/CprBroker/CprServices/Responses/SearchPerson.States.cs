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

        [Obsolete("Use the response from Familie service")]
        public CivilStatusType ToCivilStatusType()
        {
            return null;
        }

        public LivStatusType ToLivStatusType()
        {
            var status = Utilities.Strings.FirstNonEmpty(
                GetFieldValue(_Node, "STATUS"),
                GetFieldValue(_Node, "CNVN_STATUS")
            );

            int intStatus;
            if (!string.IsNullOrEmpty(status) && int.TryParse(status, out intStatus))
            {
                return new LivStatusType()
                {
                    // Passing true even if birthdate is unknow to avoid Prenatal return
                    LivStatusKode = Schemas.Util.Enums.ToLifeStatus(intStatus, true),
                    TilstandVirkning = TilstandVirkningType.Create(
                        Utilities.Dates.ToDateTimeOrNull(
                            Utilities.Strings.FirstNonEmpty(
                                GetFieldValue(_Node, "STARTDATOSTATUS"),
                                GetFieldValue(_Node, "STARTDATO")
                            ),
                            "yyyyMMddHHmm",
                            "yyyyMMdd"
                        )
                    )
                };
            }
            return null;
        }
    }
}
