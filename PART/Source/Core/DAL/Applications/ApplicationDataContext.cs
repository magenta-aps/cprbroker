namespace CprBroker.Data.Applications
{
    /// <summary>
    /// Represents the data context for applications
    /// </summary>
    partial class ApplicationDataContext
    {
        public ApplicationDataContext()
            : this(Config.Properties.Settings.Default.CprBrokerConnectionString)
        { }

    }
}
