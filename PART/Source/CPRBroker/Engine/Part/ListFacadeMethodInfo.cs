using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ListFacadeMethodInfo : FacadeMethodInfo<ListOutputType1>
    {
        public ListFacadeMethodInfo()
        {
            this.InitializationMethod = new Action(InitializationMethod);
        }

        void Initialize()
        { 

        }
    }
}
