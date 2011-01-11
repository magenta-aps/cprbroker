using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class SearchFacadeMethodInfo : FacadeMethodInfo<SoegOutputType>
    {
        private SearchFacadeMethodInfo()
        { }

        public SearchFacadeMethodInfo(SoegInputType1 input, string appToken, string userToken)
            : base(appToken, userToken, true)
        {
            this.SubMethodInfos = new SubMethodInfo[] { new SearchSubMethodInfo(input) };
        }

        public override SoegOutputType Aggregate(object[] results)
        {
            var ret = new SoegOutputType();
            var foundIds = results[0] as Guid[];
            if (foundIds != null)
            {
                ret.IdListe = (from id in foundIds select id.ToString()).ToArray();
            }
            return ret;
        }
    }
}
