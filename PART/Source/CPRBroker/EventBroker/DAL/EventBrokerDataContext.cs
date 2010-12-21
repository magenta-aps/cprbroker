namespace CprBroker.EventBroker.DAL
{
    partial class EventBrokerDataContextDataContext
    {
        public EventBrokerDataContextDataContext()
            : this(CPRBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {

        }
    }
}
