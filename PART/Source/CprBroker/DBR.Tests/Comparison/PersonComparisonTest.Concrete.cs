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
                string[] excluded = 
                {
                    /* BELOW EXCLUSIONS ARE ONES THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                    
                    "DirectoryProtectionMarker", // TODO: Lookup ?
                    "AddressProtectionMarker", // TODO: Lookup ?
                    // There seems no correlation between these markers and the rows in protection
                    
                    "SpouseMarker", // Lookup? // We do not know the origin of this marker.
                    // No correlation found with rows in civil status
                                    
                    "PaternityAuthorityName", // CPR Services 'far_mynkod' ?
                    //"AddressDateMarker", // We do not know the origin of this marker.
                    //"PreviousAddress", // This field is not fully implemented because it is not used.
                    //"PreviousMunicipalityName", // This field is not fully implemented because it is not used.

                    // Extra exclusions - DO NOT COMMIT
                    "DprLoadDate", // Irrelevant for comparison
                    //"MunicipalityArrivalDate", // Already implemented
                    //"MunicipalityLeavingDate", // Already implemented
                    //"PostDistrictName", // Already implemented
                    //"PreviousAddress", // Already implemented
                    //"PreviousMunicipalityName", // Already implemented
                    //"PaternityDate", // Already implemented
                    "FatherMarker", // DPR Specific // TODO: Get from Parents record (FARNVN_MRK)
                    "MotherMarker", // DPR Specific // TODO: Get from Parents record (MORNVN_MRK)
                    //"ExitEntryMarker", // Already implemented //Is this DPR specific?
                    "ApplicationCode", // DPR Specific
                    //"BirthplaceText", // lookup
                    
                    "MaritalAuthorityName", // CPR Services 'mynkod' ?
                    // Difference between test & production: It is possible in test to have DTTOTAL without DTCIV, which does not happen in production
                    // TODO: Get the field from latest civil status -> authority code -> join with authority -> name (matches more than 99%, the rest seems to be outdated data)
                    // New finding: no authority name in current and historical civil status records coming from CPR extracts

                    //"AddressDate", // Already implemented
                    //"SpousePersonalOrBirthdate", // Already implemented
                    //"StandardAddress", // Already implemented
                    //"PnrMarkingDate", // Always null in DPR
                    
                    "NationalMemoMarker" // TODO: get from DTNOTAT
                    // There seems no correlation in DPR between this field and the rows in DTNOTAT
                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class PersonComparisonTest : PersonComparisonTest<CprBroker.Providers.DPR.Person>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    //"BirthplaceText", //Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"BirthRegistrationDate", //Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "BirthRegistrationPlaceUpdateDate", // CPR Services 'foedmynhaenstart' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"ChurchAuthorityCode", // Church district lookup? // CPR Services 'fkirkmynkod' // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"ChurchDate", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"ChurchRelationUpdateDate", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"CprUpdateDate", Already implemented 
                                    /* 
                                                      * We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                                      * The field should contain a chronologically sorted list, so that client systems can use it for sorting rows.
                                                      */

                                    //"CurrentPnr", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "CurrentPnrUpdateDate", // CPR Services 'timestamp' // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "CustomerNumber", // DPR Specific // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "FatherDocumentation", // CPR Sevices 'far_dok' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"FatherName", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"Job", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "KinshipUpdateDate", // CPR Services 'timestamp' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "MotherDocumentation", // cpr sERVICES 'mor_dok' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    //"MotherName", // Already implemented // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PaternityAuthorityCode", // CPR Services 'far_mynkod' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PaternityDate", // CPR Services 'farhaenstart' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "PnrDate", // CPR Services 'pnrmrkhaenstart' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.
                                    "UnderGuardianshipAuthprityCode", // CPR Services 'mynkod-ctumyndig' ? // We know that this field contains wrong data, but it is not used by known systems, so we skip it in the tests.

                                    // EXTRA - do not commit
                                    "PnrMarkingDate", // CPR Services 'pnrhaenstart' ?
                                    //"UnderGuardianshipRelationType", // Already implemented
                                    "BirthplaceTextUpdateDate", // CPR Services 'foedtxttimestamp' ?
                                    "JobDate" // CPR Services 'stillingsdato' ?
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class ChildComparisonTests : PersonComparisonTest<Child>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[] { 
                    // EXTRA - do not commit
                    "MotherOrFatherDocumentation", // CPR Service ?
                };
            }
        }
    }

    [TestFixture]
    public class PersonNameComparisonTests : PersonComparisonTest<PersonName>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    "AddressingNameDate", // CPR Services 'adrnvnhaenstart' ? // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    "AddressingNameReportingMarker", // CPR Services 'indrap' ? // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    //"CprUpdateDate", // Already implemented // We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.
                                    "NameAuthorityCode", // CPR Services 'mynkod' ?// We know that this field contains a 'wrong' date, but it is not used by known systems, so we skip it in the tests.

                                    // EXTRA- DO NOT COMMIT
                                    // "NameStartDate", // Already implemented
                                    //"SurnameMarker", // Already implemented
                                    //"MotherOrFatherDocumentation", // TODO: Not a DPR field !!
                                    // "NameTerminationDate" // some approximation differences on 'second' level // TODO: Ignore seconds in datetime comparison
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
    public class CivilStatusComparisonTests : PersonComparisonTest<CivilStatus>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                        // Review 2.0
                                    "MaritalStatusAuthorityCode", // CPR Services 'mynkod' ?
                                    "SpouseDocumentation", // CPR Services 'aegtedok' ?
                                    "AuthorityTextUpdateDate", // CPR Services, myntxttimestamp
                                    "MaritalStatusAuthorityText"// CPR Services, myntxt
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class SeparationComparisonTests : PersonComparisonTest<Separation>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[] { 
                    //"CprUpdateDate", // Already implemented // CPR Direct has no time component
                };
            }
        }
    }

    [TestFixture]
    public class NationalityComparisonTests : PersonComparisonTest<Nationality>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    //"CprUpdateDate", // Already implemented // The values in this columns are wrong, but not used.

                                    // EXTRA - DO NOT COMMIT
                                    //"CorrectionMarker", // Already implemented
                                    //"CountryCode", // Already implemented
                                    //"NationalityEndDate", // Already implemented
                                    //"NationalityStartDate", // Already implemented
                                };
                return excluded;
            }
        }
    }

    [TestFixture]
    public class DepartureComparisonTests : PersonComparisonTest<Departure>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    "ExitUpdateDate", // CPR Services 'udrtimestamp' ?// The values in this columns are wrong, but not used.
                                    //"CprUpdateDate", // The values in this columns are wrong, but not used.
                                    "ForeignAddressDate", // CPR Services 'udlandadrdto' ?// The values in this columns are wrong, but not used.

                                    // EXTRA - do not commit
                                    "EntryUpdateDate", // CPR Services 'indrtimestamp' ?
                                    //"ExitCountryCode" // Already implemented
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
    public class ContactAddressComparisonTests : PersonComparisonTest<ContactAddress>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]{
                    //"CprUpdateDate", // Already implemented // Data from CPR extracts only has a date but no time
                    "MunicipalityCode", // FROM CPR services or maybe other records
                };
            }
        }
    }

    [TestFixture]
    public class PersonAddressComparisonTests : PersonComparisonTest<PersonAddress>
    {
        override public string[] ExcludedProperties
        {
            get
            {
                string[] excluded = {
                                    /* BELOW EXCLUSIONS ARE ONCE THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
                                    //"AddressStartDateMarker", // Already implemented // We do not know the origin of this marker.
                                    //"CprUpdateDate", // Already implemented. //It is skipped for now, as the contents are wrong and not used by known systems.
                                    "MunicipalityArrivalDate", // TODO: Implement in histoical records // It is skipped for now, as the contents are wrong and not used by known systems.
                                    //"StreetAddressingName", // Already implemented //It is skipped for now, as the contents are wrong and not used by known systems.

                                    // EXTRA - do not commit
                                    //"PostCode", // Already implemented
                                    //"CorrectionMarker", // Already implemented
                                    //"MunicipalityCode", // Already implemented
                                    //"MunicipalityName", // Already implemented
                                    //"StreetCode", // Already implemented
                                    //"AddressEndDate",// Already implemented
                                    //"DoorNumber", // Already implemented
                                    //"LeavingFromMunicipalityCode", // TODO: implement in historical adresses
                                    //"LeavingFromMunicipalityDate", // TODO: implement in historical adresses
                                    //"AddressStartDate" // minor approximation problems on millisecond level. TODO: Ignore milliseconds in datetime comparison
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
    public class ProtectionComparisonTests : PersonComparisonTest<Protection>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new[]{
                    // EXTRA - do not commit
                    // "CprUpdateDate", // Implemented
                    //"ProtectionType", // Implemented
                    "ReportingMarker", // CPR Services 'indrap' ?
                };
            }
        }
    }

    [TestFixture]
    public class DisappearanceComparisonTests : PersonComparisonTest<Disappearance> { }

    [TestFixture]
    public class EventComparisonTests : PersonComparisonTest<Event> { }

    [TestFixture]
    public class NoteComparisonTests : PersonComparisonTest<Note>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]{
                    // Extra - do not commit
                    //"CprUpdateDate", // Implemented
                    "MunicipalityCode" // CPR Services 'komkod'
                };
            }
        }
    }

    [TestFixture]
    public class MunicipalConditionComparisonTests : PersonComparisonTest<MunicipalCondition> { }

    [TestFixture]
    public class ParentalAuthorityConditionComparisonTests : PersonComparisonTest<ParentalAuthority>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]{
                    // EXTRA - DO NOT COMMIT
                    //"CprUpdateDate", // Implemented
                    "CustodyStartAuthorityCode", // Only in CPR Services 'mynkod_start' ?
                    // "StartDateMarker" //Now implemented
                };
            }
        }
    }

    [TestFixture]
    public class GuardianAndParentalAuthorityRelationComparisonTests : PersonComparisonTest<GuardianAndParentalAuthorityRelation> { }

    [TestFixture]
    public class GuardianAddressComparisonTests : PersonComparisonTest<GuardianAddress> { }

}