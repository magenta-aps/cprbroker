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

        public abstract bool UpdateOioFromXmlIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg);
        public abstract void UpdateDbFromXmlType(PersonRegistration dbReg, RegistreringType1 newObj);


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
            var appliedRules = new List<MatchRule>();

            // Attempt rule application to OIO objects
            foreach (var rule in matchRules)
            {
                if (rule.UpdateOioFromXmlIfPossible(existingReg, newReg))
                {
                    appliedRules.Add(rule);
                }
            }

            if (appliedRules.Count > 0)
            {
                var existingXml = Strings.SerializeObject(existingReg);
                var newXml = Strings.SerializeObject(newReg);

                if (string.Equals(existingXml, newXml))
                {
                    foreach (var appliedRule in appliedRules)
                    {
                        appliedRule.UpdateDbFromXmlType(dbReg, newReg);
                    }
                    dbReg.SetContents(newReg);
                    return true;
                }
            }
            return false;
        }
    }

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
