using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Events;

namespace CprBroker.Engine.Events
{
    public class DequeueDataChangeEventsFacadeMethod : FacadeMethodInfo<BasicOutputType<DataChangeEventInfo[]>>
    {
        public int MaxCount = 0;

        public DequeueDataChangeEventsFacadeMethod(int maxCount, string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.MaxCount = maxCount;
        }

        public override bool IsValidInput(ref BasicOutputType<DataChangeEventInfo[]> invalidInputReturnValue)
        {
            return MaxCount > 0;
        }

        public override void Initialize()
        {
            base.Initialize();
            SubMethodInfos = new SubMethodInfo[] { new Events.DequeueDataChangeEventSubMethodInfo(MaxCount) };
        }
    }
}
