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
        internal static readonly Expression<Func<decimal, decimal, DPRDataContext, PersonTotal>> NextPersonTotalExpression = (pnr, statusDate, dataContext) =>
            (from personTotal in dataContext.PersonTotals
             where pnr == personTotal.PNR && personTotal.StatusDate > statusDate
             orderby personTotal.StatusDate
             select personTotal
            ).FirstOrDefault();

        internal PersonRegistration ToPersonRegistration(DateTime? effectTime, DPRDataContext dataContext)
        {
            var civilRegistrationStatus = Schemas.Util.Enums.ToCivilRegistrationStatus(PersonTotal.Status);
            var effectTimeDecimal = Utilities.DecimalFromDate(effectTime);
            PersonNameStructureType tempPersonName = new PersonNameStructureType(PersonName.FirstName, PersonName.LastName);
            var civilStates = (from civilStatus in dataContext.CivilStatus
                               where civilStatus.PNR == PersonTotal.PNR && civilStatus.MaritalStatusDate <= effectTimeDecimal.Value
                               orderby civilStatus.MaritalStatusDate
                               select civilStatus).ToArray();

            PersonRegistration ret = new PersonRegistration()
            {
                Attributes = new PersonAttributes()
                {
                    BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value,
                    OtherAddresses = new Address[0],
                    ContactChannel = new ContactChannel[0],
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
                        //TODO: find if applicable
                        NickName = null,
                        // TODO: Ensure that ContactAddress is the right object to pass
                        PopulationAddress = PersonTotal.ToPartAddress(civilRegistrationStatus, Street, ContactAddress),
                    },
                },
                //TODO: Fix calculation of registration date in DPR
                RegistrationDate = Utilities.DateFromDecimal(this.PersonName.NameStartDate).Value,
                // TODO: Add relations
                Relations = new PersonRelations()
                {
                    Children = new Effect<PersonRelation>[0],
                    Parents = new PersonRelation[0],
                    ReplacedBy = null,
                    Spouses = new Effect<PersonRelation>[0],
                    SubstituteFor = new Effect<PersonRelation>[0],
                },
                States = new PersonStates()
                {
                    CivilStatus = new Effect<CPRBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = Utilities.DateFromDecimal(PersonTotal.MaritalStatusDate),
                        // Handled later
                        EndDate = null,
                        Value = PersonTotal.PartMaritalStatus,
                    },
                    LifeStatus = new Effect<CPRBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        StartDate = Utilities.DateFromDecimal(PersonTotal.StatusDate),
                        // Handled later using the first PersonTotal that has a later StatusDate
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToLifeStatus(PersonTotal.Status)
                    }
                }
            };

            // Now fill the null EndDate(s)
            if (effectTime.HasValue && effectTime.Value.Date < DateTime.Today)
            {
                if (PersonTotal.StatusDate.HasValue)
                {
                    var nextPersonTotal = NextPersonTotalExpression.Compile()(PersonTotal.PNR, PersonTotal.StatusDate.Value, dataContext);
                    if (nextPersonTotal != null)
                    {
                        ret.States.LifeStatus.EndDate = Utilities.DateFromDecimal(nextPersonTotal.StatusDate);
                    }
                }
                if (PersonTotal.MaritalStatusDate.HasValue)
                {
                    var maritalStatus =
                        (
                            from ms in civilStates
                            where ms.MaritalStatusDate == PersonTotal.MaritalStatusDate
                            select ms
                        ).FirstOrDefault();
                    if (maritalStatus != null)
                    {
                        ret.States.CivilStatus.EndDate = Utilities.DateFromDecimal(maritalStatus.MaritalEndDate);
                    }
                }
            }

            //TODO: Fill the relations in DPR
            /**************/
            /*
            ret.Relations.Parents = Array.ConvertAll<Guid, PersonRelation>
            (
                DAL.Part.PersonMapping.AssignGuids
                (
                    (
                        from pers in Child.PersonParentsExpression.Compile()(PersonTotal.PNR, dataContext)
                        select new PersonIdentifier()
                        {
                            CprNumber = pers.PNR.ToString("D2")
                        }
                    ).ToArray()
                ),
                (id) => new PersonRelation() { TargetUUID = id }
            );
            ret.Relations.Parents = DAL.Part.PersonMapping.AssignGuids<decimal, PersonRelation>(
                 (
                        from pers in Child.PersonParentsExpression.Compile()(PersonTotal.PNR, dataContext)
                        select pers.PNR
                    ).ToArray(),
                    (pnr) => null,
                    (pnr) => new PersonIdentifier() { CprNumber = pnr.ToString("D2") },
                    (dd,id)=>dd.TargetUUID = id);


            ret.Relations.Children = DAL.Part.PersonMapping.AssignGuids<PersonTotal, Effect<PersonRelation>>
            (
                Child.PersonChildrenExpression.Compile()(effectTimeDecimal.Value, PersonTotal.PNR, dataContext).ToArray(),
                (child) => new Effect<PersonRelation>()
                {
                    StartDate = Utilities.DateFromDecimal(child.DateOfBirth),
                    EndDate = null,
                    Value = new PersonRelation()
                },
                (child) => new PersonIdentifier()
                {
                    CprNumber = child.PNR.ToString("D2"),
                },
                (rel, id) => rel.Value.TargetUUID = id
            );

            ret.Relations.Spouses = DAL.Part.PersonMapping.AssignGuids<CivilStatus, Effect<PersonRelation>>
            (
                civilStates,
                (civilStatus) => new Effect<PersonRelation>()
                {
                    StartDate = Utilities.DateFromDecimal(civilStatus.MaritalStatusDate),
                    EndDate = Utilities.DateFromDecimal(civilStatus.MaritalEndDate),
                    Value = new PersonRelation()
                    {
                    }
                },
                (civilStatus) => new PersonIdentifier()
                {
                    CprNumber = civilStatus.SpousePNR.Value.ToString("D2"),
                },
                (p, id) =>
                {
                    p.Value.TargetUUID = id;
                }
            );
            */
            return ret;
        }

    }
}
