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
    public static class LocalSearchExtensions
    {
        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, CprBroker.Schemas.Part.SoegObjektType searchCriteria)
        {
            if (!string.IsNullOrEmpty(searchCriteria.UUID))
            {
                var personUuid = new Guid(searchCriteria.UUID);
                pred = pred.And(p => p.UUID == personUuid);
            }

            // Lifecycle status
            if (searchCriteria.SoegRegistrering != null)
            {
                if (searchCriteria.SoegRegistrering.LivscyklusKodeSpecified)
                {
                    pred = pred.And(p => p.LivscyklusKode == searchCriteria.SoegRegistrering.LivscyklusKode.ToString());
                }
            }

            // Search by cpr number
            if (!string.IsNullOrEmpty(searchCriteria.BrugervendtNoegleTekst))
            {
                // TODO: In theory, this is the same as RegisterOplysning/CprBorger/UserInterfaceKeyText
                pred = pred.And(pr => pr.UserInterfaceKeyText == searchCriteria.BrugervendtNoegleTekst);
            }

            // Attributes
            if (searchCriteria.SoegAttributListe != null)
            {
                if (searchCriteria.SoegAttributListe.SoegEgenskab != null)
                {
                    foreach (var prop in searchCriteria.SoegAttributListe.SoegEgenskab)
                    {
                        if (prop != null)
                        {
                            pred = pred.And(prop);
                        }
                    }

                    foreach (var prop in searchCriteria.SoegAttributListe.SoegRegisterOplysning)
                    {
                        // TODO: What about other Item values?
                        if (prop != null)
                        {
                            if (prop.Item is CprBroker.Schemas.Part.CprBorgerType)
                            {
                                var cprBorger = prop.Item as CprBroker.Schemas.Part.CprBorgerType;
                                pred = pred.And(cprBorger);
                            }
                        }
                    }
                }
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, SoegEgenskabType prop)
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
                pred = pred.And(prop);
            }
            if (prop.PersonGenderCodeSpecified)
            {
                pred = pred.And((pt) => pt.PersonGenderCode == prop.PersonGenderCode.ToString());
            }
            if (prop.BirthDateSpecified)
            {
                pred = pred.And((pt) => pt.Birthdate == prop.BirthDate);
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, NavnStrukturType prop)
        {
            if (!string.IsNullOrEmpty(prop.PersonNameForAddressingName))
            {
                pred = pred.And((pt) => pt.AddressingName == prop.PersonNameForAddressingName);
            }
            if (!string.IsNullOrEmpty(prop.KaldenavnTekst))
            {
                pred = pred.And((pt) => pt.NickName == prop.KaldenavnTekst);
            }
            if (!string.IsNullOrEmpty(prop.NoteTekst))
            {
                pred = pred.And((pt) => pt.Note == prop.NoteTekst);
            }
            if (prop.PersonNameStructure != null)
            {
                // Search by name
                var name = prop.PersonNameStructure;
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
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, CprBorgerType cprBorger)
        {
            // CprBorger fields
            // --------------------

            if (!string.IsNullOrEmpty(cprBorger.PersonCivilRegistrationIdentifier))
            {
                pred = pred.And(pt => pt.PersonCivilRegistrationIdentifier == cprBorger.PersonCivilRegistrationIdentifier);
            }
            if (cprBorger.PersonNummerGyldighedStatusIndikator)
            {
                // False is treated like Not specified
                pred = pred.And(pt => pt.PersonNummerGyldighedStatusIndikator == cprBorger.PersonNummerGyldighedStatusIndikator);
            }
            if (cprBorger.PersonNationalityCode != null && !string.IsNullOrEmpty(cprBorger.PersonNationalityCode.Value))
            {
                pred = pred.And(pt => pt.PersonNationalityCode == cprBorger.PersonNationalityCode.Value);
            }
            if (cprBorger.NavneAdresseBeskyttelseIndikator)
            {
                // False is treated like Not specified
                pred = pred.And(pt => pt.NavneAdresseBeskyttelseIndikator == cprBorger.NavneAdresseBeskyttelseIndikator);
            }
            if (cprBorger.TelefonNummerBeskyttelseIndikator)
            {
                // False is treated like Not specified
                pred = pred.And(pt => pt.TelefonNummerBeskyttelseIndikator == cprBorger.TelefonNummerBeskyttelseIndikator);
            }
            if (cprBorger.ForskerBeskyttelseIndikator)
            {
                // False is treated like Not specified 
                pred = pred.And(pt => pt.ForskerBeskyttelseIndikator == cprBorger.ForskerBeskyttelseIndikator);
            }

            // CprBorger fields - After address
            // --------------------------------
            if (!string.IsNullOrEmpty(cprBorger.AdresseNoteTekst))
            {
                pred = pred.And(pt => pt.AdresseNoteTekst == cprBorger.AdresseNoteTekst);
            }
            if (cprBorger.FolkekirkeMedlemIndikator)
            {
                // False is treated like Not specified
                pred = pred.And(pt => pt.FolkekirkeMedlemIndikator == cprBorger.FolkekirkeMedlemIndikator);
            }

            //  FolkeregisterAdresse fields
            // ----------------------------
            if (cprBorger.FolkeregisterAdresse != null && cprBorger.FolkeregisterAdresse.Item is DanskAdresseType)
            {
                var danskAddress = cprBorger.FolkeregisterAdresse.Item as DanskAdresseType;
                pred = pred.And(danskAddress);
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, DanskAdresseType danskAddress)
        {
            if (!string.IsNullOrEmpty(danskAddress.NoteTekst))
            {
                pred = pred.And(pt => pt.NoteTekst_DanskAdresse == danskAddress.NoteTekst);
            }
            if (danskAddress.UkendtAdresseIndikator)
            {
                // False is treated like Not specified    
                pred = pred.And(pt => pt.UkendtAdresseIndikator == danskAddress.UkendtAdresseIndikator);
            }
            if (danskAddress.SpecielVejkodeIndikatorSpecified)
            {
                // False is treated like Not specified    
                pred = pred.And(pt => pt.SpecielVejkodeIndikator == danskAddress.SpecielVejkodeIndikator);
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
            return pred;
        }
    }
}