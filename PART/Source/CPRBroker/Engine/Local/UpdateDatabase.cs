using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.DAL;
using CprBroker;

namespace CprBroker.Engine.Local
{
    public partial class UpdateDatabase
    {
        #region private methods

        /// <summary>
        /// Udates a person's address
        /// A later call to SubmitChanges is required
        /// </summary>
        /// <param name="oioAddress">OIO object representing the address</param>
        /// <param name="addressIdentifierCode"></param>
        /// <param name="dbPerson">Database object representing the person of interest</param>
        private static void UpdateAddress(object oioAddress, AddressIdentifierCodeType addressIdentifierCode, Person dbPerson)
        {
            Schemas.Util.Address tempAddress = Schemas.Util.Address.FromOioAddress(oioAddress);

            Address dbAddress = dbPerson.Address;

            //dbAddress.AddressStatusId = "";
            dbAddress.CareOfName = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.CareOfName, false);
            dbAddress.DistrictSubDivisionIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.DistrictSubDivisionIdentifier, false);
            dbAddress.FloorIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Floor, false);
            dbAddress.Line1 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line1, false);
            dbAddress.Line2 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line2, false);
            dbAddress.Line3 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line3, false);
            dbAddress.Line4 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line4, false);
            dbAddress.Line5 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line5, false);
            dbAddress.Line6 = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Line6, false);
            dbAddress.LocationDescription = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.LocationDescription, false);
            dbAddress.MailDeliverSubLocationIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.MailDeliverSubLocationIdentifier, false);
            dbAddress.StreetBuildingIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Building, false);

            dbAddress.SuiteIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.Door, false);


            using (CPRBrokerDALDataContext lookupContext = new CPRBrokerDALDataContext())
            {
                // Country
                if (tempAddress.ContainsKey(CprBroker.Schemas.Util.AddressField.Alpha2CountryCode))
                {
                    dbAddress.CountryAlpha2Code = tempAddress[CprBroker.Schemas.Util.AddressField.Alpha2CountryCode];
                }
                else if (tempAddress.ContainsKey(CprBroker.Schemas.Util.AddressField.EnglishCountryName))
                {
                    dbAddress.CountryAlpha2Code = Country.GetCountryAlpha2CodeByEnglishName(tempAddress[CprBroker.Schemas.Util.AddressField.EnglishCountryName]);
                }
                else if (tempAddress.ContainsKey(CprBroker.Schemas.Util.AddressField.DanishCountryName))
                {
                    dbAddress.CountryAlpha2Code = Country.GetCountryAlpha2CodeByDanishName(tempAddress[CprBroker.Schemas.Util.AddressField.DanishCountryName]);
                }

                // Municipality
                if (tempAddress.ContainsKey(CprBroker.Schemas.Util.AddressField.MunicipalityCode))
                {
                    dbAddress.MunicipalityCode = tempAddress[CprBroker.Schemas.Util.AddressField.MunicipalityCode];
                }
                else if (tempAddress.ContainsKey(CprBroker.Schemas.Util.AddressField.MunicipalityName))
                {
                    dbAddress.MunicipalityCode =
                        (from m in lookupContext.Municipalities
                         where m.MunicipalityName == tempAddress[CprBroker.Schemas.Util.AddressField.MunicipalityName]
                         select m.MunicipalityCode
                        ).SingleOrDefault();
                }

                // Street
                dbAddress.StreetCode = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.StreetCode, false);
                dbAddress.StreetName = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.StreetName, false);
                dbAddress.StreetNameForAddressing = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.StreetNameForAddressing, false);

                // Post
                dbAddress.PostBoxIdentifier = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.PostBox, false);
                dbAddress.PostCode = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.PostCode, false);
                dbAddress.PostDistrictName = tempAddress.GetFieldValue(CprBroker.Schemas.Util.AddressField.PostDistrictName, false);
            }
        }

        #endregion

        #region Person data
        /// <summary>
        /// Updates a person's name and address
        /// </summary>
        /// <param name="oioPerson"></param>
        public static void UpdateCitizenNameAndAddress(PersonNameAndAddressStructureType oioPerson)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                loadOptions.LoadWith<Person>((p) => p.Address);
                context.LoadOptions = loadOptions;

                string cprNumber = oioPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                var dbPerson = Person.GetPerson(context, cprNumber, null, true);

                #region Update Basic Information
                dbPerson.PersonProtectionIndicator = oioPerson.PersonInformationProtectionIndicator;
                dbPerson.PersonNumber = oioPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                dbPerson.FirstName = oioPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName;
                dbPerson.MiddleName = oioPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName;
                dbPerson.LastName = oioPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName;
                #endregion

                UpdateAddress(oioPerson.Item, oioPerson.AddressIdentifierCode, dbPerson);

                dbPerson.UpgradeDetailLevel(DetailLevel.DetailLevelType.NameAndAddress);

                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Updates a person's basic data
        /// </summary>
        /// <param name="oioPerson"></param>
        public static void UpdateCitizenBasic(PersonBasicStructureType oioPerson)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                loadOptions.LoadWith<Person>((p) => p.Address);
                context.LoadOptions = loadOptions;

                string cprNumber = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                var dbPerson = Person.GetPerson(context, cprNumber, null, true);

                #region Update Basic Information
                if (oioPerson.PersonDeathDateStructure != null)
                {
                    if (oioPerson.PersonDeathDateStructure.PersonDeathDate != null && oioPerson.PersonDeathDateStructure.PersonDeathDate > DateTime.MinValue)
                    {
                        dbPerson.DeathDate = oioPerson.PersonDeathDateStructure.PersonDeathDate;
                    }
                    dbPerson.DeathDateUncertaintyIndicator = oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator;
                }
                

                if (oioPerson.RegularCPRPerson.SimpleCPRPerson != null)
                {
                    dbPerson.PersonNumber = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                    dbPerson.FirstName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName;
                    dbPerson.MiddleName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName;
                    dbPerson.LastName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName;
                    dbPerson.PersonProtectionIndicator = oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator;
                }
                dbPerson.NameForAddressing = oioPerson.RegularCPRPerson.PersonNameForAddressingName;
                if (oioPerson.RegularCPRPerson.PersonBirthDateStructure != null)
                {
                    dbPerson.BirthDate = oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDate;
                    dbPerson.BirthDateUncertaintyIndicator = oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator;
                }
                if (oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure != null)
                {
                    dbPerson.PersonStatusTypeCode = PersonStatusType.GetCode(oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode);
                    dbPerson.PersonStatusDate = Util.Sql.GetSqlDateTime(oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate);
                }
                dbPerson.GenderId = Gender.GetCode(oioPerson.RegularCPRPerson.PersonGenderCode);

                dbPerson.PersonProtectionIndicator = oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator;

                dbPerson.ModifiedDate = DateTime.Now;

                dbPerson.MaritalStatusTypeId = MaritalStatusType.GetCode(oioPerson.MaritalStatusCode);

                dbPerson.NationalityCountryAlpha2Code = oioPerson.PersonNationalityCode;


                #endregion

                dbPerson.UpgradeDetailLevel(DetailLevel.DetailLevelType.BasicData);
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Updates a person's full information
        /// </summary>
        /// <param name="oioPerson"></param>
        public static void UpdateCitizenFull(PersonFullStructureType oioPerson)
        {
            UpdateCitizenFull(oioPerson, false);
        }

        /// <summary>
        /// Updates a person's full data
        /// </summary>
        /// <param name="oioPerson">Object containing the person's full data</param>
        /// <param name="isTest">Whether the person is a test person</param>
        public static void UpdateCitizenFull(PersonFullStructureType oioPerson, bool isTest)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                loadOptions.LoadWith<Person>((p) => p.Address);
                context.LoadOptions = loadOptions;

                string cprNumber = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                var dbPerson = Person.GetPerson(context, cprNumber, null, true);

                #region Setting Basic Information
                if (oioPerson.PersonDeathDateStructure != null)
                {
                    dbPerson.DeathDate = oioPerson.PersonDeathDateStructure.PersonDeathDate;
                    dbPerson.DeathDateUncertaintyIndicator = oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator;
                }

                dbPerson.PersonNumber = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonCivilRegistrationIdentifier;
                dbPerson.FirstName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName;
                dbPerson.MiddleName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonMiddleName;
                dbPerson.LastName = oioPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName;
                dbPerson.NameForAddressing = oioPerson.RegularCPRPerson.PersonNameForAddressingName;

                dbPerson.PersonProtectionIndicator = oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator;

                if (oioPerson.RegularCPRPerson.PersonBirthDateStructure != null)
                {
                    dbPerson.BirthDate = oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDate;
                    dbPerson.BirthDateUncertaintyIndicator = oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator;
                }


                if (oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure != null)
                {
                    dbPerson.PersonStatusTypeCode = PersonStatusType.GetCode(oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode);
                    dbPerson.PersonStatusDate = Util.Sql.GetSqlDateTime(oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate);
                }

                dbPerson.GenderId = (int)oioPerson.RegularCPRPerson.PersonGenderCode;

                dbPerson.PersonProtectionIndicator = oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator;


                dbPerson.MaritalStatusTypeId = MaritalStatusType.GetCode(oioPerson.MaritalStatusCode);

                dbPerson.NationalityCountryAlpha2Code = oioPerson.PersonNationalityCode;


                #endregion

                UpdateAddress(oioPerson.Item, oioPerson.AddressIdentifierCode, dbPerson);

                dbPerson.IsTestPerson = isTest;
                #region Other Information

                // TODO: change full person type to hold children rather than their number

                // TODO : Use SimpleCPRPersonType for souse instead of name

                #endregion

                dbPerson.UpgradeDetailLevel(DetailLevel.DetailLevelType.FullData);
                context.SubmitChanges();
            }
        }

        #endregion

        /// <summary>
        /// Updates the list of person's children
        /// </summary>
        /// <param name="cprNumber">Person CPR number</param>
        /// <param name="children">The person's children</param>
        public static void UpdateCitizenChildren(string cprNumber, SimpleCPRPersonType[] children)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var person = Person.GetPerson(context, cprNumber, null, false);
                var childRelations = (from child in children select new ChildRelationshipType() { SimpleCPRPerson = child }).ToArray();
                Relationship.MergePersonRelationshipsByType<ChildRelationshipType>(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.ParentChild,
                    true,
                    childRelations,
                    (oioRel, dbRel) => dbRel.ChildRelationship = new ChildRelationship()
                    );

                person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Number);
                // Save to database
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Updates the given person's relations
        /// </summary>
        /// <param name="cprNumber">Person CPR number</param>
        /// <param name="personRelations">Object representing the person's relations</param>
        public static void UpdateCitizenRelations(string cprNumber, PersonRelationsType personRelations)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var person = Person.GetPerson(context, cprNumber, personRelations.SimpleCPRPerson.PersonNameStructure, false);
                person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Name);

                // Merge with database objects
                // -----------------------------------
                // Parents
                Relationship.MergePersonRelationshipsByType(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.ParentChild,
                    false,
                    personRelations.Parents.ToArray(),
                    (oioRel, dbRel) => dbRel.ChildRelationship = new ChildRelationship()
                );

                // Children
                Relationship.MergePersonRelationshipsByType(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.ParentChild,
                    true,
                    personRelations.Children.ToArray(),
                    (oioRel, dbRel) => dbRel.ChildRelationship = new ChildRelationship()
                );

                Action<TimedRelationshipType, Relationship> timedRelationshipAction =
                    (oioRel, dbRel) =>
                        dbRel.TimedRelationship = new TimedRelationship()
                        {
                            StartDate = oioRel.RelationStartDate,
                            EndDate = oioRel.RelationEndDate
                        };

                // Spouse            
                Relationship.MergePersonRelationshipsByType(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.Marital,
                    null,
                    personRelations.Spouses.ToArray(),
                    (oioRel, dbRel) =>
                    {
                        timedRelationshipAction(oioRel, dbRel);
                        dbRel.TimedRelationship.MaritalRelationship = new MaritalRelationship();
                    }
                );

                // Authoritative Parents
                Relationship.MergePersonRelationshipsByType(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.Custody,
                    false,
                    personRelations.AuthoritativeParents.ToArray(),
                    (oioRel, dbRel) =>
                    {
                        timedRelationshipAction(oioRel, dbRel);
                        dbRel.TimedRelationship.CustodyRelationship = new CustodyRelationship();
                    }
                );

                // Custodied children
                Relationship.MergePersonRelationshipsByType(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.Custody,
                    true,
                    personRelations.CustodiedChildren.ToArray(),
                    (oioRel, dbRel) =>
                    {
                        timedRelationshipAction(oioRel, dbRel);
                        dbRel.TimedRelationship.CustodyRelationship = new CustodyRelationship();
                    }
                );

                // Save to database
                context.SubmitChanges();
            }
        }

    }
}
