using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public class DequeueDataChangeEventsFacadeMethod:FacadeMethodInfo<Schemas.Part.Events.DataChangeEventInfo[]>
    {
        public int MaxCount = 0;

        public DequeueDataChangeEventsFacadeMethod(int maxCount, string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.MaxCount = maxCount;
        }

        public override bool IsValidInput(ref CprBroker.Schemas.Part.Events.DataChangeEventInfo[] invaliInputReturnValue)
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
