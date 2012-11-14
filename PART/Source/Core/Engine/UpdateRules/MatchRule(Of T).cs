using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Engine.UpdateRules
{
    public abstract class MatchRule<TOio> : MatchRule
    {
        public override sealed bool UpdateOioFromXmlIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg)
        {
            var existingObj = GetObject(existingReg);
            var newObj = GetObject(newReg);

            if (existingObj != null && newObj != null)
            {
                if (AreCandidates(existingObj, newObj))
                {
                    UpdateOioFromXmlType(existingObj, newObj);
                    return true;
                }
            }
            return false;
        }

        public override sealed void UpdateDbFromXmlType(PersonRegistration dbReg, RegistreringType1 newObj)
        {
            var obj = GetObject(newObj);
            UpdateDbFromXmlType(dbReg, obj);
        }

        public abstract TOio GetObject(RegistreringType1 oio);
        public abstract bool AreCandidates(TOio existingObj, TOio newObj);
        public abstract void UpdateOioFromXmlType(TOio existingObj, TOio newObj);
        public abstract void UpdateDbFromXmlType(PersonRegistration dbReg, TOio newObj);
    }
}
