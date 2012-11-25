using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Part;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.UpdateRules
{
    public class SpecielVejkodeIndikatorMatchRule : MatchRule<DanskAdresseType>
    {
        public override DanskAdresseType GetObject(RegistreringType1 oio)
        {
            if (
                oio != null
                && oio.AttributListe != null
                && oio.AttributListe.RegisterOplysning != null
                && oio.AttributListe.RegisterOplysning.Length > 0
                && oio.AttributListe.RegisterOplysning[0] != null
                && oio.AttributListe.RegisterOplysning[0].Item is CprBorgerType
                && (oio.AttributListe.RegisterOplysning[0].Item as CprBorgerType).FolkeregisterAdresse != null
                )
                return (oio.AttributListe.RegisterOplysning[0].Item as CprBorgerType).FolkeregisterAdresse.Item as DanskAdresseType;
            return null;
        }

        public override bool AreCandidates(DanskAdresseType existingObj, DanskAdresseType newObj)
        {
            return existingObj.SpecielVejkodeIndikatorSpecified == false && newObj.SpecielVejkodeIndikatorSpecified == true;
        }

        public override void UpdateOioFromXmlType(DanskAdresseType existingObj, DanskAdresseType newObj)
        {
            existingObj.SpecielVejkodeIndikator = newObj.SpecielVejkodeIndikator;
            existingObj.SpecielVejkodeIndikatorSpecified = newObj.SpecielVejkodeIndikatorSpecified;
        }

        public override void UpdateDbFromXmlType(PersonRegistration dbReg, DanskAdresseType newObj)
        {
            dbReg.PersonAttributes.CprData.Address.DenmarkAddress.SpecialRoadCode = newObj.SpecielVejkodeIndikator;
        }
    }
}
