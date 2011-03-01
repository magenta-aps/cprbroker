namespace CprBroker.Data.Applications
{
    partial class ApplicationDataContext
    {
        public ApplicationDataContext()
            : this(Config.Properties.Settings.Default.CprBrokerConnectionString)
        { }

    }
}
