using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class GenericFacadeMethodInfo<TItem> : FacadeMethodInfo<BasicOutputType<TItem>, TItem>
    {
        public GenericFacadeMethodInfo()
        { }

        public GenericFacadeMethodInfo(string appToken, string userToken)
            : base(appToken, userToken)
        {

        }
    }
}
