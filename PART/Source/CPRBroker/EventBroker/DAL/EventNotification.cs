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
                    EventIdentifier = Strings.GuidToUri(this.EventNotificationId).ToString(),
                    EventObjectStructure = new EventObjectStructureType()
                    {
                        actionSchemeReference = null,//new Uri(""),
                        EventObjectActionCode = "",
                        EventObjectReference = null,//new Uri(""),
                        ObjectTypeReference = null,//new Uri(""),
                    },
                    EventProducerReference = null,//new Uri(""),
                    EventRegistrationDateTime = this.CreatedDate,
                },
                EventSubscriptionReference = Strings.GuidToUri(this.SubscriptionId).ToString(),
                EventTopic = null,//new Uri(""),
                ExtensionStructure = null,
                //Signature = null
            };
            return ret;
        }

        public NotificationService.CommonEventStructureType ToWsdl()
        {
            //TODO: Implement this method
            var ret = new NotificationService.CommonEventStructureType()
            {
                EventDetailStructure = null,
                EventInfoStructure = new NotificationService.EventInfoStructureType()
                {
                    EventIdentifier = Strings.GuidToUri(this.EventNotificationId).ToString(),
                    EventObjectStructure = new NotificationService.EventObjectStructureType()
                    {
                        actionSchemeReference = null,//new Uri(""),
                        EventObjectActionCode = "",
                        EventObjectReference = null,//new Uri(""),
                        ObjectTypeReference = null,//new Uri(""),
                    },
                    EventProducerReference = null,//new Uri(""),
                    EventRegistrationDateTime = this.CreatedDate,
                },
                EventSubscriptionReference = Strings.GuidToUri(this.SubscriptionId).ToString(),
                EventTopic = null,//new Uri(""),
                ExtensionStructure = null,
                //Signature = null
            };
            return ret;
        }
    }
}
