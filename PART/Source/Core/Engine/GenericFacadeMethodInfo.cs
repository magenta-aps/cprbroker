using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    /// <summary>
    /// Generic facade method
    /// Allows werapping of non IBasicOutputType into a BasicOutputType
    /// </summary>
    /// <typeparam name="TItem">Type of inner item</typeparam>
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
