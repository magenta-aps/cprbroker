using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.UpdateRules
{
    public abstract class MatchRule
    {

        public static bool Overwrites(RegistreringType1 existingReg, RegistreringType1 newReg)
        {
            IEnumerable<MatchRule> matchRules = null;
            foreach (var rule in matchRules)
            {
                rule.NeutralizeIfPossible(existingReg, newReg);
            }
            var existingXml = Strings.SerializeObject(existingReg);
            var newXml = Strings.SerializeObject(newReg);
            return string.Equals(existingXml, newXml);
        }

        public abstract bool NeutralizeIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg);

    }

    public abstract class MatchRule<T> : MatchRule
    {
        public override bool NeutralizeIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg)
        {
            var existingObj = GetObject(existingReg);
            var newObj = GetObject(newReg);

            if (existingObj != null && newObj != null)
            {
                if (AreCandidates(existingObj, newObj))
                {
                    Neutralize(existingObj, newObj);
                    return true;
                }
            }
            return false;
        }

        public abstract T GetObject(RegistreringType1 oio);

        public abstract bool AreCandidates(T existingObj, T newObj);
        public abstract void Neutralize(T existingObj, T newObj);
    }
}
