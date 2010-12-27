using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IDataProvider, IPersonNameAndAddressDataProvider, IPersonBasicDataProvider, IPersonFullDataProvider, IPersonRelationsDataProvider, IPersonChildrenDataProvider, IPersonCustodyDataProvider, ITestCitizenManager
    {
        #region IDataProvider Members

        bool IDataProvider.IsAlive()
        {
            return true;
        }

        Version IDataProvider.Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Converts a database address to an OIO address
        /// </summary>
        /// <param name="personAddress">Database address</param>
        /// <returns></returns>
        private object GetAddress(DAL.Address personAddress)
        {
            /*
             * The idea here is to fill the Schemas.UtilAddress fields and then return its ToOioAddress() method
            */

            Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();
            address[AddressField.Building] = personAddress.StreetBuildingIdentifier;
            address[AddressField.CareOfName] = personAddress.CareOfName;
            address[AddressField.Door] = personAddress.SuiteIdentifier;
            address[AddressField.Floor] = personAddress.FloorIdentifier;
            address[AddressField.HouseNumber] = personAddress.StreetBuildingIdentifier;
            address[AddressField.Line1] = personAddress.Line1;
            address[AddressField.Line2] = personAddress.Line2;
            address[AddressField.Line3] = personAddress.Line3;
            address[AddressField.Line4] = personAddress.Line4;
            address[AddressField.Line5] = personAddress.Line5;
            address[AddressField.Line6] = personAddress.Line6;


            address[AddressField.MunicipalityCode] = personAddress.MunicipalityCode;
            if (personAddress.Municipality != null)
            {
                address[AddressField.MunicipalityName] = personAddress.Municipality.MunicipalityName;
            }
            address[AddressField.StreetCode] = personAddress.StreetCode;
            address[AddressField.StreetName] = personAddress.StreetName;
            address[AddressField.StreetNameForAddressing] = personAddress.StreetNameForAddressing;

            address[AddressField.PostCode] = personAddress.PostCode;
            address[AddressField.PostDistrictName] = personAddress.PostDistrictName;

            return address.ToOioAddress(PersonCivilRegistrationStatusCodeType.Item01);
        }

        /// <summary>
        /// Retruns a Person object with at least the given detail level from the database
        /// </summary>
        /// <param name="cprNumber">CPR number</param>
        /// <param name="detailLevel">Minimum detail level</param>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        private Person GetPersonFromDatabase(string cprNumber, DetailLevel.DetailLevelType detailLevel, CPRBrokerDALDataContext context)
        {
            System.Data.Linq.DataLoadOptions loadOptios = new System.Data.Linq.DataLoadOptions();
            loadOptios.LoadWith<DAL.Person>((per) => per.Address);
            loadOptios.LoadWith<DAL.Address>((adr) => adr.Municipality);
            context.LoadOptions = loadOptios;

            var person = context.Persons.FirstOrDefault(
                per =>
                    per.PersonNumber == cprNumber
                    && per.DetailLevelId >= (int)detailLevel
            );

            return person;
        }

        #endregion

        #region IPersonNameAndAddressDataProvider

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = null;
            PersonNameAndAddressStructureType oioPerson = null;
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var person = GetPersonFromDatabase(cprNumber, DetailLevel.DetailLevelType.NameAndAddress, context);

                if (person != null)
                {
                    oioPerson = new PersonNameAndAddressStructureType();

                    #region Setting Basic Information
                    if (person.PersonProtectionIndicator.HasValue)
                    {
                        oioPerson.PersonInformationProtectionIndicator = person.PersonProtectionIndicator.Value;
                    }
                    oioPerson.SimpleCPRPerson = person.ToSimpleCPRPerson();
                    #endregion

                    oioPerson.Item = GetAddress(person.Address);
                    qualityLevel = QualityLevel.LocalCache;
                }
                return oioPerson;
            }
        }
        #endregion

        #region IPersonDataProvider Members
        public PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = QualityLevel.LocalCache;

            PersonBasicStructureType oioPerson = null;
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var person = GetPersonFromDatabase(cprNumber, DetailLevel.DetailLevelType.BasicData, context);

                if (person != null)
                {
                    // Simply fill the properties in the PersonBasicStructureType object
                    oioPerson = new PersonBasicStructureType();
                    #region Setting Basic Information
                    if (person.DeathDate.HasValue)
                    {
                        oioPerson.PersonDeathDateStructure = new PersonDeathDateStructureType()
                        {
                            PersonDeathDate = person.DeathDate.GetValueOrDefault()
                        };
                        if (person.DeathDateUncertaintyIndicator.HasValue)
                        {
                            oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator = person.DeathDateUncertaintyIndicator.Value;
                        }
                    }

                    oioPerson.RegularCPRPerson = new RegularCPRPersonType()
                    {
                        SimpleCPRPerson = person.ToSimpleCPRPerson(),
                        PersonNameForAddressingName = person.NameForAddressing
                    };

                    if (person.BirthDate.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonBirthDateStructure = new PersonBirthDateStructureType()
                        {
                            BirthDate = person.BirthDate.Value
                        };
                        if (person.BirthDateUncertaintyIndicator.HasValue)
                        {
                            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator = person.BirthDateUncertaintyIndicator.Value;
                        }
                    }

                    if (person.PersonStatusTypeCode != null)
                    {
                        oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure = new PersonCivilRegistrationStatusStructureType()
                        {
                            PersonCivilRegistrationStatusCode = PersonStatusType.GetEnum(person.PersonStatusTypeCode)
                        };
                        if (person.PersonStatusDate.HasValue)
                        {
                            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate = person.PersonStatusDate.Value;
                        }
                    }

                    if (person.GenderId.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonGenderCode = Gender.GetEnum(person.GenderId.Value);
                    }

                    if (person.PersonProtectionIndicator.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator = person.PersonProtectionIndicator.Value;
                    }

                    if (person.MaritalStatusTypeId.HasValue)
                    {
                        oioPerson.MaritalStatusCode = MaritalStatusType.GetEnum(person.MaritalStatusTypeId.Value);
                    }

                    oioPerson.PersonNationalityCode = person.NationalityCountryAlpha2Code;

                    #endregion

                    oioPerson.Item = GetAddress(person.Address);
                }
                qualityLevel = QualityLevel.LocalCache;
                return oioPerson;
            }
        }

        public PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = null;
            PersonFullStructureType oioPerson = null;
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var person = GetPersonFromDatabase(cprNumber, DetailLevel.DetailLevelType.FullData, context);

                if (person != null)
                {
                    oioPerson = new PersonFullStructureType();

                    #region Set basic information
                    if (person.DeathDate.HasValue)
                    {
                        oioPerson.PersonDeathDateStructure = new PersonDeathDateStructureType()
                        {
                            PersonDeathDate = person.DeathDate.Value
                        };
                        if (person.DeathDateUncertaintyIndicator.HasValue)
                        {
                            oioPerson.PersonDeathDateStructure.PersonDeathDateUncertaintyIndicator = person.DeathDateUncertaintyIndicator.Value;
                        }
                    }

                    oioPerson.RegularCPRPerson = new RegularCPRPersonType()
                    {
                        SimpleCPRPerson = person.ToSimpleCPRPerson(),
                        PersonNameForAddressingName = person.NameForAddressing,
                        PersonInformationProtectionIndicator = person.PersonProtectionIndicator.HasValue ? person.PersonProtectionIndicator.Value : false,
                    };

                    if (person.BirthDate.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonBirthDateStructure = new PersonBirthDateStructureType()
                        {
                            BirthDate = person.BirthDate.Value
                        };
                        if (person.BirthDateUncertaintyIndicator.HasValue)
                        {
                            oioPerson.RegularCPRPerson.PersonBirthDateStructure.BirthDateUncertaintyIndicator = person.BirthDateUncertaintyIndicator.Value;
                        }
                    }

                    if (person.PersonStatusTypeCode != null)
                    {
                        oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure = new PersonCivilRegistrationStatusStructureType()
                        {
                            PersonCivilRegistrationStatusCode = PersonStatusType.GetEnum(person.PersonStatusTypeCode)
                        };
                        if (person.PersonStatusDate.HasValue)
                        {
                            oioPerson.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate = person.PersonStatusDate.Value;
                        }
                    }
                    if (person.GenderId.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonGenderCode = Gender.GetEnum(person.GenderId.Value);
                    }
                    if (person.PersonProtectionIndicator.HasValue)
                    {
                        oioPerson.RegularCPRPerson.PersonInformationProtectionIndicator = person.PersonProtectionIndicator.Value;
                    }

                    if (person.MaritalStatusTypeId.HasValue)
                    {
                        oioPerson.MaritalStatusCode = MaritalStatusType.GetEnum(person.MaritalStatusTypeId.Value);
                    }
                    oioPerson.PersonNationalityCode = person.NationalityCountryAlpha2Code;

                    #endregion

                    oioPerson.Item = GetAddress(person.Address);

                    #region relations
                    int numOfChildren = 0;

                    Array.ForEach(person.Relationships.ToArray(), new Action<Relationship>(delegate(Relationship relationShip)
                        {
                            if (relationShip.RelationshipTypeId == (int)RelationshipType.RelationshipTypes.ParentChild) // Is child
                            {
                                numOfChildren++;
                            }
                            if (relationShip.RelationshipTypeId == 3) // Get spouse name
                            {
                                oioPerson.SpouseName = relationShip.RelatedPerson.FirstName + " " + relationShip.RelatedPerson.MiddleName + " " + relationShip.RelatedPerson.LastName;
                            }
                        }));
                    oioPerson.NumberOfChildren = (from rel in person.Relationships where rel.RelationshipTypeId == (int)RelationshipType.RelationshipTypes.ParentChild select rel).Count();
                    var spouse =
                        (
                            (from rel in person.Relationships where rel.RelationshipTypeId == (int)RelationshipType.RelationshipTypes.Marital select rel.RelatedPerson)
                            .Union(from rel in person.Relationships1 where rel.RelationshipTypeId == (int)RelationshipType.RelationshipTypes.Marital select rel.Person)
                         ).SingleOrDefault();

                    if (spouse != null)
                    {
                        oioPerson.SpouseName = spouse.ToSimpleCPRPerson().PersonNameStructure.ToString();
                    }
                    #endregion
                }
                qualityLevel = QualityLevel.LocalCache;
                return oioPerson;
            }
        }

        #endregion

        #region IPersonRelationsDataProvider Members
        public PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            qualityLevel = null;
            PersonRelationsType relations = null;

            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                // Set load options
                DataLoadOptions loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Relationship>((rel) => rel.Person);
                loadOptions.LoadWith<Relationship>((rel) => rel.RelatedPerson);
                loadOptions.LoadWith<Relationship>((rel) => rel.ChildRelationship);
                loadOptions.LoadWith<Relationship>((rel) => rel.TimedRelationship);
                loadOptions.LoadWith<TimedRelationship>((rel) => rel.CustodyRelationship);
                loadOptions.LoadWith<TimedRelationship>((rel) => rel.MaritalRelationship);

                context.LoadOptions = loadOptions;

                var person = GetPersonFromDatabase(cprNumber, DetailLevel.DetailLevelType.Name, context);
                if (person != null)
                {
                    // Fill relation objects                
                    relations = new PersonRelationsType();
                    relations.SimpleCPRPerson = person.ToSimpleCPRPerson();

                    relations.Children.AddRange(Relationship.GetRelationships<ChildRelationshipType>
                        (context, cprNumber, RelationshipType.RelationshipTypes.ParentChild, true, null, null).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));
                    relations.Parents.AddRange(Relationship.GetRelationships<ParentRelationshipType>
                        (context, cprNumber, RelationshipType.RelationshipTypes.ParentChild, false, null, null).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));

                    relations.CustodiedChildren.AddRange(Relationship.GetTimedRelationships<ChildCustodyRelationshipType>
                        (context, cprNumber, RelationshipType.RelationshipTypes.Custody, true, null, null));
                    relations.AuthoritativeParents.AddRange(Relationship.GetTimedRelationships<ParentAuthorityRelationshipType>
                        (context, cprNumber, RelationshipType.RelationshipTypes.Custody, false, null, null));

                    relations.Spouses.AddRange(Relationship.GetTimedRelationships<MaritalRelationshipType>
                        (context, cprNumber, RelationshipType.RelationshipTypes.Marital, null, null, null));

                    qualityLevel = QualityLevel.LocalCache;
                }
                return relations;
            }
        }

        #endregion

        #region IPersonChildrenDataProvider

        public SimpleCPRPersonType[] GetCitizenChildren(string userToken, string appToken, string cprNumber, bool includeCustodies, out QualityLevel? qualityLevel)
        {
            qualityLevel = QualityLevel.LocalCache;
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var relations = Relationship.GetRelationships<ChildRelationshipType>
                    (context, cprNumber, RelationshipType.RelationshipTypes.ParentChild, true, null, null);

                if (relations != null)
                {
                    List<SimpleCPRPersonType> ret = new List<SimpleCPRPersonType>();
                    ret.AddRange((from rel in relations select rel.SimpleCPRPerson));
                    if (includeCustodies)
                    {
                        var custodyRelations = Relationship.GetRelationships<ChildCustodyRelationshipType>
                            (context, cprNumber, RelationshipType.RelationshipTypes.Custody, true, null, null);
                        ret.AddRange(from rel in custodyRelations select rel.SimpleCPRPerson);
                    }
                    return ret.OrderBy((p) => DAL.Person.PersonNumberToDate(p.PersonCivilRegistrationIdentifier)).ToArray();
                }
                return null;
            }
        }
        #endregion

        #region IPersonCustodyDataProvider Members

        public bool RemoveParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                var oioCustodiedChildren = new SimpleCPRPersonType[]
            {
                new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier=cprChildNumber,
                    PersonNameStructure=null
                }
            };

                Person person = Person.GetPerson(context, cprNumber, null, false);
                person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Number);
                var ret = Relationship.EndPersonRelationship(
                    person,
                    RelationshipType.RelationshipTypes.Custody,
                    true,
                    oioCustodiedChildren
                    );
                if (ret)
                {
                    context.SubmitChanges();
                    return true;
                }
                return ret;
            }
        }

        public bool SetParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                // Create OIO relationship objects
                var oioCustodyRelationships = new ChildCustodyRelationshipType[]
                {
                    new ChildCustodyRelationshipType()
                    {
                        SimpleCPRPerson = new SimpleCPRPersonType()
                        {
                            PersonCivilRegistrationIdentifier=cprChildNumber,
                            PersonNameStructure=null
                        },
                        RelationStartDate = DateTime.Today,
                        RelationEndDate=null
                    }
                };
                // Now add the relationships
                Person person = Person.GetPerson(context, cprNumber, null, false);
                person.UpgradeDetailLevel(DetailLevel.DetailLevelType.Number);
                Relationship.AddPersonRelationshipIfNotExist(
                    context,
                    person,
                    RelationshipType.RelationshipTypes.Custody,
                    true,
                    oioCustodyRelationships,
                    (oioRel, dbRel) =>
                    {
                        dbRel.TimedRelationship = new TimedRelationship()
                        {
                            StartDate = oioRel.RelationStartDate,
                            EndDate = oioRel.RelationEndDate,
                            CustodyRelationship = new CustodyRelationship()
                        };
                    }
                    );
                context.SubmitChanges();
                return true;
            }
        }

        public ParentAuthorityRelationshipType[] GetParentAuthorityOverChildChanges(string userToken, string appToken, string cprChildNumber)
        {
            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                return Relationship.GetRelationships<ParentAuthorityRelationshipType>
                    (context, cprChildNumber, RelationshipType.RelationshipTypes.Custody, false, null, null);
            }
        }

        #endregion

        #region ITestCitizenManager Members

        public bool CreateTestCitizen(string userToken, string appToken, PersonFullStructureType oioPerson)
        {
            Engine.Local.UpdateDatabase.UpdateCitizenFull(oioPerson, true);
            return true;
        }

        #endregion
    }
}
