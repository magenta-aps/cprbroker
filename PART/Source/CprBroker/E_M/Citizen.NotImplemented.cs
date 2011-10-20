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
        private LokalUdvidelseType ToLokalUdvidelseType()
        {
            // No extension
            return null;
        }

        private KontaktKanalType ToNextOfKin()
        {
            //TODO: Shoud NextOfKin be implemented?
            return null;
        }

        private KontaktKanalType ToKontaktKanalType()
        {
            // No contact channels
            return null;
        }

        private AdresseType ToAndreAdresse()
        {
            //TODO: Shoud other address be filled?
            return null;
        }

        private SundhedOplysningType ToSundhedOplysningType()
        {
            return null;
        }
    }
}