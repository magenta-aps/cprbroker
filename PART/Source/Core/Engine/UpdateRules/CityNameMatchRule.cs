using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Engine.UpdateRules
{
    public class CityNameMatchRule : MatchRule<DanskAdresseType>
    {
        public override DanskAdresseType GetObject(RegistreringType1 oio)
        {
            if (
                oio.AttributListe != null
                && oio.AttributListe.RegisterOplysning != null
                && oio.AttributListe.RegisterOplysning.Length > 0
                && oio.AttributListe.RegisterOplysning[0] != null
                )
            {
                var cprBorger = oio.AttributListe.RegisterOplysning[0].Item as CprBorgerType;
                if (cprBorger != null && cprBorger.FolkeregisterAdresse != null)
                    return cprBorger.FolkeregisterAdresse.Item as DanskAdresseType;
            }
            return null;
        }

        public override bool AreCandidates(DanskAdresseType existingObj, DanskAdresseType newObj)
        {
            return

                string.Equals(existingObj.PostDistriktTekst, newObj.PostDistriktTekst)

                && existingObj.AddressComplete != null && existingObj.AddressComplete.AddressPostal != null
                && newObj.AddressComplete != null && newObj.AddressComplete.AddressPostal != null

                && string.Equals(existingObj.PostDistriktTekst, existingObj.AddressComplete.AddressPostal.DistrictName)
                && !string.Equals(existingObj.AddressComplete.AddressPostal.DistrictName, newObj.AddressComplete.AddressPostal.DistrictName);
        }

        public override void UpdateOioFromXmlType(DanskAdresseType existingObj, DanskAdresseType newObj)
        {
            existingObj.AddressComplete.AddressPostal.DistrictName = newObj.AddressComplete.AddressPostal.DistrictName;            
        }

        public override void UpdateDbFromXmlType(PersonRegistration dbReg, DanskAdresseType newObj)
        {
            dbReg.PersonAttributes.CprData.Address.DenmarkAddress.DistrictName = newObj.AddressComplete.AddressPostal.DistrictName;
        }
    }
}
