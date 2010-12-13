using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Business;
using CPR_Business_Application_Demo.Adapters.CPRPersonWS;

namespace CPR_Business_Application_Demo
{
    public partial class MainForm : Form, INotificationListener
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void FillPersonResults(PersonBasicStructureType person)
        {
            // General information.
            personFirstNameTextBox.Text = person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName;
            personMiddleNameTextBox.Text = person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName;
            personSurnameTextBox.Text = person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName;

            var address = person.Item as DanishAddressStructureType;
            if (address != null)
            {
                AddressLine1TextBox.Text = address.CompletePostalLabel.PostalAddressFirstLineText;
                AddressLine2TextBox.Text = address.CompletePostalLabel.PostalAddressSecondLineText;
                AddressLine3TextBox.Text = address.CompletePostalLabel.PostalAddressThirdLineText;
                AddressLine4TextBox.Text = address.CompletePostalLabel.PostalAddressFourthLineText;
                AddressLine5TextBox.Text = address.CompletePostalLabel.PostalAddressFifthLineText;
                AddressLine6TextBox.Text = address.CompletePostalLabel.PostalAddressSixthLineText;
            }
        }

        private static void FillPersonNameAndAddressResults(PersonNameAndAddressStructureType nameAndAddress)
        {
            //AddressIdentifierCodeType
            //nameAndAddress.
        }


        private void ConsoleWriteLine(string message)
        {
            notificationResultsConsoleTextBox.Text += message + "\n";
        }

        private bool CheckRegistration(bool showMessageBox)
        {
            try
            {
                var adminController = new CPRAdministrationController(Properties.Settings.Default);
                var applicationRegistration =
                    adminController.ApplicationIsRegistered(Properties.Settings.Default.CPRBrokerWebServiceUrl);
                if (applicationRegistration != null)
                {
                    mainTabControl.Enabled = true;
                    registeredStatusLabel.Text = "Application is registered";
                    if (showMessageBox)
                    {
                        MessageBox.Show(this,
                                        "The application is registered",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    StartWatcher();
                    return true;
                }
                else
                {
                    mainTabControl.Enabled = false;
                    registeredStatusLabel.Text = "Application is NOT registered!";
                    if (showMessageBox)
                    {
                        MessageBox.Show(this,
                                        "The application is not registered or connection failed",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void OpenOptionsForm()
        {
            var optionsForm = new OptionsForm();
            optionsForm.ShowDialog();
            CheckRegistration(false);
        }

        #region Subscription watching
        private void StartWatcher()
        {
            if (Directory.Exists(Properties.Settings.Default.NotificationFileShare))
            {
                if (notificationWatcher == null)
                {
                    notificationWatcher = new FileSystemWatcher(Properties.Settings.Default.NotificationFileShare);
                    notificationWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                    notificationWatcher.Filter = "*.*";

                    // Add event handlers.
                    notificationWatcher.Changed += new FileSystemEventHandler(OnChanged);
                    notificationWatcher.Created += new FileSystemEventHandler(OnChanged);
                    notificationWatcher.Deleted += new FileSystemEventHandler(OnChanged);

                    // Begin watching.
                    notificationWatcher.EnableRaisingEvents = true;
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var t = e.ChangeType;
            if (t == WatcherChangeTypes.Created || t == WatcherChangeTypes.Changed)
            {
                var notificationController = new NotificationController();
                notificationController.HandleNotification(Properties.Settings.Default, e.FullPath, this);
            }
        }

        private FileSystemWatcher notificationWatcher = null;

        #endregion

        
        #region INotificationListener Members

        private delegate void ReportChangedPersonRegistrationIdsDelegate(List<NotificationPerson> changedPersonRegistrationIds);

        public void ReportChangedPersonRegistrationIds(List<NotificationPerson> changedPersonRegistrationIds)
        {
            Invoke(new ReportChangedPersonRegistrationIdsDelegate(ReportChangedPersonRegistrationIdsGuiThread),
                   changedPersonRegistrationIds);
        }

        private void ReportChangedPersonRegistrationIdsGuiThread(List<NotificationPerson> changedPersonRegistrationIds)
        {
            if (notificationsForm == null)
            {
                notificationsForm = new NotificationsForm();
                notificationsForm.FormClosing += ((sender, e) => notificationsForm = null);
            }

            notificationsForm.Show(this);
            notificationsForm.AddNotification(changedPersonRegistrationIds);
        }

        #endregion

        #region Private Members

        private NotificationsForm notificationsForm = null;
        #endregion

        #region Events
        private void CreateTestPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var createForm = new CreateTestCitizenForm();
            var dialogResult = createForm.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                var person = createForm.GetPerson();
                var adminController = new CPRAdministrationController(Properties.Settings.Default);
                var result = adminController.CreateTestCitizen(person);
            }
        }

        private void checkRegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckRegistration(true);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!CheckRegistration(false))
                OpenOptionsForm();

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOptionsForm();
        }

        private void personQueryButton_Click(object sender, EventArgs e)
        {
            var personController = new CPRPersonController(Properties.Settings.Default);

            var personalCivilIdentificationNumber = personalCivilIdentificationNumberTextBox.Text;
            PersonBasicStructureType person = personController.GetCitizenBasic(personalCivilIdentificationNumber);
            if (person != null)
            {
                FillPersonResults(person);
            }

            PersonNameAndAddressStructureType personNameAndAddress =
                personController.GetCitizenNameAndAddress(personalCivilIdentificationNumber);
            if (personNameAndAddress != null)
            {
                FillPersonNameAndAddressResults(personNameAndAddress);
            }

            QualityHeader qualityHeader;
            var custodyChildren = personController.GetCitizenChildren(personalCivilIdentificationNumber, false, out qualityHeader);
            if (custodyChildren != null)
            {
                RelationsTextBox.Text += "Non-Custody children:\r\n";
                foreach (var child in custodyChildren)
                {
                    RelationsTextBox.Text += "  "
                                          + child.PersonCivilRegistrationIdentifier
                                          + ": "
                                          + child.PersonNameStructure.PersonGivenName
                                          + " "
                                          + child.PersonNameStructure.PersonSurnameName
                                          + "\r\n";
                }
            }
            else
            {
                RelationsTextBox.Text += "No non-custody children\r\n";
            }

            custodyChildren = personController.GetCitizenChildren(personalCivilIdentificationNumber, true, out qualityHeader);
            if (custodyChildren != null)
            {
                RelationsTextBox.Text += "Custody children:\r\n";
                foreach (var child in custodyChildren)
                {
                    RelationsTextBox.Text += "  "
                                          + child.PersonCivilRegistrationIdentifier
                                          + ": "
                                          + child.PersonNameStructure.PersonGivenName
                                          + " "
                                          + child.PersonNameStructure.PersonSurnameName
                                          + "\r\n";
                }
            }
            else
            {
                RelationsTextBox.Text += "No custody children\r\n";
            }

            var relations = personController.GetCitizenRelations(personalCivilIdentificationNumber, out qualityHeader);
            if (relations != null)
            {
                RelationsTextBox.Text += "Spouses:\r\n";
                foreach (var spouse in relations.Spouses)
                {
                    RelationsTextBox.Text += "  "
                                             + spouse.SimpleCPRPerson.PersonCivilRegistrationIdentifier
                                             + ": "
                                             + spouse.SimpleCPRPerson.PersonNameStructure.PersonGivenName
                                             + " "
                                             + spouse.SimpleCPRPerson.PersonNameStructure.PersonSurnameName
                                             + "\r\n";
                }
            }
            else
            {
                RelationsTextBox.Text += "No spouses\r\n";
            }
        }

        private void subscribeCprButton_Click(object sender, EventArgs e)
        {
            ConsoleWriteLine("Subscribing.....");
            var t = Properties.Settings.Default.NotificationMode;
            var adminController = new CPRAdministrationController(Properties.Settings.Default);

            string result = adminController.Subscribe(notificationPersonsTextBox.Lines);
            if (result.Length > 0)
            {
                ConsoleWriteLine("Suceeded!");
                ConsoleWriteLine("Subscription ID: " + result);
            }
            else
            {
                ConsoleWriteLine("FAILED!!!");
            }
        }

        private void GetServiceInfoButton_Click(object sender, EventArgs e)
        {
            var adminController = new CPRAdministrationController(Properties.Settings.Default);
            var capabilities = adminController.GetCapabillities();
            InfoText.Text = "";
            foreach (var capability in capabilities)
            {
                InfoText.Text += "Version: " + capability.Version + "\r\n"
                              + "Details: " + capability.Details + "\r\n"
                              + "Functions: " + String.Join("\r\n", capability.Functions.ToArray()) + "\r\n";
            }
        }

        private void BirthDaySubscriptionButton_Click(object sender, EventArgs e)
        {
            var adminController = new CPRAdministrationController(Properties.Settings.Default);
            int? age = (int)AgeSpin.Value;
            if (IgnoreAgeCheckBox.Checked)
                age = null;
            var result = adminController.SubscribeBirthdate(age, (int)PriorDaysSpin.Value,
                                                             notificationPersonsTextBox.Lines);
            if (result.Length > 0)
            {
                ConsoleWriteLine("Suceeded!");
                ConsoleWriteLine("Subscription ID: " + result);
            }
            else
            {
                ConsoleWriteLine("FAILED!!!");
            }
        }


        private void ListActiveSubscriptionsButton_Click(object sender, EventArgs e)
        {
            var adminController = new CPRAdministrationController(Properties.Settings.Default);
            var activeSubscriptions = adminController.GetActiveSubscriptions();
            Dictionary<string, Dictionary<string, bool>> subscriptionIds = new Dictionary<string, Dictionary<string, bool>>();

            foreach (var subscription in activeSubscriptions)
            {
                Dictionary<string, bool> personRegistrationIds = new Dictionary<string, bool>();
                foreach (var personRegistrationId in subscription.PersonCivilRegistrationIdentifiers)
                {
                    if (!personRegistrationIds.ContainsKey(personRegistrationId))
                        personRegistrationIds.Add(personRegistrationId, true);
                }
                subscriptionIds.Add(subscription.SubscriptionId, personRegistrationIds);
            }

            notificationResultsConsoleTextBox.Text = "";
            foreach (var personRegistrationSet in subscriptionIds)
            {
                notificationResultsConsoleTextBox.Text += "ID: " + personRegistrationSet.Key + "\r\n";
                foreach (var id in personRegistrationSet.Value.Keys)
                {
                    notificationResultsConsoleTextBox.Text += id;
                }
            }
        }
        #endregion

        private void readButton_Click(object sender, EventArgs e)
        {            
            var partController = new PartController(Properties.Settings.Default);
            Guid personUuid = new Guid(uuidTextBox.Text);
            var personReg = partController.Read(personUuid);
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof (PartService.PersonRegistration));
            StringWriter w = new StringWriter();
            ser.Serialize(w,personReg);
            resultXmlTextBox.Text = w.ToString();
        }
    }
}
