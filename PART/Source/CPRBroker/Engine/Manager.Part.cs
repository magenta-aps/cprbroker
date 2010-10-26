using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;

namespace CPRBroker.Engine
{
    /// <summary>
    /// This section implements the PART interface methods as of the PART standard
    /// </summary>
    public static partial class Manager
    {
        public static class Part
        {
            public static Guid[] Search(string userToken, string appToken, PersonSearchCriteria searchCriteria, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPartSearchDataProvider, Guid[]>
                (
                    userToken,
                    appToken,
                    true,
                    true,
                    (prov) => prov.Search(searchCriteria, out ql),
                    true,
                    null //(personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(personRegistration)
                );

                qualityLevel = ql;                
                return ret;
            }

            public static PersonRegistration Read(string userToken, string appToken, Guid uuid, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPartReadDataProvider, PersonRegistration>
                (
                    userToken,
                    appToken,
                    true,
                    true,
                    (prov) => prov.Read(uuid, out ql),
                    true,
                    null //(personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(personRegistration)
                );

                qualityLevel = ql;
                return ret;
            }
        }
    }
}
