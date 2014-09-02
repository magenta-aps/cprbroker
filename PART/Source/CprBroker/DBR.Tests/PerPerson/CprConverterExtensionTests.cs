using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;
using CprBroker.DBR;
using CprBroker.DBR.Extensions;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DBR.PerPerson
{
    [TestFixture]
    public class CprConverterExtensionTests : PersonBaseTest
    {
        [Test]
        public void PersonTotal_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            pers.ToPersonTotal(pers.CurrentAddressInformation, pers.CurrentDepartureData);
        }

        [Test]
        public void Person_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            Persons[pnr].ToPerson();
        }

        [Test]
        public void Child_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var ch in pers.Child)
                ch.ToDpr();
        }

        [Test]
        public void CurrentName_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            pers.CurrentNameInformation.ToDpr(pers.PersonInformation);
        }

        [Test]
        public void HistoricalName_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var n in pers.HistoricalName)
                n.ToDpr();
        }

        [Test]
        public void CurrentCivilStatus_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            pers.CurrentCivilStatus.ToDpr();
        }

        [Test]
        public void HistoricalCivilStatus_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var c in pers.HistoricalCivilStatus)
                c.ToDpr();
        }

        [Test]
        public void CurrentSeparation_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.CurrentSeparation != null)
                pers.CurrentSeparation.ToDpr();
        }

        [Test]
        public void HistoricalSeparation_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var s in pers.HistoricalSeparation)
                s.ToDpr();
        }

        [Test]
        public void CurrentCitizenship_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            pers.CurrentCitizenship.ToDpr();
        }

        [Test]
        public void HistoricalCitizenship_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var c in pers.HistoricalCitizenship)
                c.ToDpr();
        }

        [Test]
        public void CurrentDeparture_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.CurrentDepartureData != null)
                pers.CurrentDepartureData.ToDpr(pers.ElectionInformation.First());
        }

        [Test]
        public void HistoricalDeparture_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var d in pers.HistoricalDeparture)
                d.ToDpr();
        }

        [Test]
        public void ContactAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.ContactAddress != null)
                pers.ContactAddress.ToDpr();
        }

        [Test]
        public void FolkeregisterAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            var adr = pers.GetFolkeregisterAdresseSource(false) as CprBroker.Providers.CPRDirect.CurrentAddressWrapper;
            if (adr != null)
                adr.ToDpr();
        }

        [Test]
        public void HistoricalAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var a in pers.HistoricalAddress)
                a.ToDpr();
        }

        [Test]
        public void Protection_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var p in pers.Protection)
                p.ToDpr();
        }

        [Test]
        public void CirrentDisappearance_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.CurrentDisappearanceInformation != null)
                pers.CurrentDisappearanceInformation.ToDpr();
        }

        [Test]
        public void HistoricalDisappearance_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var d in pers.HistoricalDisappearance)
                d.ToDpr();
        }

        [Test]
        public void Events_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var e in pers.Events)
                e.ToDpr();
        }

        [Test]
        public void Notes_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var n in pers.Notes)
                n.ToDpr();
        }

        [Test]
        public void MunicipalConditions_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var m in pers.MunicipalConditions)
                m.ToDpr();
        }

        [Test]
        public void ParentalAuthority_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            foreach (var p in pers.ParentalAuthority)
                p.ToDpr();
        }

        [Test]
        public void Disempowerment_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.Disempowerment != null)
                pers.Disempowerment.ToDpr();
        }

        [Test]
        public void Disempowerment_ToDprAddress_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            if (pers.Disempowerment != null)
                pers.Disempowerment.ToDprAddress();
        }
    }
}
