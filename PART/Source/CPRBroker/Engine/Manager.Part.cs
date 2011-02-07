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
                ReadFacadeMethodInfo facadeMethod = new ReadFacadeMethodInfo(input, localAction, appToken, userToken, true);
                var ret = GetMethodOutput<LaesOutputType>(facadeMethod);
                qualityLevel = facadeMethod.QualityLevel;
                return ret;
            }

            public static ListOutputType1 List(string userToken, string appToken, ListInputType input, out QualityLevel? qualityLevel)
            {
                ListOutputType1 ret = null;

                ret = GetMethodOutput<ListOutputType1>(
                    new ListFacadeMethodInfo(input, appToken, userToken, true)
                    );

                //TODO: remove quality level because it applies to individual elements rather than the whole result
                qualityLevel = QualityLevel.LocalCache;
                return ret;
            }

            public static SoegOutputType Search(string userToken, string appToken, SoegInputType1 searchCriteria, out QualityLevel? qualityLevel)
            {
                SearchFacadeMethodInfo facadeMethod = new SearchFacadeMethodInfo(searchCriteria, appToken, userToken);
                var ret = GetMethodOutput<SoegOutputType>(facadeMethod);
                //TODO: Move into Search method of data provider
                qualityLevel = QualityLevel.LocalCache;
                return ret;
            }

            public static Guid GetPersonUuid(string userToken, string appToken, string cprNumber)
            {
                var facadeMethod = new GerPersonUuidFacadeMethodInfo(cprNumber, appToken, userToken, true);
                var ret = GetMethodOutput<Guid>(facadeMethod);
                return ret;
            }

        }
    }
}
