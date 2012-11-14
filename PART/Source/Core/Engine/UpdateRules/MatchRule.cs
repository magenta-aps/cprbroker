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

        /// <summary>
        /// Checks if current rule Updates <paramref name="existingReg"/> with the corresponding values from <paramref name="newReg"/>
        /// </summary>
        /// <param name="existingReg">PART registration object to be changed, usually obtained from local database and subject to the bug fix</param>
        /// <param name="newReg">Updated PART registration obtained from the data source, with the fix implemented</param>
        /// <returns>True </returns>
        public abstract bool UpdateXmlTypeIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg);

        /// <summary>
        /// Updates the database object with data from the PART object according to the rule specification
        /// </summary>
        /// <param name="dbReg"></param>
        /// <param name="newObj"></param>
        public abstract void UpdateDbFromXmlType(PersonRegistration dbReg, RegistreringType1 newObj);


        private readonly static MatchRule[] _AllRules = new MatchRule[] { new CityNameMatchRule() };
        
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
                if (rule.UpdateXmlTypeIfPossible(existingReg, newReg))
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
}
