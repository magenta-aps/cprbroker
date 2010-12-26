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
                return Read(userToken, appToken, uuid, effectDate, out qualityLevel, LocalDataProviderUsageOption.UseFirst);
            }

            public static PersonRegistration RefreshRead(string userToken, string appToken, Guid uuid, DateTime? effectDate, out QualityLevel? qualityLevel)
            {
                return Read(userToken, appToken, uuid, effectDate, out qualityLevel, LocalDataProviderUsageOption.Forbidden);
            }

            private static PersonRegistration Read(string userToken, string appToken, Guid uuid, DateTime? effectDate, out QualityLevel? qualityLevel, LocalDataProviderUsageOption localAction)
            {
                QualityLevel? ql = null;
                PersonIdentifier pId = null;

                FacadeMethodInfo<PersonRegistration> facadeMethodInfo = new FacadeMethodInfo<PersonRegistration>(appToken, userToken, true)
                {
                    InitializationMethod = () =>
                    {
                        pId = DAL.Part.PersonMapping.GetPersonIdentifier(uuid);
                        if (pId == null)
                        {
                            throw new Exception(TextMessages.UuidNotFound);
                        }
                    },

                    SubMethodInfos = new SubMethodInfo[] 
                    {
                        new SubMethodInfo<IPartReadDataProvider,PersonRegistration>()
                        {
                            LocalDataProviderOption= localAction,
                            FailIfNoDataProvider = true,
                            FailOnDefaultOutput=true,
                            Method = (prov)=>prov.Read(pId,effectDate,out ql),
                            UpdateMethod = (personRegistration)=> Local.UpdateDatabase.UpdatePersonRegistration(uuid, personRegistration)
                        }
                    },

                    AggregationMethod = (subresults) => subresults[0] as PersonRegistration
                };

                var ret = GetMethodOutput<PersonRegistration>(facadeMethodInfo);
                qualityLevel = ql;
                return ret;
            }

            // TODO: Add List method here after Read method is finalized

            public static Guid[] Search(string userToken, string appToken, PersonSearchCriteria searchCriteria, DateTime? effectDate, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                FacadeMethodInfo<Guid[]> facadeMethod = new FacadeMethodInfo<Guid[]>(appToken, userToken, true)
                {
                    SubMethodInfos = new SubMethodInfo[]
                    {
                        new SubMethodInfo<IPartSearchDataProvider,Guid[]>()
                        {
                            LocalDataProviderOption = LocalDataProviderUsageOption.UseLast,
                            FailIfNoDataProvider = true,
                            FailOnDefaultOutput = true,
                            Method = (prov)=>prov.Search(searchCriteria,effectDate,out ql),
                            UpdateMethod = null
                        }
                    }
                };

                var ret = GetMethodOutput<Guid[]>(facadeMethod);
                qualityLevel = ql;
                return ret;
            }


            public static Guid GetPersonUuid(string userToken, string appToken, string cprNumber)
            {
                var facadeMethod = new FacadeMethodInfo<Guid>()
                    {
                        UserToken = userToken,
                        ApplicationToken = appToken,
                        ApplicationTokenRequired = true,

                        SubMethodInfos = new SubMethodInfo[]
                        {
                            new SubMethodInfo<IPartPersonMappingDataProvider,Guid>()
                            {
                                Method = (prov)=>prov.GetPersonUuid(cprNumber),
                                LocalDataProviderOption = LocalDataProviderUsageOption.UseLast
                            }
                        },

                        AggregationMethod = (results) => (Guid)results[0],
                    };

                var ret = GetMethodOutput<Guid>(facadeMethod);
                return ret;
            }

        }
    }
}
