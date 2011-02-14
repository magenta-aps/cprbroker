namespace CprBroker.Installers.EventBrokerInstallers
{
    partial class EventBrokerBackendServiceInstaller
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
            this.backendServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.backendServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // backendServiceProcessInstaller
            // 
            this.backendServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.backendServiceProcessInstaller.Password = null;
            this.backendServiceProcessInstaller.Username = null;
            // 
            // backendServiceInstaller
            // 
            this.backendServiceInstaller.ServiceName = "CPR broker backend service";
            this.backendServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.backendServiceProcessInstaller,
            this.backendServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller backendServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller backendServiceInstaller;
    }
}