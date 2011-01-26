using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.DAL.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Util;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IPartReadDataProvider, IPartSearchDataProvider, IPartPersonMappingDataProvider
    {

        #region IPartSearchDataProvider Members

        public Guid[] Search(CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            Guid[] ret = null;
            using (var dataContext = new PartDataContext())
            {
                var pred = PredicateBuilder.True<DAL.Part.PersonRegistration>();
                if (searchCriteria.Soeg != null)
                {
                    // Search by cpr number
                    if (!string.IsNullOrEmpty(searchCriteria.Soeg.BrugervendtNoegleTekst))
                    {
                        pred = pred.And(pr => pr.Person.UserInterfaceKeyText == searchCriteria.Soeg.BrugervendtNoegleTekst);
                    }
                    if (searchCriteria.Soeg.Attributter != null)
                    {
                        if (searchCriteria.Soeg.Attributter.SoegEgenskab != null)
                        {
                            foreach (var prop in searchCriteria.Soeg.Attributter.SoegEgenskab)
                            {
                                // Search by name
                                var name = prop.PersonNameStructure;
                                if (!name.IsEmpty)
                                {
                                    var cprNamePred = PredicateBuilder.True<DAL.Part.PersonRegistration>();
                                    cprNamePred = cprNamePred.And((pr) => pr.PersonAttribute.CprData != null);
                                    if (!string.IsNullOrEmpty(name.PersonGivenName))
                                    {
                                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.FirstName == name.PersonGivenName);
                                    }
                                    if (!string.IsNullOrEmpty(name.PersonMiddleName))
                                    {
                                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.MiddleName == name.PersonMiddleName);
                                    }
                                    if (!string.IsNullOrEmpty(name.PersonSurnameName))
                                    {
                                        cprNamePred = cprNamePred.And((pt) => pt.PersonAttribute.CprData.LastName == name.PersonSurnameName);
                                    }
                                    pred = pred.And(cprNamePred);
                                }
                            }
                        }
                    }

                    /*if (searchCriteria.BirthDateFrom.HasValue)
                    {
                        pred = pred.And((pt) => pt.PersonAttribute.BirthDate >= searchCriteria.BirthDateFrom.Value);
                    }
                    if (searchCriteria.BirthDateTo.HasValue)
                    {
                        pred = pred.And((pt) => pt.PersonAttribute.BirthDate <= searchCriteria.BirthDateTo.Value);
                    }*/

                    /*if (!string.IsNullOrEmpty(searchCriteria.CprNumber))
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
                    }*/

                    /*if (searchCriteria.Gender.HasValue)
                    {
                        pred = pred.And((pt) => pt.PersonAttribute.GenderId == (int)searchCriteria.Gender);
                    }*/


                    /*if (!string.IsNullOrEmpty(searchCriteria.NationalityCountryCode))
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
                    }*/
                }
                // TODO: Query Person table to avoid Distinct operation
                var result = (from pr in dataContext.PersonRegistrations.Where(pred)
                              select pr.UUID)
                              .Distinct();


                int firstResults = 0;
                if (int.TryParse(searchCriteria.FoersteResultat, out firstResults))
                {
                    result = result.Skip(firstResults);
                }

                int maxResults = 0;
                if (int.TryParse(searchCriteria.MaksimalAntalResultater, out maxResults))
                {
                    result = result.Take(maxResults);
                }
                ret = result.ToArray();
            }
            // TODO: filter by effect date            
            return ret;
        }

        #endregion

        #region IPartReadDataProvider Members

        public CprBroker.Schemas.Part.RegistreringType1 Read(PersonIdentifier uuid, CprBroker.Schemas.Part.LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            Schemas.Part.RegistreringType1 ret = null;
            var fromRegistrationDate = TidspunktType.ToDateTime(input.RegistreringFraFilter);
            var toRegistrationDate = TidspunktType.ToDateTime(input.RegistreringTilFilter);

            var fromEffectDate = TidspunktType.ToDateTime(input.VirkningFraFilter);
            var ToEffectDate = TidspunktType.ToDateTime(input.VirkningTilFilter);

            using (var dataContext = new PartDataContext())
            {
                DAL.Part.PersonRegistration.SetChildLoadOptions(dataContext);
                ret =
                (
                    from personReg in dataContext.PersonRegistrations
                    where personReg.UUID == uuid.UUID
                        // Filter by registration date
                    && (!fromRegistrationDate.HasValue || personReg.RegistrationDate >= fromRegistrationDate)
                    && (!toRegistrationDate.HasValue || personReg.RegistrationDate <= toRegistrationDate)
                    // TODO: Filter by effect date
                    orderby personReg.RegistrationDate descending
                    orderby personReg.BrokerUpdateDate descending
                    select personReg.ToXmlType()
                ).FirstOrDefault();
            }
            ql = QualityLevel.LocalCache;
            return ret;
        }
        #endregion

        #region IPartPersonMappingDataProvider Members
        // TODO: Move this method to a separate data provider
        public Guid GetPersonUuid(string cprNumber)
        {
            PersonIdentifier[] identifiers = new PersonIdentifier[] { new PersonIdentifier() { CprNumber = cprNumber } };
            DAL.Part.PersonMapping.AssignGuids(identifiers);
            return identifiers[0].UUID.Value;
        }

        #endregion


    }
}
