namespace CprBroker.EventBroker.Data
{
    /// <summary>
    /// Represents the data context for events
    /// </summary>
    partial class EventBrokerDataContext
    {
        public EventBrokerDataContext():this(CprBroker.Config.Properties.Settings.Default.EventBrokerConnectionString)
        {
 
        }
    }
}
