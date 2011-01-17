using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part.Events;
using CprBroker.Engine.Util;

namespace CprBroker.EventBroker.DAL
{
    public partial class EventNotification
    {
        public CprBroker.Schemas.Part.Events.CommonEventStructureType ToOioNotification()
        {
            //TODO: Implement this method
            var ret = new Schemas.Part.Events.CommonEventStructureType()
            {
                EventDetailStructure = null,
                EventInfoStructure = new EventInfoStructureType()
                {
                    EventIdentifier = Strings.GuidToUri(this.EventNotificationId),
                    EventObjectStructure = new EventObjectStructureType()
                    {
                        actionSchemeReference = new Uri(""),
                        EventObjectActionCode = "",
                        EventObjectReference = new Uri(""),
                        ObjectTypeReference = new Uri(""),
                    },
                    EventProducerReference = new Uri(""),
                    EventRegistrationDateTime = this.CreatedDate,
                },
                EventSubscriptionReference = Strings.GuidToUri(this.SubscriptionId),
                EventTopic = new Uri(""),
                ExtensionStructure = null,
                Signature = null
            };
            return ret;
        }
    }
}
