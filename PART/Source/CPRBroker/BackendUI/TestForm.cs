using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CprBroker.Engine;
using CprBroker.DAL;

namespace CprBroker.BackendUI
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void notifyButton_Click(object sender, EventArgs e)
        {
            /*
            string notifyButtonText = notifyButton.Text;
            notifyButton.Enabled = false;
            resultTextBox.Text = "Calling...";
            // Create a new worker thread that sends the notifications
            WaitCallback callBack = (target) =>
                {
                    BrokerContext.Initialize(DAL.Applications.Application.BaseApplicationToken.ToString(), CprBroker.Engine.Constants.UserToken, true, false, true);

                    // Refresh persons
                    Engine.NotificationEngine.RefreshPersonsData();
                    var result = Engine.NotificationEngine.SendNotifications(notifyDateTimePicker.Value.Date);
                    string xml = Engine.Util.Strings.SerializeObject(result);
                    Action notificationAction = () =>
                    {
                        notifyButton.Enabled = true;
                        notifyButton.Text = notifyButtonText;
                        resultTextBox.Text = xml;
                    };
                    this.Invoke(notificationAction);
                };
            ThreadPool.QueueUserWorkItem(callBack);*/
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            this.BirthdateEventEnqueuer.Start();
            this.DataChangeEventEnqueuer.Start();
            this.NotificationSender.Start();
        }
    }
}
