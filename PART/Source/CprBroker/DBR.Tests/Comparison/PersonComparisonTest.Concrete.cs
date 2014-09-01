using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    [TestFixture]
    public class PersonTotalComparisonTest : PersonComparisonTest<PersonTotal> {
        override public string[] ExcludedProperties
        {
            get
            {
            string[] excluded = {
                                    "DirectoryProtectionMarker", // We do not test the street name as it is not present in historical records.
                                    "SpouseMarker", // We do not know the origin of this marker.
                                    "PaternityAuthorityName", // We can only get this one from CPR Services.
                                    "ArrivalDateMarker", // We do not know the origin of this marker.

                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                };
            return excluded;
            }
        }
    }

    [TestFixture]
    public class PersonComparisonTest : PersonComparisonTest<CprBroker.Providers.DPR.Person> { }

    [TestFixture]
    public class ChildComparisonTests : PersonComparisonTest<Child> { }

    [TestFixture]
    public class PersonNameComparisonTests : PersonComparisonTest<PersonName> {
        /*
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                        "NameAuthorityCode", // This one can only be fetched by CPR Services
                                        "AddressingNameDate", // This one can only be fetched by CPR Services

                                     BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS 
                                };
                return excluded;
            }
        }
         */
        public override IQueryable<PersonName> Get(DPRDataContext dataContext, string pnr)
        {
            Console.WriteLine(dataContext.Connection.ConnectionString);
            return dataContext.PersonNames.Where(c => c.PNR == decimal.Parse(pnr)).OrderByDescending(c => c.NameStartDate).ToArray().AsQueryable();
        }
    }

    [TestFixture]
    public class CivilStatusComparisonTests : PersonComparisonTest<CivilStatus> { }

    [TestFixture]
    public class SeparationComparisonTests : PersonComparisonTest<Separation> { }

    [TestFixture]
    public class NationalityComparisonTests : PersonComparisonTest<Nationality> { }

    [TestFixture]
    public class DepartureComparisonTests : PersonComparisonTest<Departure> { }

    [TestFixture]
    public class ContactAddressComparisonTests : PersonComparisonTest<ContactAddress> { }

    [TestFixture]
    public class PersonAddressComparisonTests : PersonComparisonTest<PersonAddress> {
        /*
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    "AddressStartDateMarker", // We do not know the origin of this marker.

                                     BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS 
                                    "CprUpdateDate", // It is skipped for now, as the contents are wrong
                                };
                return excluded;
            }
        }
         */

        public override IQueryable<PersonAddress> Get(DPRDataContext dataContext, string pnr)
        {
            return dataContext.PersonAddresses.Where(c => c.PNR == decimal.Parse(pnr)).OrderByDescending(c => c.AddressStartDate).ThenBy(c => c.MunicipalityCode);
        }
    }

    [TestFixture]
    public class ProtectionComparisonTests : PersonComparisonTest<Protection> { }

    [TestFixture]
    public class DisappearanceComparisonTests : PersonComparisonTest<Disappearance> { }

    [TestFixture]
    public class EventComparisonTests : PersonComparisonTest<Event> { }

    [TestFixture]
    public class NoteComparisonTests : PersonComparisonTest<Note> { }

    [TestFixture]
    public class MunicipalConditionComparisonTests : PersonComparisonTest<MunicipalCondition> { }

    [TestFixture]
    public class ParentalAuthorityConditionComparisonTests : PersonComparisonTest<ParentalAuthority> { }

    [TestFixture]
    public class GuardianAndParentalAuthorityRelationComparisonTests : PersonComparisonTest<GuardianAndParentalAuthorityRelation> { }

    [TestFixture]
    public class GuardianAddressComparisonTests : PersonComparisonTest<GuardianAddress> { }

}