using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class HealthInformation
    {
        public SundhedOplysningType ToXmlType()
        {
            return new SundhedOplysningType()
            {
                PraktiserendeLaegeNavn = PhysicianName,
                PraktiserendeLaegeYderNummerIdentifikator = PhysicianProviderNumber,
                SygesikringsgruppeKode = HealthInsuranceGroupCode,
                Virkning = Effect.ToXmlType(),
            };
        }

        public static HealthInformation FromXmlType(SundhedOplysningType oio)
        {
            return new HealthInformation()
            {
                PhysicianName = oio.PraktiserendeLaegeNavn,
                PhysicianProviderNumber = oio.PraktiserendeLaegeYderNummerIdentifikator,
                HealthInsuranceGroupCode = oio.SygesikringsgruppeKode,
                Effect = Effect.FromXmlType(oio.Virkning)
            };
        }
    }
}
