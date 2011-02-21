using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class SearchFacadeMethodInfo : FacadeMethodInfo<SoegOutputType, string[]>
    {
        private SearchFacadeMethodInfo()
        { }

        public SearchFacadeMethodInfo(SoegInputType1 input, string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.SubMethodInfos = new SubMethodInfo[] { new SearchSubMethodInfo(input) };
        }

        public override string[] Aggregate(object[] results)
        {
            var foundIds = results[0] as Guid[];
            return (from id in foundIds select id.ToString()).ToArray();
        }
    }
}
