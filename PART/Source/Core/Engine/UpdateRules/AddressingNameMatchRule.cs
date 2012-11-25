using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Part;
using CprBroker.Schemas.Part;
using CprBroker.Schemas;

namespace CprBroker.Engine.UpdateRules
{
    public class AddressingNameMatchRule : MatchRule<NavnStrukturType>
    {
        public override NavnStrukturType GetObject(RegistreringType1 oio)
        {
            if (oio != null && oio.AttributListe != null && oio.AttributListe.Egenskab != null && oio.AttributListe.Egenskab.Length > 0 && oio.AttributListe.Egenskab[0] != null)
                return oio.AttributListe.Egenskab[0].NavnStruktur;
            return null;
        }

        public override bool AreCandidates(NavnStrukturType existingObj, NavnStrukturType newObj)
        {
            return string.IsNullOrEmpty(existingObj.PersonNameForAddressingName) && !string.IsNullOrEmpty(newObj.PersonNameForAddressingName);
        }

        public override void UpdateOioFromXmlType(NavnStrukturType existingObj, NavnStrukturType newObj)
        {
            existingObj.PersonNameForAddressingName = newObj.PersonNameForAddressingName;
        }

        public override void UpdateDbFromXmlType(PersonRegistration dbReg, NavnStrukturType newObj)
        {
            dbReg.PersonAttributes.PersonProperties.AddressingName = newObj.PersonNameForAddressingName;
        }
    }
}
