using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public partial class ServiceNameForm : BaseForm
    {
        public string ServiceName="Event broker service";
        public string CprEventsServiceUrl = "http://CprBroker/Services/Events.asmx";

        public ServiceNameForm()
        {
            InitializeComponent();
            serviceNameTextBox.Text = ServiceName;
            cprBrokerEventsServiceUrlCustomTextBox.Text = CprEventsServiceUrl;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CloseAsOK();
        }

        protected override bool ValidateContents()
        {
            if (!base.ValidateContents())
                return false;

            ServiceName = serviceNameTextBox.Text;
            CprEventsServiceUrl = cprBrokerEventsServiceUrlCustomTextBox.Text;

            var services = System.ServiceProcess.ServiceController.GetServices();
            if(services.Where(sc=>sc.ServiceName.ToLower() == ServiceName.ToLower()).Count() >0)
            {
                MessageBox.Show(this,"Service name exists");
                return false;
            }

            string url = CprEventsServiceUrl + "?wsdl";
            try
            {
                System.Net.WebClient cl = new System.Net.WebClient();
                cl.DownloadData(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to local Cpr Broker. Please validate the URL");
                return false;
            }


            return true;
        }
    }
}
