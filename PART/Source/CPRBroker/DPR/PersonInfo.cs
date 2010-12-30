using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Used as a link between miscellaneous person tables & person information
    /// </summary>
    internal partial class PersonInfo
    {
        public PersonName PersonName { get; set; }
        public PersonTotal PersonTotal { get; set; }
        public Street Street { get; set; }
        public ContactAddress ContactAddress { get; set; }
        //TODO: remove this field
        public bool HasProtection { get; set; }
        public IQueryable<CivilStatus> CivilStates { get; set; }

        /// <summary>
        /// LINQ expression that is able to create a IQueryable&lt;PersonInfo;gt; object based on a given date
        /// </summary>
        internal static readonly Expression<Func<DateTime, DPRDataContext, IQueryable<PersonInfo>>> PersonInfoExpression = (DateTime today, DPRDataContext dataContext) =>
            from personName in dataContext.PersonNames
            join personTotal in dataContext.PersonTotals on personName.PNR equals personTotal.PNR
            join street in dataContext.Streets on new { personTotal.MunicipalityCode, personTotal.StreetCode } equals new { street.MunicipalityCode, street.StreetCode } into strt
            join contactAddress in dataContext.ContactAddresses on personName.PNR equals contactAddress.PNR into contactAddr
            // TODO correct this condition
            where
            personName.NameTerminationDate == null
            select new PersonInfo()
            {
                PersonName = personName,
                PersonTotal = personTotal,
                Street = strt.FirstOrDefault(),
                ContactAddress = contactAddr.SingleOrDefault(),
                // TODO: include protection type with PNR because the index is on PNR & ProtectionType                
                HasProtection = (
                   from protection in dataContext.Protections
                   where protection.StartDate <= today && (!protection.EndDate.HasValue || protection.EndDate > today)
                   select protection.PNR
                ).Contains(personName.PNR),
                //TODO: Beware that there might be time range intersections in the last day of an older period, like a marriage period after a divorce period
                CivilStates = (from civ in dataContext.CivilStatus where !civ.CorrectionMarker.HasValue && (civ.PNR == personTotal.PNR || civ.SpousePNR == personTotal.PNR) select civ),
            };

        /// <summary>
        /// Converts the onject to RegularCPRPersonType
        /// </summary>
        /// <returns></returns>
        internal RegularCPRPersonType ToRegularCprPerson()
        {
            RegularCPRPersonType regularCprPerson = new RegularCPRPersonType();

            // Birthdate
            regularCprPerson.PersonBirthDateStructure = new PersonBirthDateStructureType();
            regularCprPerson.PersonBirthDateStructure.BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value;
            regularCprPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator = false;

            // Civil status
            regularCprPerson.PersonCivilRegistrationStatusStructure = new PersonCivilRegistrationStatusStructureType();
            regularCprPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode
                = Schemas.Util.Enums.ToCivilRegistrationStatus(this.PersonTotal.Status);
            // TODO : Returns different date than KMD AS78207 for PNR = 2811781323
            regularCprPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate = Utilities.DateFromFirstDecimal(PersonTotal.StatusDate, PersonTotal.AddressDate).Value;


            // Gender
            switch (PersonTotal.Sex)
            {
                case 'M':
                    regularCprPerson.PersonGenderCode = PersonGenderCodeType.male;
                    break;
                case 'K':
                    regularCprPerson.PersonGenderCode = PersonGenderCodeType.female;
                    break;
                default:
                    regularCprPerson.PersonGenderCode = PersonGenderCodeType.unknown;
                    break;
            }

            // Protection
            regularCprPerson.PersonInformationProtectionIndicator = HasProtection;

            // Name for addressing
            regularCprPerson.PersonNameForAddressingName = PersonName.AddressingName.Trim();

            // Simple CPR Person
            regularCprPerson.SimpleCPRPerson = this.PersonName.ToSimpleCprPerson();

            return regularCprPerson;
        }

    }

    /// <summary>
    /// Represents a pair of database relation object and its corresponding PersonInfo object
    /// </summary>
    /// <typeparam name="TRelation">Type of database relation object</typeparam>
    internal class RelationInfo<TRelation>
    {
        public TRelation RelationObject { get; set; }
        public PersonInfo RelatedPersonInfo { get; set; }
    }

    /// <summary>
    /// Represents a relationship in both its database and OIO objects
    /// </summary>
    /// <typeparam name="TDbRelation">Type of database relation object</typeparam>
    /// <typeparam name="TOioRelation">Type of OIO relation object</typeparam>
    internal class Relation<TDbRelation, TOioRelation>
    {
        public TDbRelation DbRelation = default(TDbRelation);
        public TOioRelation OioRelation;
    }


    internal class PersonInfo2 : PersonInfo
    {
        public DateTime RegistrationDate = DateTime.Today;

        /// <summary>
        /// Creates a cross product of the person database objects based on the possible registration dates
        /// </summary>
        /// <param name="personInfos"></param>
        /// <returns></returns>
        public static System.Collections.Generic.ICollection<PersonInfo2> Populate(IQueryable<PersonInfo> personInfos)
        {
            var ret = new List<PersonInfo2>();

            foreach (var personInfo in personInfos)
            {
                var personInfoAsQueryable = new PersonInfo[] { personInfo }.AsQueryable();
                var dates = new List<DateTime?>();

                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MaritalStatusDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MunicipalityArrivalDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MunicipalityLeavingDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.PaternityDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.PersonalSelectionDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.StatusDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.UnderGuardianshipDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.VotingDate));

                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.AddressingNameDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.AuthorityTextUpdateDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.CprUpdateDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameStartDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameTerminationDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.SearchNameDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.StatusDate));

                dates.Add(Utilities.DateFromDecimal(personInfo.ContactAddress.ContactAddressDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.ContactAddress.UpdateDate));

                dates.Add(Utilities.DateFromDecimal(personInfo.ContactAddress.ContactAddressDate));
                dates.Add(Utilities.DateFromDecimal(personInfo.ContactAddress.UpdateDate));

                dates.AddRange((from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalStatusDate)));
                dates.AddRange((from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalEndDate)));
                dates.AddRange((from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.AuthorityTextUpdateDate)));
                dates.AddRange((from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.UpdateDateOfCpr)));


                dates = dates
                    .Where((d) => d.HasValue)
                    .Distinct()
                    .ToList();

                ret.AddRange((from pi in personInfoAsQueryable
                              from d in dates.AsQueryable()
                              select new PersonInfo2()
                              {
                                  CivilStates = pi.CivilStates,
                                  ContactAddress=pi.ContactAddress,
                                  HasProtection = pi.HasProtection,
                                  PersonName=pi.PersonName,
                                  PersonTotal=pi.PersonTotal,
                                  Street=pi.Street,
                                  RegistrationDate = d.Value
                              }
                          ));
            }
            return ret;
        }
    }

}
