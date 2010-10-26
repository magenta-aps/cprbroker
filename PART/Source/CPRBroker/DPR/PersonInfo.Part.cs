using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;

namespace CPRBroker.Providers.DPR
{
    /// <summary>
    /// Used as a link between miscellaneous person tables & person information
    /// </summary>
    internal partial class PersonInfo
    {
        internal PersonRegistration ToPersonRegistration()
        {
            PersonNameStructureType tempPersonName = new PersonNameStructureType(PersonName.FirstName, PersonName.LastName);
            PersonRegistration ret = new PersonRegistration()
            {
                Attributes = new PersonAttributes()
                {
                    BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value,
                    ContactAddresses = new Effect<Address>[0],
                    ContactChannel = new Effect<ContactChannel>[0],
                    Gender = Utilities.GenderFromChar(PersonTotal.Sex),
                    Name = new Effect<string>()
                    {
                        StartDate = Utilities.DateFromDecimal(PersonName.NameStartDate),
                        EndDate = Utilities.DateFromDecimal(PersonName.NameTerminationDate),
                        Value = tempPersonName.ToString(),
                    },
                    PersonData = new CprData()
                    {
                        PersonName = new Effect<PersonNameStructureType>()
                        {
                            StartDate = Utilities.DateFromDecimal(PersonName.NameStartDate),
                            EndDate = Utilities.DateFromDecimal(PersonName.NameTerminationDate),
                            Value = tempPersonName
                        },
                        AddressingName = PersonName.AddressingName,
                        BirthDateUncertainty = null,

                        CprNumber = Convert.ToInt64(PersonName.PNR).ToString("D10"),

                        Gender = Utilities.GenderFromChar(PersonTotal.Sex),
                        //TODO: correct this field
                        IndividualTrackStatus = true,

                        NameAndAddressProtection = HasProtection,
                        NationalityCountryCode = DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality),
                        NickName = null,
                        // TODO correct the composition of the address
                        PopulationAddress = new AddressDenmark()
                        {
                            AddressComplete = new DanishAddressStructureType(),
                            //...
                            //...
                        },
                    },
                },

                RegistrationDate = DateTime.Today,
                Relations = new PersonRelations()
                {
                    Children = new Effect<PersonRelation>[0],
                    Parents = new Effect<PersonRelation>[0],
                    ReplacedBy = null,
                    Spouses = new Effect<PersonRelation>[0],
                    SubstituteFor = new Effect<PersonRelation>[0],
                },
                States = new PersonStates()
                {
                    CivilStatus = new Effect<CPRBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = null,
                        EndDate = null,
                        // TODO correct this field
                        Value =  CPRBroker.Schemas.Part.Enums.MaritalStatus.single,
                    },
                    LifeStatus = new Effect<CPRBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        StartDate = null,
                        EndDate = null,
                        // TODO correct this field
                        Value = CPRBroker.Schemas.Part.Enums.LifeStatus.born,
                    }
                }
            };

            return ret;
        }
    }
}
