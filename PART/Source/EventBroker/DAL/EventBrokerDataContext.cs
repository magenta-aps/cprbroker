namespace CprBroker.EventBroker.DAL
{
    partial class EventBrokerDataContext
    {
        public EventBrokerDataContext():this(CprBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {
 
        }
    }
}
