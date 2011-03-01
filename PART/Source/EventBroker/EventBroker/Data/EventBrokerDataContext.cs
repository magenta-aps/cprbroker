namespace CprBroker.EventBroker.Data
{
    partial class EventBrokerDataContext
    {
        public EventBrokerDataContext():this(CprBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {
 
        }
    }
}
