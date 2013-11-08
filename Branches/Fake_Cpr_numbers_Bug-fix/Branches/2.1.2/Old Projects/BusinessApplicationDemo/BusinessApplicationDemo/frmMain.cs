using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace BusinessApplicationDemo
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Build menu items
            AddMenuItems(typeof(Enums.PersonMethod), personContextMenuStrip, new EventHandler(personItem_Click));
            AddMenuItems(typeof(Enums.Admin.ApplicationMethod), applicationContextMenuStrip, new EventHandler(applicationItem_Click));
            AddMenuItems(typeof(Enums.Admin.SubscriptionMethod), subscriptionContextMenuStrip, new EventHandler(subscriptionItem_Click));
            AddMenuItems(typeof(Enums.Admin.OtherMethod), otherContextMenuStrip, new EventHandler(otherItem_Click));
        }

        private CPRAdministrationWS.CPRAdministrationWS CreateAdminWebService()
        {
            CPRAdministrationWS.CPRAdministrationWS admin = new CPRAdministrationWS.CPRAdministrationWS();
            admin.Url = txtAdminServiceUrl.Text;
            admin.ApplicationHeaderValue = new CPRAdministrationWS.ApplicationHeader()
            {
                ApplicationToken = txtAppToken.Text,
                UserToken = txtUserToken.Text
            };
            return admin;
        }

        private CPRPersonWS.CPRPersonWS CreatePersonWebService()
        {
            CPRPersonWS.CPRPersonWS person = new CPRPersonWS.CPRPersonWS();
            person.Url = txtPersonServiceUrl.Text;
            person.ApplicationHeaderValue = new CPRPersonWS.ApplicationHeader()
            {
                ApplicationToken = txtAppToken.Text,
                UserToken = txtUserToken.Text
            };
            return person;
        }

        /// <summary>
        /// Creates a context menu and adds the handler for each item
        /// </summary>
        /// <param name="enumType">Type of enum that contains the menu actions</param>
        /// <param name="contextMenu">Context menu</param>
        /// <param name="personItem_Click">Event handler</param>
        void AddMenuItems(Type enumType, ContextMenuStrip contextMenu, EventHandler personItem_Click)
        {
            Array.ForEach(Enum.GetNames(enumType), new Action<string>(delegate(string enumItem)
            {
                ToolStripItem personItem = contextMenu.Items.Add(enumItem);
                object enumValue = Enum.Parse(enumType, enumItem);
                // Only enable items that are <100, as other items require complex input
                personItem.Enabled = ((int)enumValue) < 100;
                personItem.Tag = Convert.ChangeType(enumValue, enumType);

                personItem.Click += personItem_Click;
            }));
        }

        void personItem_Click(object sender, EventArgs e)
        {
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            ToolStripItem item = (ToolStripItem)sender;
            object result = null;
            switch ((Enums.PersonMethod)item.Tag)
            {
                case Enums.PersonMethod.GetCitizenBasic:
                    result = person.GetCitizenBasic(txtCPRNumber.Text);
                    break;
                case Enums.PersonMethod.GetCitizenChildren:
                    result = person.GetCitizenChildren(txtCPRNumber.Text, false);
                    break;
                case Enums.PersonMethod.GetCitizenFull:
                    result = person.GetCitizenFull(txtCPRNumber.Text);
                    break;
                case Enums.PersonMethod.GetCitizenNameAndAddress:
                    result = person.GetCitizenNameAndAddress(txtCPRNumber.Text);
                    break;
                case Enums.PersonMethod.GetCitizenRelations:
                    result = person.GetCitizenRelations(txtCPRNumber.Text);
                    break;
                case Enums.PersonMethod.GetParentAuthorityOverChildChanges:
                    result = person.GetParentAuthorityOverChildChanges(txtCPRNumber.Text);
                    break;
                case Enums.PersonMethod.RemoveParentAuthorityOverChild:
                    result = person.RemoveParentAuthorityOverChild(txtCPRNumber.Text, txtChildCprNumber.Text);
                    break;
                case Enums.PersonMethod.SetParentAuthorityOverChild:
                    result = person.SetParentAuthorityOverChild(txtCPRNumber.Text, txtChildCprNumber.Text);
                    break;
            }
            DisplayResult(result);
        }

        void applicationItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            object result = null;
            switch ((Enums.Admin.ApplicationMethod)item.Tag)
            {
                case Enums.Admin.ApplicationMethod.ApproveAppRegisteration:
                    MessageBox.Show(Enums.Admin.ApplicationMethod.ApproveAppRegisteration.ToString());
                    break;
                case Enums.Admin.ApplicationMethod.ListAppRegisteration:
                    result = admin.ListAppRegistrations();
                    break;
                case Enums.Admin.ApplicationMethod.RequestAppRegisteration:
                    MessageBox.Show(Enums.Admin.ApplicationMethod.RequestAppRegisteration.ToString());
                    break;
                case Enums.Admin.ApplicationMethod.UnregisterApp:
                    MessageBox.Show(Enums.Admin.ApplicationMethod.UnregisterApp.ToString());
                    break;
            }
            DisplayResult(result);
        }

        void subscriptionItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            object result = null;
            switch ((Enums.Admin.SubscriptionMethod)item.Tag)
            {
                case Enums.Admin.SubscriptionMethod.Subscribe:
                    MessageBox.Show(Enums.Admin.SubscriptionMethod.Subscribe.ToString());
                    break;
                case Enums.Admin.SubscriptionMethod.Unsubscribe:
                    MessageBox.Show(Enums.Admin.SubscriptionMethod.Unsubscribe.ToString());
                    break;
                case Enums.Admin.SubscriptionMethod.SubscribeOnBirthdate:
                    MessageBox.Show(Enums.Admin.SubscriptionMethod.SubscribeOnBirthdate.ToString());
                    break;
                case Enums.Admin.SubscriptionMethod.RemoveBirthDateSubscriptions:
                    MessageBox.Show(Enums.Admin.SubscriptionMethod.RemoveBirthDateSubscriptions.ToString());
                    break;
                case Enums.Admin.SubscriptionMethod.GetActiveSubsciptionsList:
                    result = admin.GetActiveSubscriptionsList();
                    break;
            }
            DisplayResult(result);
        }

        void otherItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            object result = null;
            switch ((Enums.Admin.OtherMethod)item.Tag)
            {
                case Enums.Admin.OtherMethod.CreateTestCitizen:
                    MessageBox.Show(Enums.Admin.OtherMethod.CreateTestCitizen.ToString());
                    break;
                case Enums.Admin.OtherMethod.GetCapabilities:
                    result = admin.GetCapabilities();
                    break;
                case Enums.Admin.OtherMethod.GetCPRDataProviderList:
                    result = admin.GetDataProviderList();
                    break;
                case Enums.Admin.OtherMethod.IsImplementing:
                    MessageBox.Show(Enums.Admin.OtherMethod.IsImplementing.ToString());
                    break;
                case Enums.Admin.OtherMethod.LogFunctions:
                    MessageBox.Show(Enums.Admin.OtherMethod.LogFunctions.ToString());
                    break;
                case Enums.Admin.OtherMethod.SetCPRDataProviderList:
                    MessageBox.Show(Enums.Admin.OtherMethod.SetCPRDataProviderList.ToString());
                    break;
            }
        }

        private void btnCitizenData_Click(object sender, EventArgs e)
        {
            Point position = new Point(btnCitizenData.Width / 2, btnCitizenData.Height / 2);
            personContextMenuStrip.Show(btnCitizenData, position);
        }

        private void btnApplication_Click(object sender, EventArgs e)
        {
            Point position = new Point(btnApplication.Width / 2, btnApplication.Height / 2);
            applicationContextMenuStrip.Show(btnApplication, position);
        }

        private void btnSubscription_Click(object sender, EventArgs e)
        {
            Point position = new Point(btnSubscription.Width / 2, btnSubscription.Height / 2);
            subscriptionContextMenuStrip.Show(btnSubscription, position);
        }

        private void btnOther_Click(object sender, EventArgs e)
        {
            Point position = new Point(btnOther.Width / 2, btnOther.Height / 2);
            otherContextMenuStrip.Show(btnOther, position);
        }

        void DisplayResult(object result)
        {
            if (result == null)
            {
                MessageBox.Show("No data found");
            }
            else
            {
                // Display as XML
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
                System.IO.StringWriter w = new System.IO.StringWriter();
                serializer.Serialize(w, result);
                txtResultXml.Text = w.ToString();

                // Display as dataset
                try
                {
                    dataGridView1.DataSource =null;
                    DataSet ds = new DataSet();
                    ds.ReadXml(new System.IO.StringReader(txtResultXml.Text));
                    dataGridView1.DataSource = ds;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Data cannot be displayed as a grid");
                }
            }
        }
    }
}
