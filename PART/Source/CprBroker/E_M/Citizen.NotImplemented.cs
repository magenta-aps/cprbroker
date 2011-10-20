using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        private static LokalUdvidelseType ToLokalUdvidelseType(Citizen citizen)
        {
            // No extension
            return null;
        }

        private static KontaktKanalType ToNextOfKin(Citizen citizen)
        {
            //TODO: Shoud NextOfKin be implemented?
            return null;
        }

        private static KontaktKanalType ToKontaktKanalType(Citizen citizen)
        {
            // No contact channels
            return null;
        }

        private static AdresseType ToAndreAdresse(Citizen citizen)
        {
            //TODO: Shoud other address be filled?
            return null;
        }

        private static SundhedOplysningType ToSundhedOplysningType(Citizen citizen)
        {
            return null;
        }
    }
}