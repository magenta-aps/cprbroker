using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine.Part;

namespace CprBroker.Engine
{
    /// <summary>
    /// This section implements the PART interface methods as of the PART standard
    /// </summary>
    public static partial class Manager
    {
        public static class Part
        {
            public static LaesOutputType Read(string userToken, string appToken, LaesInputType input, out QualityLevel? qualityLevel)
            {
                return Read(userToken, appToken, input, out qualityLevel, LocalDataProviderUsageOption.UseFirst);
            }

            public static LaesOutputType RefreshRead(string userToken, string appToken, LaesInputType input, out QualityLevel? qualityLevel)
            {
                return Read(userToken, appToken, input, out qualityLevel, LocalDataProviderUsageOption.Forbidden);
            }

            private static LaesOutputType Read(string userToken, string appToken, LaesInputType input, out QualityLevel? qualityLevel, LocalDataProviderUsageOption localAction)
            {
                QualityLevel? ql = null;
                PersonIdentifier pId = null;

                FacadeMethodInfo<LaesOutputType> facadeMethodInfo = new FacadeMethodInfo<LaesOutputType>(appToken, userToken, true)
                {
                    InitializationMethod = () =>
                    {
                        pId = DAL.Part.PersonMapping.GetPersonIdentifier(new Guid(input.UUID));
                        if (pId == null)
                        {
                            throw new Exception(TextMessages.UuidNotFound);
                        }
                    },

                    SubMethodInfos = new SubMethodInfo[] 
                    {
                        new SubMethodInfo<IPartReadDataProvider,RegistreringType1>()
                        {
                            LocalDataProviderOption= localAction,
                            FailIfNoDataProvider = true,
                            FailOnDefaultOutput=true,
                            Method = (prov)=>prov.Read(pId,input, (cpr)=>Manager.Part.GetPersonUuid(userToken, appToken, cpr), out ql),
                            UpdateMethod = (personRegistration)=> Local.UpdateDatabase.UpdatePersonRegistration(new Guid(input.UUID), personRegistration)
                        }
                    },

                    AggregationMethod = (subresults) =>
                    {
                        LaesOutputType o = new LaesOutputType()
                        {
                            LaesResultat = new LaesResultatType()
                            {
                                Item = subresults[0]
                            },
                            //TODO: Fill this StandardRetur object
                            StandardRetur = new StandardReturType()
                            {
                                FejlbeskedTekst = "",
                                StatuskodeKode = ""
                            }
                        };
                        return o;
                    }
                };

                var ret = GetMethodOutput<LaesOutputType>(facadeMethodInfo);
                qualityLevel = ql;
                return ret;
            }

            public static ListOutputType1 List(string userToken, string appToken, ListInputType input, out QualityLevel? qualityLevel)
            {
                //TODO: remove quality level because it applies to individual elements rather than the whole result
                QualityLevel? ql = null;
                ListOutputType1 ret = null;

                ListFacadeMethodInfo facadeMethodInfo = new ListFacadeMethodInfo(input, appToken, userToken, true);

                //TODO: Could fail if Input.UUID is null
                facadeMethodInfo.SubMethodInfos = Array.ConvertAll<string, SubMethodInfo>
                (
                    input.UUID.ToArray(),
                    (pUUID) => new ReadSubMethodInfo(
                        facadeMethodInfo.inputUuidToPersonIdentifierMap[pUUID], 
                        LaesInputType.Create(pUUID, input), 
                        (cpr) => Manager.Part.GetPersonUuid(userToken, appToken, cpr), 
                        LocalDataProviderUsageOption.UseFirst)
               );

                ret = GetMethodOutput<ListOutputType1>(facadeMethodInfo);

                qualityLevel = ql;
                return ret;
            }

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
