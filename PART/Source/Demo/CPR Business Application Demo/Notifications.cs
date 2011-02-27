using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Business;

namespace CPR_Business_Application_Demo
{
    public partial class NotificationsForm : Form
    {
        public NotificationsForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// This is very basic. Just show the cpr and name of the person.
        /// </summary>
        /// <param name="notifications"></param>
        public void AddNotification(List<NotificationPerson> notifications)
        {
            foreach (var notificationPerson in notifications)
            {
                NotificationsText.Text += notificationPerson.Cpr + ": "
                                       + notificationPerson.FirstName + " "
                                       + notificationPerson.LastName
                                       + "\r\n";
            }
            
        }

        private void ClearNotificationsButton_Click(object sender, EventArgs e)
        {
            NotificationsText.Clear();
        }
    }
}
