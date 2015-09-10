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
                    "SpouseMarker", // Not available in CPR Extracts
                    "PaternityAuthorityName", // CPR Services 'far_mynkod' // Not available in CPR Extracts
                    "DprLoadDate", // Irrelevant for comparison
                    "FatherMarker", // No correlation found yet
                    "MotherMarker", // No correlation found yet
                    "ApplicationCode", // DPR Specific
                    "MaritalAuthorityName", // CPR Services 'mynkod' ? // Not available in CPR Extracts
                    
                    // Review 2.1
                    "PreviousAddress", // Some records are parts of municipalities without possible lookup
                    "CurrentMunicipalityName", // Some dead (status 90) people have a value from latest address while others do not

                    // Review 2.2
                    "AddressDateMarker", // Like PersonAddress.AddressStartDateMarker, Some real DPR records have a value that has no origin in CPR Extracts
                    "CareOfName", // Some real DPR records have a value that comes from an address that is marked as 'A' (Undo). 
                    "DataRetrievalType", // Always 'D' (from CPR extract with subscription) in DBR emulation
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
                    // Review 2.0
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
                                        "MunicipalityArrivalDate", // Sometimes it matches start date of previous address, other times matches birthdate

                                        // Review 2.1
                                        "LeavingFromMunicipalityCode", // Some records gets this value from address records that have a correction marker - unreliable source
                                        "LeavingFromMunicipalityDate", // Some records have no previous address but have a value equal to MunicipalityArrivalDate, which seems inconsistent
                                        "Location", // Not available in CPR extracts for historical addresses 

                                        //Review 2.2
                                        "AddressStartDateMarker", // Some real DPR records have a value that has no origin in CPR Extracts
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
                    // Review 2.0
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

        public override bool IgnoreCount
        {
            get
            {
                // Review 2.2
                return true; // Some records have notes in DPR with no notes in CPR Extracts
            }
        }
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