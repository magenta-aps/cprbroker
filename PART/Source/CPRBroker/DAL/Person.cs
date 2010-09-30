using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.DAL
{
    public partial class Person
    {
        /// <summary>
        /// Upgrades the detail level for the current person
        /// </summary>
        /// <param name="detailLevel"></param>
        public void UpgradeDetailLevel(DetailLevel.DetailLevelType detailLevel)
        {
            int detailLevelValue = (int)detailLevel;
            if (DetailLevelId == null || DetailLevelId < detailLevelValue)
            {
                DetailLevelId = detailLevelValue;
            }
        }

        public SimpleCPRPersonType ToSimpleCPRPerson()
        {
            return new SimpleCPRPersonType()
            {
                PersonCivilRegistrationIdentifier = PersonNumber,
                PersonNameStructure = new PersonNameStructureType(FirstName, MiddleName, LastName)
            };
        }

        /// <summary>
        /// Gets a Person object from database or creates a new one if not found
        /// A later call to SubmitChanges() is required
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="cprNumber">Person CPR number</param>
        /// <param name="personName">Name of the person to be set in case a new person is created</param>
        /// <param name="addAddress">Whether to add an Address object to the person</param>
        /// <returns>The database object representing a person</returns>
        public static Person GetPerson(CPRBrokerDALDataContext context, string cprNumber, PersonNameStructureType personName, bool addAddress)
        {
            return GetPersons(context, new string[] { cprNumber }, new PersonNameStructureType[] { personName }, addAddress).Single();
        }

        /// <summary>
        /// Gets a list of Person objects from database or creates new objects if not found
        /// A later call to SubmitChanges() is required
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="cprNumbers">Array of CPR numbers for persons to be found</param>
        /// <param name="personNames">Optional list of person names to be used to fill Person objects</param>
        /// <param name="addAddress">Whether to add an Address object to the Person objects</param>
        /// <returns></returns>
        public static List<Person> GetPersons(CPRBrokerDALDataContext context, string[] cprNumbers, PersonNameStructureType[] personNames, bool addAddress)
        {
            var dbPersons = (
                            from num in cprNumbers
                            join per in context.Persons on num equals per.PersonNumber into Person

                            select new
                            {
                                CprNumber = num,
                                Person = Person.FirstOrDefault()
                            }).ToArray();
            dbPersons = (from p in dbPersons
                         select new
                         {
                             CprNumber = p.CprNumber,
                             Person = p.Person != null ? p.Person : new Person()
                         }
                         ).ToArray();
            for (int i = 0; i < dbPersons.Count(); i++)
            {
                var person = dbPersons[i];
                if (person.Person.PersonId == Guid.Empty)
                {
                    person.Person.PersonId = Guid.NewGuid();
                    person.Person.PersonNumber = person.CprNumber;
                    person.Person.ModifiedDate = DateTime.Now;
                    person.Person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Number);
                    context.Persons.InsertOnSubmit(person.Person);
                }
                if (personNames != null && personNames.Length > i && personNames[i] != null)
                {
                    var personName = personNames[i];
                    person.Person.FirstName = personName.PersonGivenName;
                    person.Person.MiddleName = personName.PersonMiddleName;
                    person.Person.LastName = personName.PersonSurnameName;
                    person.Person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Name);
                }

                if (addAddress && person.Person.Address == null)
                {
                    person.Person.Address = new Address();
                }
            }

            return new List<Person>(from p in dbPersons select p.Person);
        }

        /// <summary>
        /// Creates a date from a CPR number
        /// </summary>
        /// <param name="cprNumber">CPR number</param>
        /// <returns></returns>
        public static DateTime? PersonNumberToDate(string cprNumber)
        {
            int day;
            int month;
            int year;
            int serialNo;
            try
            {
                if (!string.IsNullOrEmpty(cprNumber)
                    && cprNumber.Length >= 7
                    && int.TryParse(cprNumber.Substring(0, 2), out day)
                    && int.TryParse(cprNumber.Substring(2, 2), out month)
                    && int.TryParse(cprNumber.Substring(4, 2), out year)
                    && int.TryParse(cprNumber.Substring(6, 1), out serialNo)
                    )
                {
                    int centuryYears = 1900;
                    if (
                        (serialNo == 4 && year <= 36)
                        || (serialNo >= 5 && serialNo <= 8 && year <= 57)
                        || (serialNo == 9 && year <= 36)
                        )
                    {
                        centuryYears = 2000;
                    }
                    return new DateTime(centuryYears + year, month, day);
                }
            }
            catch { }
            return null;
        }


        /// <summary>
        /// Tries to set birthdate from cpr number
        /// </summary>
        partial void OnPersonNumberChanged()
        {
            if (!BirthDate.HasValue)
            {
                try
                {
                    DateTime? date = PersonNumberToDate(PersonNumber);
                    if (date.HasValue && date >= Constants.MinSqlDate && date <= Constants.MaxSqlDate)
                    {
                        BirthDate = date;
                    }
                }
                catch
                { }
            }
        }
    }
}
