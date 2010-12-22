namespace CPRBroker.DAL.Events
{
    partial class EventBrokerDataContext
    {
        public EventBrokerDataContext():this(CPRBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {
 
        }
    }
}
