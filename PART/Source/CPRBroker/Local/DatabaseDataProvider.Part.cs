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
                if (searchCriteria.SoegObjekt != null)
                {
                    if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.UUID))
                    {
                        var personUuid=new Guid(searchCriteria.SoegObjekt.UUID);
                        pred = pred.And(p => p.UUID == personUuid);
                    }
                    // Search by cpr number
                    if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.BrugervendtNoegleTekst))
                    {
                        pred = pred.And(pr => pr.Person.UserInterfaceKeyText == searchCriteria.SoegObjekt.BrugervendtNoegleTekst);
                    }
                    if (searchCriteria.SoegObjekt.SoegAttributListe != null)
                    {
                        if (searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab != null)
                        {
                            foreach (var prop in searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab)
                            {
                                if (prop!=null && prop.NavnStruktur != null)
                                {
                                    if (prop.NavnStruktur.PersonNameStructure != null)
                                    {
                                        // Search by name
                                        var name = prop.NavnStruktur.PersonNameStructure;
                                        if (!name.IsEmpty)
                                        {
                                            var cprNamePred = PredicateBuilder.True<DAL.Part.PersonRegistration>();                                            
                                            if (!string.IsNullOrEmpty(name.PersonGivenName))
                                            {
                                                cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.PersonProperties.PersonName.FirstName == name.PersonGivenName);
                                            }
                                            if (!string.IsNullOrEmpty(name.PersonMiddleName))
                                            {
                                                cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.PersonProperties.PersonName.MiddleName == name.PersonMiddleName);
                                            }
                                            if (!string.IsNullOrEmpty(name.PersonSurnameName))
                                            {
                                                cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.PersonProperties.PersonName.LastName == name.PersonSurnameName);
                                            }
                                            pred = pred.And(cprNamePred);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // TODO: Query Person table to avoid Distinct operation
                var result = (from pr in dataContext.PersonRegistrations.Where(pred)
                              select pr.UUID)
                              .Distinct();


                int firstResults = 0;
                if (int.TryParse(searchCriteria.FoersteResultatReference, out firstResults))
                {
                    result = result.Skip(firstResults);
                }

                int maxResults = 0;
                int.TryParse(searchCriteria.MaksimalAntalKvantitet, out maxResults);
                if (maxResults <= 0)
                {
                    maxResults = 1000;
                }
                result = result.Take(maxResults);
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
                    select DAL.Part.PersonRegistration.ToXmlType(personReg)
                ).FirstOrDefault();
            }
            ql = QualityLevel.LocalCache;
            return ret;
        }
        #endregion

        #region IPartPersonMappingDataProvider Members
        
        public Guid? GetPersonUuid(string cprNumber)
        {
            return DAL.Part.PersonMapping.AssignGuids(new string[] { cprNumber })[0];
        }

        #endregion


    }
}
