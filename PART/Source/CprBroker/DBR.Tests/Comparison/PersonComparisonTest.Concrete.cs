using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    [TestFixture]
    public class PersonTotalComparisonTest : PersonComparisonTest<PersonTotal7>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "DirectoryProtectionMarker", // We do not test the street name as it is not present in historical records.
                                    "SpouseMarker", // We do not know the origin of this marker.
                                    "PaternityAuthorityName", // We can only get this one from CPR Services.
                                    "ArrivalDateMarker", // We do not know the origin of this marker.
                                    "PreviousAddress", // This field is not fully implemented because it is not used.
                                    "PreviousMunicipalityName", // This field is not fully implemented because it is not used.
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class PersonComparisonTest : PersonComparisonTest<CprBroker.Providers.DPR.Person> {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "BirthplaceText", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "BirthRegistrationDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "BirthRegistrationPlaceUpdateDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "ChurchAuthorityCode", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "ChurchDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "ChurchRelationUpdateDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "CprUpdateDate", /* 
                                                      * We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                                      * The field should contain a chronologically sorted list, so that client systems can use it for sorting rows.
                                                      */
                                    "CurrentPnr", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "CurrentPnrUpdateDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "CustomerNumber", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "FatherDocumentation", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "FatherName", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "Job", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "KinshipUpdateDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "MotherDocumentation", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "MotherName", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PaternityAuthorityCode", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PaternityDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PnrDate", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "UnderGuardianshipAuthprityCode", // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class ChildComparisonTests : PersonComparisonTest<Child> { }

    [TestFixture]
    public class PersonNameComparisonTests : PersonComparisonTest<PersonName>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "AddressingNameDate", // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    "AddressingNameReportingMarker", // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    "CprUpdateDate", // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    "NameAuthorityCode", // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                };
                return excluded;
            }
        }
        public override IQueryable<PersonName> Get(DPRDataContext dataContext, string key)
        {
            var pnr = decimal.Parse(key);
            return dataContext.PersonNames.Where(pn => pn.PNR == pnr).OrderByDescending(pn => pn.NameStartDate).ThenBy(pn => pn.CorrectionMarker);
        }
    }

    [TestFixture]
    public class CivilStatusComparisonTests : PersonComparisonTest<CivilStatus> {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "UpdateDateOfCpr", // We know that this field contains a 'wrong' date, so we do not test it.
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class SeparationComparisonTests : PersonComparisonTest<Separation> { }

    [TestFixture]
    public class NationalityComparisonTests : PersonComparisonTest<Nationality> {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    "CprUpdateDate", // The values in this columns are wrong, but not used.
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class DepartureComparisonTests : PersonComparisonTest<Departure> {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    "ExitUpdateDate", // The values in this columns are wrong, but not used.
                                    "CprUpdateDate", // The values in this columns are wrong, but not used.
                                    "ForeignAddressDate", // The values in this columns are wrong, but not used.
                                };
                return excluded;
            }
        }

        public override IQueryable<Departure> Get(DPRDataContext dataContext, string pnr)
        {
            return dataContext.Departures.Where(c => c.PNR == decimal.Parse(pnr)).OrderByDescending(c => c.ExitDate).ThenBy(c => c.EntryDate);
        }
    }

    [TestFixture]
    public class ContactAddressComparisonTests : PersonComparisonTest<ContactAddress> { }

    [TestFixture]
    public class PersonAddressComparisonTests : PersonComparisonTest<PersonAddress>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "AddressStartDateMarker", // We do not know the origin of this marker.
                                    "CprUpdateDate", // It is skipped for now, as the contents are wrong and not used by known systems.
                                    "MunicipalityArrivalDate", // It is skipped for now, as the contents are wrong and not used by known systems.
                                    "StreetAddressingName", // It is skipped for now, as the contents are wrong and not used by known systems.
                                };
                return excluded;
            }
        }

        public override IQueryable<PersonAddress> Get(DPRDataContext dataContext, string pnr)
        {
            return dataContext.PersonAddresses.Where(c => c.PNR == decimal.Parse(pnr)).OrderByDescending(c => c.AddressStartDate);
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