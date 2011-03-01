using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    public partial class HealthInformation
    {
        public static SundhedOplysningType[] ToXmlType(HealthInformation db)
        {
            if (db != null)
            {
                return new SundhedOplysningType[]
                {
                    new SundhedOplysningType()
                    {
                        PraktiserendeLaegeNavn = db.PhysicianName,
                        PraktiserendeLaegeYderNummerIdentifikator = db.PhysicianProviderNumber,
                        SygesikringsgruppeKode = db.HealthInsuranceGroupCode,
                        Virkning = Effect.ToVirkningType(db.Effect),
                    }
                };
            }
            return null;
        }

        public static HealthInformation FromXmlType(SundhedOplysningType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return new HealthInformation()
                {
                    PhysicianName = oio[0].PraktiserendeLaegeNavn,
                    PhysicianProviderNumber = oio[0].PraktiserendeLaegeYderNummerIdentifikator,
                    HealthInsuranceGroupCode = oio[0].SygesikringsgruppeKode,
                    Effect = Effect.FromVirkningType(oio[0].Virkning)
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<HealthInformation>(hi => hi.Effect);
        }
    }
}
