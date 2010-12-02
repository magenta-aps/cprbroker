using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.DAL;
using CPRBroker.DAL.Part;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Util;
using CPRBroker.Engine.Local;

namespace CPRBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IPartReadDataProvider, IPartSearchDataProvider
    {

        #region IPartSearchDataProvider Members

        public PersonIdentifier[] Search(CPRBroker.Schemas.Part.PersonSearchCriteria searchCriteria, DateTime? effectDate, out QualityLevel? ql)
        {
            PersonIdentifier[] ret = null;
            using (var dataContext = new PartDataContext())
            {
                var pred = PredicateBuilder.True<PersonRegistration>();
                if (searchCriteria.BirthDate.HasValue)
                {
                    pred = pred.And((pt) => pt.PersonAttribute.BirthDate == searchCriteria.BirthDate.Value);
                }
                if (!string.IsNullOrEmpty(searchCriteria.CprNumber))
                {
                    var cprPred = PredicateBuilder.True<PersonRegistration>();
                    cprPred = cprPred.And((pr) => pr.PersonAttribute.CprData != null);
                    cprPred = cprPred.And((pr) => pr.PersonAttribute.CprData.CprNumber == searchCriteria.CprNumber);

                    var foreignPred = PredicateBuilder.True<PersonRegistration>();
                    foreignPred = cprPred.And((pr) => pr.PersonAttribute.ForeignCitizenData != null);
                    foreignPred = cprPred.And((pr) => pr.PersonAttribute.ForeignCitizenData.ForeignNumber == searchCriteria.CprNumber);

                    var compPred = PredicateBuilder.False<PersonRegistration>();
                    compPred = compPred.Or(cprPred);
                    compPred = compPred.Or(foreignPred);

                    pred = pred.And(compPred);
                }

                if (searchCriteria.Gender.HasValue)
                {
                    pred = pred.And((pt) => pt.PersonAttribute.GenderId == (int)searchCriteria.Gender);
                }

                if (!searchCriteria.Name.IsEmpty)
                {
                    var simpleNamePred = PredicateBuilder.True<PersonRegistration>();
                    simpleNamePred = simpleNamePred.And((pr) => pr.PersonAttribute.CprData == null);
                    simpleNamePred = simpleNamePred.And((pr) => pr.PersonAttribute.Name == searchCriteria.Name.ToString());

                    var cprNamePred = PredicateBuilder.True<PersonRegistration>();
                    cprNamePred = cprNamePred.And((pr) => pr.PersonAttribute.CprData != null);
                    if (!string.IsNullOrEmpty(searchCriteria.Name.PersonGivenName))
                    {
                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.FirstName == searchCriteria.Name.PersonGivenName);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.Name.PersonMiddleName))
                    {
                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.MiddleName == searchCriteria.Name.PersonMiddleName);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.Name.PersonSurnameName))
                    {
                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.LastName == searchCriteria.Name.PersonSurnameName);
                    }

                    var fullNamePred = PredicateBuilder.False<PersonRegistration>();
                    fullNamePred = fullNamePred.Or(simpleNamePred);
                    fullNamePred = fullNamePred.Or(cprNamePred);

                    pred = pred.And(fullNamePred);
                };

                if (!string.IsNullOrEmpty(searchCriteria.NationalityCountryCode))
                {
                    var cprNationality = PredicateBuilder.True<PersonRegistration>();
                    cprNationality = cprNationality.And((pr) => pr.PersonAttribute.CprData != null);
                    cprNationality = cprNationality.And((pr) => pr.PersonAttribute.CprData.NationalityCountryAlpha2Code == searchCriteria.NationalityCountryCode);

                    var foreignNationality = PredicateBuilder.True<PersonRegistration>();
                    foreignNationality = foreignNationality.And((pr) => pr.PersonAttribute.ForeignCitizenData != null);
                    foreignNationality = foreignNationality.And((pr) => pr.PersonAttribute.ForeignCitizenData.NationalityCountryAlpha2Code == searchCriteria.NationalityCountryCode);

                    var nationalityPred = PredicateBuilder.False<PersonRegistration>();
                    nationalityPred = nationalityPred.Or(cprNationality);
                    nationalityPred = nationalityPred.Or(foreignNationality);

                    pred = pred.And(nationalityPred);
                }

                var result = from pr in dataContext.PersonRegistrations.Where(pred)
                             select new PersonIdentifier() { UUID = pr.UUID };
                ret = result.ToArray();
            }
            // TODO: filter by effect date
            ql = QualityLevel.LocalCache;
            return ret;
        }

        #endregion

        #region IPartReadDataProvider Members

        public CPRBroker.Schemas.Part.PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql)
        {
            Schemas.Part.PersonRegistration ret = null;
            using (var dataContext = new PartDataContext())
            {
                ret =
                (
                    from personReg in dataContext.PersonRegistrations
                    where personReg.UUID == uuid.UUID
                    // TODO: add effect date to where condition
                    orderby personReg.RegistrationDate descending
                    select personReg.ToXmlType()
                ).FirstOrDefault();
            }
            ql = QualityLevel.LocalCache;
            return ret;
        }

        public CPRBroker.Schemas.Part.PersonRegistration[] List(PersonIdentifier[] uuids, DateTime? effectDate, out QualityLevel? ql)
        {
            // TODO: implement List after Read
            throw new NotImplementedException();
        }

        #endregion
    }
}
