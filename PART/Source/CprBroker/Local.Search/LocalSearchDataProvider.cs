using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Providers.Local.Search
{
    public class LocalSearchDataProvider : IPartSearchDataProvider
    {

        public Guid[] Search(CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            using (var dataContext = new PartSearchDataContext())
            {
                int firstResults = 0;
                int.TryParse(searchCriteria.FoersteResultatReference, out firstResults);

                int maxResults = 0;
                int.TryParse(searchCriteria.MaksimalAntalKvantitet, out maxResults);
                if (maxResults <= 0)
                {
                    maxResults = 1000;
                }

                var expr = CreateWhereExpression(dataContext, searchCriteria);
                return dataContext
                    .PersonSearchCaches
                    .Where(expr)
                    .OrderBy(psc => psc.UUID)
                    .Select(psc => psc.UUID)
                    .Skip(firstResults)
                    .Take(maxResults)
                    .ToArray();
            }
        }

        public static Expression<Func<PersonSearchCache, bool>> CreateWhereExpression(PartSearchDataContext dataContext, CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            var pred = PredicateBuilder.True<PersonSearchCache>();
            if (searchCriteria.SoegObjekt != null)
            {
                if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.UUID))
                {
                    var personUuid = new Guid(searchCriteria.SoegObjekt.UUID);
                    pred = pred.And(p => p.UUID == personUuid);
                }

                // Lifecycle status
                if (searchCriteria.SoegObjekt.SoegRegistrering != null)
                {
                    if (searchCriteria.SoegObjekt.SoegRegistrering.LivscyklusKodeSpecified)
                    {
                        pred = pred.And(p => p.LivscyklusKode == searchCriteria.SoegObjekt.SoegRegistrering.LivscyklusKode.ToString());
                    }
                }

                // Search by cpr number
                if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.BrugervendtNoegleTekst))
                {
                    // TODO: In theory, this is the same as RegisterOplysning/CprBorger/UserInterfaceKeyText
                    pred = pred.And(pr => pr.UserInterfaceKeyText == searchCriteria.SoegObjekt.BrugervendtNoegleTekst);
                }

                // Attributes
                if (searchCriteria.SoegObjekt.SoegAttributListe != null)
                {
                    if (searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab != null)
                    {
                        foreach (var prop in searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab)
                        {
                            if (prop != null)
                            {
                                if (!string.IsNullOrEmpty(prop.FoedestedNavn))
                                {
                                    pred = pred.And((pt) => pt.FoedestedNavn == prop.FoedestedNavn);
                                }
                                if (!string.IsNullOrEmpty(prop.FoedselsregistreringMyndighedNavn))
                                {
                                    pred = pred.And((pt) => pt.FoedselsregistreringMyndighedNavn == prop.FoedselsregistreringMyndighedNavn);
                                }

                                if (prop.NavnStruktur != null)
                                {
                                    if (!string.IsNullOrEmpty(prop.NavnStruktur.PersonNameForAddressingName))
                                    {
                                        pred = pred.And((pt) => pt.AddressingName == prop.NavnStruktur.PersonNameForAddressingName);
                                    }
                                    if (!string.IsNullOrEmpty(prop.NavnStruktur.KaldenavnTekst))
                                    {
                                        pred = pred.And((pt) => pt.NickName == prop.NavnStruktur.KaldenavnTekst);
                                    }
                                    if (!string.IsNullOrEmpty(prop.NavnStruktur.NoteTekst))
                                    {
                                        pred = pred.And((pt) => pt.Note == prop.NavnStruktur.NoteTekst);
                                    }
                                    if (prop.NavnStruktur.PersonNameStructure != null)
                                    {
                                        // Search by name
                                        var name = prop.NavnStruktur.PersonNameStructure;
                                        if (!name.IsEmpty)
                                        {
                                            if (!string.IsNullOrEmpty(name.PersonGivenName))
                                            {
                                                pred = pred.And((pt) => pt.PersonGivenName == name.PersonGivenName);
                                            }
                                            if (!string.IsNullOrEmpty(name.PersonMiddleName))
                                            {
                                                pred = pred.And((pt) => pt.PersonMiddleName == name.PersonMiddleName);
                                            }
                                            if (!string.IsNullOrEmpty(name.PersonSurnameName))
                                            {
                                                pred = pred.And((pt) => pt.PersonSurnameName == name.PersonSurnameName);
                                            }
                                        }
                                    }
                                }
                                if (prop.PersonGenderCodeSpecified)
                                {
                                    pred = pred.And((pt) => pt.PersonGenderCode == prop.PersonGenderCode.ToString());
                                }
                                if (prop.BirthDateSpecified)
                                {
                                    // TODO: Check formatting of dates, could be different between webserver and database
                                    pred = pred.And((pt) => pt.Birthdate == prop.BirthDate.ToShortDateString());
                                }
                            }
                        }

                        foreach (var prop in searchCriteria.SoegObjekt.SoegAttributListe.SoegRegisterOplysning)
                        {
                            // TODO: What about other Item values?
                            if (prop != null && prop.Item is CprBroker.Schemas.Part.CprBorgerType)
                            {
                                // CprBorger fields
                                // --------------------
                                var cprBorger = prop.Item as CprBroker.Schemas.Part.CprBorgerType;
                                if (!string.IsNullOrEmpty(cprBorger.PersonCivilRegistrationIdentifier))
                                {
                                    pred = pred.And(pt => pt.PersonCivilRegistrationIdentifier == cprBorger.PersonCivilRegistrationIdentifier);
                                }
                                if (cprBorger.PersonNummerGyldighedStatusIndikator)// TODO: Check null values for boolean
                                {
                                    pred = pred.And(pt => pt.PersonNummerGyldighedStatusIndikator == cprBorger.PersonNummerGyldighedStatusIndikator.ToString());
                                }
                                if (cprBorger.PersonNationalityCode != null && !string.IsNullOrEmpty(cprBorger.PersonNationalityCode.Value))
                                {
                                    pred = pred.And(pt => pt.PersonNationalityCode == cprBorger.PersonNationalityCode.Value);
                                }
                                if (cprBorger.NavneAdresseBeskyttelseIndikator)// TODO: Check null values for boolean
                                {
                                    pred = pred.And(pt => pt.NavneAdresseBeskyttelseIndikator == cprBorger.NavneAdresseBeskyttelseIndikator.ToString());
                                }
                                if (cprBorger.TelefonNummerBeskyttelseIndikator)// TODO: Check null values for boolean
                                {
                                    pred = pred.And(pt => pt.TelefonNummerBeskyttelseIndikator == cprBorger.TelefonNummerBeskyttelseIndikator.ToString());
                                }
                                // TODO: Check null values for boolean
                                if (cprBorger.ForskerBeskyttelseIndikator)
                                {
                                    pred = pred.And(pt => pt.ForskerBeskyttelseIndikator == cprBorger.ForskerBeskyttelseIndikator.ToString());
                                }

                                // CprBorger fields - After address
                                // --------------------------------
                                if (!string.IsNullOrEmpty(cprBorger.AdresseNoteTekst))
                                {
                                    pred = pred.And(pt => pt.AdresseNoteTekst == cprBorger.AdresseNoteTekst);
                                }
                                if (cprBorger.FolkekirkeMedlemIndikator)// TODO: Check null values for boolean
                                {
                                    pred = pred.And(pt => pt.FolkekirkeMedlemIndikator == cprBorger.FolkekirkeMedlemIndikator.ToString());
                                }

                                //  FolkeregisterAdresse fields
                                // ----------------------------
                                if (cprBorger.FolkeregisterAdresse != null && cprBorger.FolkeregisterAdresse.Item is DanskAdresseType)
                                {
                                    var danskAddress = cprBorger.FolkeregisterAdresse.Item as DanskAdresseType;
                                    if (!string.IsNullOrEmpty(danskAddress.NoteTekst))
                                    {
                                        pred = pred.And(pt => pt.NoteTekst_DanskAdresse == danskAddress.NoteTekst);
                                    }
                                    if (danskAddress.UkendtAdresseIndikator)// TODO: Check null values for boolean
                                    {
                                        pred = pred.And(pt => pt.UkendtAdresseIndikator == danskAddress.UkendtAdresseIndikator.ToString());
                                    }
                                    if (danskAddress.SpecielVejkodeIndikatorSpecified)
                                    {
                                        pred = pred.And(pt => pt.SpecielVejkodeIndikator == danskAddress.SpecielVejkodeIndikator.ToString());
                                    }
                                    if (!string.IsNullOrEmpty(danskAddress.PostDistriktTekst))
                                    {
                                        pred = pred.And(pt => pt.PostDistriktTekst == danskAddress.PostDistriktTekst);
                                    }

                                    if (danskAddress.AddressComplete != null)
                                    {
                                        if (danskAddress.AddressComplete.AddressAccess != null)
                                        {
                                            var access = danskAddress.AddressComplete.AddressAccess;
                                            if (!string.IsNullOrEmpty(access.MunicipalityCode))
                                            {
                                                pred = pred.And(pt => pt.MunicipalityCode == access.MunicipalityCode);
                                            }
                                            if (!string.IsNullOrEmpty(access.StreetCode))
                                            {
                                                pred = pred.And(pt => pt.StreetCode == access.StreetCode);
                                            }
                                            access.StreetBuildingIdentifier = PartInterface.Strings.TrimAddressString(access.StreetBuildingIdentifier);
                                            if (!string.IsNullOrEmpty(access.StreetBuildingIdentifier))
                                            {
                                                pred = pred.And(pt => pt.StreetBuildingIdentifier == access.StreetBuildingIdentifier);
                                            }
                                        }
                                        if (danskAddress.AddressComplete.AddressPostal != null)
                                        {
                                            var postal = danskAddress.AddressComplete.AddressPostal;
                                            if (!string.IsNullOrEmpty(postal.MailDeliverySublocationIdentifier))
                                            {
                                                pred = pred.And(pt => pt.MailDeliverySublocationIdentifier == postal.MailDeliverySublocationIdentifier);
                                            }
                                            if (!string.IsNullOrEmpty(postal.StreetName))
                                            {
                                                pred = pred.And(pt => pt.StreetName == postal.StreetName);
                                            }
                                            if (!string.IsNullOrEmpty(postal.StreetNameForAddressingName))
                                            {
                                                pred = pred.And(pt => pt.StreetNameForAddressingName == postal.StreetNameForAddressingName);
                                            }
                                            postal.StreetBuildingIdentifier = PartInterface.Strings.TrimAddressString(postal.StreetBuildingIdentifier);
                                            if (!string.IsNullOrEmpty(postal.StreetBuildingIdentifier))
                                            {
                                                pred = pred.And(pt => pt.StreetBuildingIdentifier_Postal == postal.StreetBuildingIdentifier);
                                            }
                                            postal.FloorIdentifier = PartInterface.Strings.TrimAddressString(postal.FloorIdentifier);
                                            if (!string.IsNullOrEmpty(postal.FloorIdentifier))
                                            {
                                                pred = pred.And(pt => pt.FloorIdentifier == postal.FloorIdentifier);
                                            }
                                            postal.SuiteIdentifier = PartInterface.Strings.TrimAddressString(postal.SuiteIdentifier);
                                            if (!string.IsNullOrEmpty(postal.SuiteIdentifier))
                                            {
                                                pred = pred.And(pt => pt.SuiteIdentifier == postal.SuiteIdentifier);
                                            }

                                            if (!string.IsNullOrEmpty(postal.DistrictSubdivisionIdentifier))
                                            {
                                                pred = pred.And(pt => pt.DistrictSubdivisionIdentifier == postal.DistrictSubdivisionIdentifier);
                                            }
                                            if (!string.IsNullOrEmpty(postal.PostOfficeBoxIdentifier))
                                            {
                                                pred = pred.And(pt => pt.PostOfficeBoxIdentifier == postal.PostOfficeBoxIdentifier);
                                            }

                                            if (!string.IsNullOrEmpty(postal.PostCodeIdentifier))
                                            {
                                                pred = pred.And(pt => pt.PostCodeIdentifier == postal.PostCodeIdentifier);
                                            }
                                            if (!string.IsNullOrEmpty(postal.DistrictName))
                                            {
                                                pred = pred.And(pt => pt.DistrictName == postal.DistrictName);
                                            }

                                            if (postal.CountryIdentificationCode != null && !string.IsNullOrEmpty(postal.CountryIdentificationCode.Value))
                                            {
                                                pred = pred.And(pt => pt.CountryIdentificationCode == postal.CountryIdentificationCode.Value);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return pred;
        }

        public bool IsAlive()
        {
            using (var dataContext = new PartSearchDataContext())
            {
                try
                {
                    var first = dataContext.PersonSearchCaches.FirstOrDefault();
                    return true;
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                    return false;
                }
            }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }
    }
}
