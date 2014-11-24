/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
            if (cprBorger.FolkeregisterAdresse != null)
            {
                var item = cprBorger.FolkeregisterAdresse.Item;
                if (!string.IsNullOrEmpty(item.NoteTekst))
                {
                    pred = pred.And(pt => pt.NoteTekst_DanskAdresse == item.NoteTekst);
                }
                if (item.UkendtAdresseIndikator)
                {
                    // False is treated like Not specified    
                    pred = pred.And(pt => pt.UkendtAdresseIndikator == item.UkendtAdresseIndikator);
                }

                // Type specific predicates
                if (item is DanskAdresseType)
                {
                    pred = pred.And(cprBorger.FolkeregisterAdresse.Item as DanskAdresseType);
                }
                else if (item is GroenlandAdresseType)
                {
                    pred = pred.And(item as GroenlandAdresseType);
                }
                else if (item is VerdenAdresseType)
                {
                    pred = pred.And(item as VerdenAdresseType);
                }
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, CountryIdentificationCodeType countryIdentificationCode)
        {
            if (!string.IsNullOrEmpty(countryIdentificationCode.Value))
            {
                pred = pred.And(pt => pt.CountryIdentificationCode == countryIdentificationCode.Value);
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, DanskAdresseType danskAddress)
        {
            pred = pred.And(p => p.AddressType == 'D');

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

                    if (postal.CountryIdentificationCode != null)
                    {
                        pred = pred.And(postal.CountryIdentificationCode);
                    }
                }
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, GroenlandAdresseType groenlandskAddress)
        {
            if (groenlandskAddress.AddressCompleteGreenland != null)
            {
                var addressCompleteGreenland = groenlandskAddress.AddressCompleteGreenland;

                pred = pred.And(p => p.AddressType == 'G');

                if (addressCompleteGreenland.CountryIdentificationCode != null)
                {
                    pred = pred.And(addressCompleteGreenland.CountryIdentificationCode);
                }
                if (addressCompleteGreenland.DistrictName != null)
                {
                    pred.And(p => p.DistrictName == addressCompleteGreenland.DistrictName);
                }
                if (addressCompleteGreenland.DistrictSubdivisionIdentifier != null)
                {
                    pred.And(p => p.DistrictSubdivisionIdentifier == addressCompleteGreenland.DistrictSubdivisionIdentifier);
                }
                if (addressCompleteGreenland.FloorIdentifier != null)
                {
                    pred.And(p => p.FloorIdentifier == addressCompleteGreenland.FloorIdentifier);
                }
                if (addressCompleteGreenland.GreenlandBuildingIdentifier != null)
                {
                    pred.And(p => p.GreenlandBuildingIdentifier == addressCompleteGreenland.GreenlandBuildingIdentifier);
                }
                if (addressCompleteGreenland.MailDeliverySublocationIdentifier != null)
                {
                    pred.And(p => p.MailDeliverySublocationIdentifier == addressCompleteGreenland.MailDeliverySublocationIdentifier);
                }
                if (addressCompleteGreenland.MunicipalityCode != null)
                {
                    pred.And(p => p.MunicipalityCode == addressCompleteGreenland.MunicipalityCode);
                }
                if (addressCompleteGreenland.PostCodeIdentifier != null)
                {
                    pred.And(p => p.PostCodeIdentifier == addressCompleteGreenland.PostCodeIdentifier);
                }
                if (addressCompleteGreenland.StreetBuildingIdentifier != null)
                {
                    pred.And(p => p.StreetBuildingIdentifier == addressCompleteGreenland.StreetBuildingIdentifier);
                }
                if (addressCompleteGreenland.StreetCode != null)
                {
                    pred.And(p => p.StreetCode == addressCompleteGreenland.StreetCode);
                }
                if (addressCompleteGreenland.StreetName != null)
                {
                    pred.And(p => p.StreetName == addressCompleteGreenland.StreetName);
                }
                if (addressCompleteGreenland.StreetNameForAddressingName != null)
                {
                    pred.And(p => p.StreetNameForAddressingName == addressCompleteGreenland.StreetNameForAddressingName);
                }
                if (addressCompleteGreenland.SuiteIdentifier != null)
                {
                    pred.And(p => p.SuiteIdentifier == addressCompleteGreenland.SuiteIdentifier);
                }
            }
            if (groenlandskAddress.SpecielVejkodeIndikatorSpecified)
            {
                pred = pred.And(p => p.SpecielVejkodeIndikator == groenlandskAddress.SpecielVejkodeIndikator);
            }
            return pred;
        }

        public static Expression<Func<PersonSearchCache, bool>> And(this Expression<Func<PersonSearchCache, bool>> pred, VerdenAdresseType verdenAdresse)
        {
            pred = pred.And(p => p.AddressType == 'V');
            
            var foreignAddress = verdenAdresse.ForeignAddressStructure;
            if(foreignAddress!=null)
            {
                if (foreignAddress.CountryIdentificationCode != null)
                {
                    pred = pred.And(foreignAddress.CountryIdentificationCode);
                }
                if (!string.IsNullOrEmpty(foreignAddress.LocationDescriptionText))
                {
                    pred = pred.And(p => p.LocationDescriptionText == foreignAddress.LocationDescriptionText);
                }
                if (!string.IsNullOrEmpty(foreignAddress.PostalAddressFirstLineText))
                {
                    pred = pred.And(p => p.PostalAddressFirstLineText == foreignAddress.PostalAddressFirstLineText);
                }
                if (!string.IsNullOrEmpty(foreignAddress.PostalAddressSecondLineText))
                {
                    pred = pred.And(p => p.PostalAddressSecondLineText == foreignAddress.PostalAddressSecondLineText);
                }
                if (!string.IsNullOrEmpty(foreignAddress.PostalAddressThirdLineText))
                {
                    pred = pred.And(p => p.PostalAddressThirdLineText == foreignAddress.PostalAddressThirdLineText);
                }
                if (!string.IsNullOrEmpty(foreignAddress.PostalAddressFourthLineText))
                {
                    pred = pred.And(p => p.PostalAddressFourthLineText == foreignAddress.PostalAddressFourthLineText);
                }
                if (!string.IsNullOrEmpty(foreignAddress.PostalAddressFirstLineText))
                {
                    pred = pred.And(p => p.PostalAddressFifthLineText == foreignAddress.PostalAddressFifthLineText);
                }
            }
            return pred;
        }
    }
}