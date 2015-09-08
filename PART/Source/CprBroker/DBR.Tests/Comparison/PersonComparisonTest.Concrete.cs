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
                    // Review 2.0
                    "PreviousMunicipalityName", // Usually it is name of municipality of previous address, but sometimes contains the value from the minucipality that is different from the current one!!

                    /* BELOW EXCLUSIONS ARE ONES THAT ARE NOT, CURRENTLY, USED BY ANY SYSTEMS - AND FAIL IN TESTS */
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
                                        // Review 2.0
                                        "BirthRegistrationDate", //Usually matches PersonInformation.PersonStartDate, but not always. 
                                        "BirthRegistrationPlaceUpdateDate", // CPR Services 'foedmynhaenstart' ? 
                                        "ChurchAuthorityCode", // Church district lookup? Not possible so far // CPR Services 'fkirkmynkod'
                                        "ChurchRelationUpdateDate", // Not available in CPR Extracts.
                                        "PnrDeletionDate", // Usually it is null, but is 0 a few times - excluding for now
                                        "UnderGuardianshipRelationType", //Usually it is null, but is 0 a few times - excluding for now
                                        "CustomerNumber", // This must differ from real DPR
                                        "FatherDocumentation", // CPR Services 'far_dok' ? // We do the best to match it, but it not exactly matching a real DPR.
                                        "MotherDocumentation", // CPR Services 'mor_dok' ? // We do the best to match it, but it not exactly matching a real DPR.                                        
                                        "KinshipUpdateDate", // CPR Services 'timestamp' ? // Not available in CPR Extracts
                                        "PaternityAuthorityCode", // CPR Services 'far_mynkod' ? // Not available in CPR Extracts
                                        "PaternityDate", // CPR Services 'farhaenstart' ? // Not available in CPR Extracts
                                        "UnderGuardianshipAuthorityCode", // CPR Services 'mynkod-ctumyndig' ? // Not available in CPR Extracts
                                        "BirthplaceTextUpdateDate", // CPR Services 'foedtxttimestamp' ? // Not available in CPR Extracts
                                        "JobDate" // CPR Services 'stillingsdato' ? // Not available in CPR Extracts
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
                                        // Review 2.0
                                        "AddressingNameDate", // CPR Services 'adrnvnhaenstart' ? // Not available in CPR Extracts
                                        "AddressingNameReportingMarker", // CPR Services 'indrap' ? // Not available in CPR Extracts
                                        "NameAuthorityCode", // CPR Services 'mynkod' ?// Not available in CPR Extracts
                                };
                return excluded;
            }
        }

        public override string[] GetOrderByColumnNames()
        {
            return new string[] { 
                "NVHAENST"
            };
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
                                    // Review 2.0
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
                                    // Review 2.0
                                    "ExitUpdateDate", // CPR Services 'udrtimestamp' ?// The values in this columns are wrong, but not used.
                                    "ForeignAddressDate", // CPR Services 'udlandadrdto' ?// The values in this columns are wrong, but not used.
                                    "EntryUpdateDate", // CPR Services 'indrtimestamp' ?
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
                    // Review 2.0
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
                                        // Review 2.0
                                        "MunicipalityArrivalDate" // Sometimes it matches start date of previous address, other times matches birthdate
                                };
                return excluded;
            }
        }

        public override string[] GetOrderByColumnNames()
        {
            return new string[] { 
                "TILFDTO" // Address StartDate
            };
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
    public class DisappearanceComparisonTests : PersonComparisonTest<Disappearance>
    {
        // Review 2.0
    }

    [TestFixture]
    public class EventComparisonTests : PersonComparisonTest<Event>
    {
        // Review 2.0
    }

    [TestFixture]
    public class NoteComparisonTests : PersonComparisonTest<Note>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]{
                    // Review 2.0
                    "MunicipalityCode" // CPR Services 'komkod'
                };
            }
        }
    }

    [TestFixture]
    public class MunicipalConditionComparisonTests : PersonComparisonTest<MunicipalCondition>
    {
        // Review 2.0
    }

    [TestFixture]
    public class ParentalAuthorityComparisonTests : PersonComparisonTest<ParentalAuthority>
    {
        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]{
                    // Review 2.0
                    "CustodyStartAuthorityCode", // Only in CPR Services 'mynkod_start' ?
                };
            }
        }
    }

    [TestFixture]
    public class GuardianAndParentalAuthorityRelationComparisonTests : PersonComparisonTest<GuardianAndParentalAuthorityRelation>
    {
        // Review 2.0
    }

    [TestFixture]
    public class GuardianAddressComparisonTests : PersonComparisonTest<GuardianAddress>
    {
        // Review 2.0
    }

}