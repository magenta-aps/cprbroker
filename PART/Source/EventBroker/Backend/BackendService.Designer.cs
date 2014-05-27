namespace CprBroker.EventBroker.Backend
{
    partial class BackendService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DataChangeEventEnqueuer = new CprBroker.EventBroker.Notifications.DataChangeEventEnqueuer(this.components);
            this.BirthdateEventEnqueuer = new CprBroker.EventBroker.Notifications.BirthdateEventEnqueuer(this.components);
            this.NotificationSender = new CprBroker.EventBroker.Notifications.NotificationSender(this.components);
            this.CprDirectExtractor = new CprBroker.EventBroker.Notifications.CPRDirectExtractor();
            this.CprDirectPersonConverter = new CprBroker.EventBroker.Notifications.CPRDirectPersonConverter();
            this.CprDirectDownloader = new CprBroker.EventBroker.Notifications.CPRDirectDownloader();
            this.BudgetChecker = new CprBroker.EventBroker.Notifications.BudgetChecker();
            this.CriteriaSubscriptionPersonPopulator = new CprBroker.EventBroker.Notifications.CriteriaSubscriptionPersonPopulator();
            this.DataChangeEventPuller = new CprBroker.EventBroker.Notifications.DataChangeEventPuller();
            // 
            // BackendService
            // 
            this.ServiceName = "CPR broker backend service";

        }

        #endregion

        private CprBroker.EventBroker.Notifications.DataChangeEventEnqueuer DataChangeEventEnqueuer;
        private CprBroker.EventBroker.Notifications.BirthdateEventEnqueuer BirthdateEventEnqueuer;
        private CprBroker.EventBroker.Notifications.NotificationSender NotificationSender;
        private Notifications.CPRDirectExtractor CprDirectExtractor;
        private Notifications.CPRDirectPersonConverter CprDirectPersonConverter;
        private Notifications.CPRDirectDownloader CprDirectDownloader;
        private Notifications.BudgetChecker BudgetChecker;
        private Notifications.CriteriaSubscriptionPersonPopulator CriteriaSubscriptionPersonPopulator;
        private Notifications.DataChangeEventPuller DataChangeEventPuller;
    }
}
