using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Adapters.CPRAdministrationWS;

namespace CPR_Business_Application_Demo
{
    public partial class CreateTestCitizenForm : Form
    {
        public CreateTestCitizenForm()
        {
            InitializeComponent();
        }

        public PersonFullStructureType GetPerson()
        {
            var name = new PersonNameStructureType();
            name.PersonGivenName = FirstNameTextBox.Text;
            name.PersonMiddleName = MiddleNameTextBox.Text;
            name.PersonSurnameName = LastNameTextBox.Text;

            var person = new PersonFullStructureType();
            person.RegularCPRPerson = new RegularCPRPersonType();
            person.RegularCPRPerson.SimpleCPRPerson = new SimpleCPRPersonType();
            person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure = name;

            person.RegularCPRPerson.PersonBirthDateStructure = new PersonBirthDateStructureType();
            person.RegularCPRPerson.PersonBirthDateStructure.BirthDate = BirthDateCalendar.Value;
            person.MaritalStatusCode = (MaritalStatusCodeType) MaritalStatusComboBox.SelectedIndex;
            person.RegularCPRPerson.PersonGenderCode = (PersonGenderCodeType) GenderCombo.SelectedIndex;
            person.PersonInformationProtectionIndicator = InformationProtectionCheckBox.Checked;
            person.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier =
                BirthDateCalendar.Value.ToString("ddMMyy" + CprTextBox.Text);

            var address = new DanishAddressStructureType();
            address.CompletePostalLabel = new CompletePostalLabelType();
            address.CompletePostalLabel.PostalAddressFirstLineText = AddressLine1TextBox.Text;
            address.CompletePostalLabel.PostalAddressSecondLineText = AddressLine2TextBox.Text;
            address.CompletePostalLabel.PostalAddressThirdLineText = AddressLine3TextBox.Text;
            address.CompletePostalLabel.PostalAddressFourthLineText = AddressLine4TextBox.Text;
            address.CompletePostalLabel.PostalAddressFifthLineText = AddressLine5TextBox.Text;
            address.CompletePostalLabel.PostalAddressSixthLineText = AddressLine6TextBox.Text;
            person.Item = address;

            return person;
        }
    }
}
