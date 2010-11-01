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
            public static PersonRegistration Read(string userToken, string appToken, Guid uuid, DateTime? effectDate, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPartReadDataProvider, PersonRegistration>
                (
                    userToken,
                    appToken,
                    true,
                    true,
                    (prov) =>
                    {
                        var pId = DAL.Part.PersonMapping.GetPersonIdentifier(uuid);
                        if (pId == null)
                        {
                            throw new Exception(TextMessages.UuidNotFound);
                        }
                        return prov.Read(pId, effectDate, out ql);
                    },
                    true,
                    // TODO: add the update method here
                    null //(personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(personRegistration)
                );

                qualityLevel = ql;
                return ret;
            }

            // TODO: Add List method here after Read method is finalized


            public static PersonIdentifier[] Search(string userToken, string appToken, PersonSearchCriteria searchCriteria, DateTime? effectDate, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPartSearchDataProvider, PersonIdentifier[]>
                (
                    userToken,
                    appToken,
                    true,
                    true,
                    (prov) => prov.Search(searchCriteria, effectDate, out ql),
                    true,
                    null //(personRegistration) => Local.UpdateDatabase.UpdatePersonRegistration(personRegistration)
                );

                qualityLevel = ql;
                return ret;
            }


        }
    }
}
