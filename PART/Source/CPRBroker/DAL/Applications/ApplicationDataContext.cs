namespace CPRBroker.DAL.Applications
{
    partial class ApplicationDataContext
    {
        public ApplicationDataContext()
            : this(CPRBroker.Config.Properties.Settings.Default.CPRConnectionString)
        { }

    }
}
