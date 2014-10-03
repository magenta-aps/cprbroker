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
    public class CprConverterExtensionTests_GoesIntoDatabase : DbrTestBase
    {
        [Test]
        public void PersonTotal_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                dataContext.PersonTotals.InsertOnSubmit(null);
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Person_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                dataContext.Persons.InsertOnSubmit(PersonBaseTest.Persons[pnr].ToPerson());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Child_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var ch in pers.Child)
                    dataContext.Childs.InsertOnSubmit(ch.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentName_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                dataContext.PersonNames.InsertOnSubmit(pers.CurrentNameInformation.ToDpr(pers.PersonInformation));
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void HistoricalName_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var n in pers.HistoricalName)
                    dataContext.PersonNames.InsertOnSubmit(n.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentCivilStatus_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                dataContext.CivilStatus.InsertOnSubmit(pers.CurrentCivilStatus.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void HistoricalCivilStatus_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var c in pers.HistoricalCivilStatus)
                    dataContext.CivilStatus.InsertOnSubmit(c.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentSeparation_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            if (pers.CurrentSeparation != null)
                using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
                {
                    dataContext.Separations.InsertOnSubmit(pers.CurrentSeparation.ToDpr());
                    dataContext.SubmitChanges();
                }
        }

        [Test]
        public void HistoricalSeparation_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var s in pers.HistoricalSeparation)
                    dataContext.Separations.InsertOnSubmit(s.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentCitizenship_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                dataContext.Nationalities.InsertOnSubmit(pers.CurrentCitizenship.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void HistoricalCitizenship_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var c in pers.HistoricalCitizenship)
                    dataContext.Nationalities.InsertOnSubmit(c.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentDeparture_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            if (pers.CurrentDepartureData != null)
                using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
                {
                    dataContext.Departures.InsertOnSubmit(pers.CurrentDepartureData.ToDpr(pers.ElectionInformation.First()));
                    dataContext.SubmitChanges();
                }
        }

        [Test]
        public void HistoricalDeparture_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var d in pers.HistoricalDeparture)
                    dataContext.Departures.InsertOnSubmit(d.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void ContactAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            if (pers.ContactAddress != null)
                using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
                {
                    dataContext.ContactAddresses.InsertOnSubmit(pers.ContactAddress.ToDpr());
                    dataContext.SubmitChanges();
                }
        }

        [Test]
        public void FolkeregisterAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            var adr = pers.GetFolkeregisterAdresseSource(false) as CprBroker.Providers.CPRDirect.CurrentAddressWrapper;
            if (adr != null)
                using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
                {
                    dataContext.PersonAddresses.InsertOnSubmit(adr.ToDpr(null));
                    dataContext.SubmitChanges();
                }
        }

        [Test]
        public void HistoricalAddress_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var a in pers.HistoricalAddress)
                    dataContext.PersonAddresses.InsertOnSubmit(a.ToDpr(null));
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Protection_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var p in pers.Protection)
                    dataContext.Protections.InsertOnSubmit(p.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void CurrentDisappearance_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            if (pers.CurrentDisappearanceInformation != null)
                using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
                {
                    dataContext.Disappearances.InsertOnSubmit(pers.CurrentDisappearanceInformation.ToDpr());
                    dataContext.SubmitChanges();
                }
        }

        [Test]
        public void HistoricalDisappearance_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var d in pers.HistoricalDisappearance)
                    dataContext.Disappearances.InsertOnSubmit(d.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Events_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var e in pers.Events)
                    dataContext.Events.InsertOnSubmit(e.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Notes_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var n in pers.Notes)
                    dataContext.Notes.InsertOnSubmit(n.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void MunicipalConditions_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var m in pers.MunicipalConditions)
                    dataContext.MunicipalConditions.InsertOnSubmit(m.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void ParentalAuthority_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                foreach (var p in pers.ParentalAuthority)
                    dataContext.ParentalAuthorities.InsertOnSubmit(p.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Disempowerment_ToDpr_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                if (pers.Disempowerment != null)
                    dataContext.GuardianAndParentalAuthorityRelations.InsertOnSubmit(pers.Disempowerment.ToDpr());
                dataContext.SubmitChanges();
            }
        }

        [Test]
        public void Disempowerment_ToDprAddress_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = PersonBaseTest.Persons[pnr];
            using (var dataContext = new DPRDataContext(DbrDatabase.ConnectionString))
            {
                if (pers.Disempowerment != null)
                    dataContext.GuardianAddresses.InsertOnSubmit(pers.Disempowerment.ToDprAddress());
                dataContext.SubmitChanges();
            }
        }
    }
}
