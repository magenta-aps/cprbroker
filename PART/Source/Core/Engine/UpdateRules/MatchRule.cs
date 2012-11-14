using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Engine.UpdateRules
{
    public abstract class MatchRule
    {

        private readonly static MatchRule[] _AllRules = new MatchRule[] { new CityNameMatchRule() };

        public abstract bool UpdateFromXmlIfPossible(PersonRegistration dbReg, RegistreringType1 existingReg, RegistreringType1 newReg);


        public static MatchRule[] AllRules()
        {
            return new List<MatchRule>(_AllRules).ToArray();
        }

        public static bool ApplyRules(PersonRegistration dbReg, RegistreringType1 newReg)
        {
            return ApplyRules(dbReg, newReg, AllRules());
        }

        public static bool ApplyRules(PersonRegistration dbReg, RegistreringType1 newReg, IEnumerable<MatchRule> matchRules)
        {
            var existingReg = PersonRegistration.ToXmlType(dbReg);

            bool updated = false;
            foreach (var rule in matchRules)
            {
                updated = updated ||
                    rule.UpdateFromXmlIfPossible(dbReg, existingReg, newReg);
            }
            if (updated)
            {
                dbReg.SetContents(newReg);
            }
            return updated;
        }
    }

    public abstract class MatchRule<TOio> : MatchRule
    {
        public override sealed bool UpdateFromXmlIfPossible(PersonRegistration dbReg, RegistreringType1 existingReg, RegistreringType1 newReg)
        {
            var existingObj = GetObject(existingReg);
            var newObj = GetObject(newReg);

            if (existingObj != null && newObj != null)
            {
                if (AreCandidates(existingObj, newObj))
                {
                    UpdateFromXmlType(dbReg, existingObj, newObj);
                    return true;
                }
            }
            return false;
        }

        public abstract TOio GetObject(RegistreringType1 oio);
        public abstract bool AreCandidates(TOio existingObj, TOio newObj);
        public abstract void UpdateFromXmlType(Data.Part.PersonRegistration dbReg, TOio existingObj, TOio newObj);
    }
}
