namespace CprBroker.EventBroker.DAL
{
    partial class EventBrokerDataContext
    {
        public EventBrokerDataContext():this(CPRBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {
 
        }
    }
}
