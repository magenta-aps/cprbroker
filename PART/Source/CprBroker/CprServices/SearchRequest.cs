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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public class SearchRequest
    {
        public SearchRequest(string pnr)
        {
            AddCriteriaField("PNR", pnr);
        }

        public SearchRequest(SoegAttributListeType attributes)
        {
            #region Egenskab
            if (attributes.SoegEgenskab != null)
            {
                foreach (var egen in attributes.SoegEgenskab.Where(e => e != null))
                {
                    if (egen.NavnStruktur != null)
                    {
                        List<string> names = new List<string>();
                        if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameForAddressingName))
                        {
                            names.Add(egen.NavnStruktur.PersonNameForAddressingName);
                        }
                        if (egen.NavnStruktur.PersonNameStructure != null && !egen.NavnStruktur.PersonNameStructure.IsEmpty)
                        {
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonGivenName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonGivenName);
                            }
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonMiddleName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonMiddleName);
                            }
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonSurnameName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonSurnameName);
                            }
                        }
                        for (int i = 0; i < names.Count; i++)
                        {
                            var propName = "NVN";
                            if (i > 0)
                                propName += i;
                            AddCriteriaField(propName, names[i]);
                        }
                        //egen.NavnStruktur.KaldenavnTekst
                        //egen.NavnStruktur.NoteTekst
                    }

                    // Gender
                    if (egen.PersonGenderCodeSpecified)
                    {
                        string gender = null;
                        switch (egen.PersonGenderCode)
                        {
                            case PersonGenderCodeType.female:
                                gender = "K";
                                break;
                            case PersonGenderCodeType.male:
                                gender = "M";
                                break;
                        }
                        AddCriteriaField("KON", gender);
                    }
                    // TODO: Unsupported
                    //egen.AndreAdresser
                    //egen.BirthDate 
                    //egen.FoedestedNavn
                    //egen.FoedselsregistreringMyndighedNavn
                    //egen.KontaktKanal                    
                    //egen.NaermestePaaroerende
                    //egen.SoegVirkning                    
                }
            }
            #endregion

            #region RegisterOplysning
            if (attributes.SoegRegisterOplysning != null)
            {
                foreach (var oplys in attributes.SoegRegisterOplysning.Where(r => r != null))
                {
                    if (oplys.Item is CprBorgerType)
                    {
                        var cpr = oplys.Item as CprBorgerType;
                        //cpr.AdresseNoteTekst
                        //cpr.FolkekirkeMedlemIndikator
                        if (cpr.FolkeregisterAdresse != null)
                        {
                            if (cpr.FolkeregisterAdresse.Item is DanskAdresseType)
                            {
                                var dansk = cpr.FolkeregisterAdresse.Item as DanskAdresseType;
                                if (dansk.AddressComplete != null)
                                {
                                    if (dansk.AddressComplete.AddressAccess != null)
                                    {
                                        AddCriteriaField("KOMK", dansk.AddressComplete.AddressAccess.MunicipalityCode);
                                        AddCriteriaField("HNR", dansk.AddressComplete.AddressAccess.StreetBuildingIdentifier);
                                        AddCriteriaField("VEJK", dansk.AddressComplete.AddressAccess.StreetCode);
                                    }
                                    if (dansk.AddressComplete.AddressPostal != null)
                                    {
                                        //dansk.AddressComplete.AddressPostal.CountryIdentificationCode
                                        //dansk.AddressComplete.AddressPostal.DistrictName
                                        //dansk.AddressComplete.AddressPostal.DistrictSubdivisionIdentifier
                                        AddCriteriaField("ETAG", dansk.AddressComplete.AddressPostal.FloorIdentifier);
                                        //dansk.AddressComplete.AddressPostal.MailDeliverySublocationIdentifier
                                        AddCriteriaField("POST", dansk.AddressComplete.AddressPostal.PostCodeIdentifier);
                                        //dansk.AddressComplete.AddressPostal.PostOfficeBoxIdentifier
                                        AddCriteriaField("HNR", dansk.AddressComplete.AddressPostal.StreetBuildingIdentifier);
                                        AddCriteriaField("VNVN", dansk.AddressComplete.AddressPostal.StreetName);
                                        // TODO: is it OK to use this?
                                        AddCriteriaField("VNVN", dansk.AddressComplete.AddressPostal.StreetNameForAddressingName);
                                        AddCriteriaField("SIDO", dansk.AddressComplete.AddressPostal.SuiteIdentifier);
                                    }
                                }
                                //dansk.AddressPoint
                                //dansk.NoteTekst
                                //dansk.PolitiDistriktTekst
                                //dansk.PostDistriktTekst
                                //dansk.SkoleDistriktTekst
                                //dansk.SocialDistriktTekst
                                //dansk.SogneDistriktTekst
                                //dansk.SpecielVejkodeIndikator
                                //dansk.UkendtAdresseIndikator
                                //dansk.ValgkredsDistriktTekst

                            }
                            else if (cpr.FolkeregisterAdresse.Item is GroenlandAdresseType)
                            {
                                var groen = cpr.FolkeregisterAdresse.Item as GroenlandAdresseType;
                                if (groen.AddressCompleteGreenland != null)
                                {
                                    //groen.AddressCompleteGreenland.CountryIdentificationCode
                                    //groen.AddressCompleteGreenland.DistrictName
                                    //groen.AddressCompleteGreenland.DistrictSubdivisionIdentifier
                                    AddCriteriaField("ETAG", groen.AddressCompleteGreenland.FloorIdentifier);
                                    AddCriteriaField("BNR", groen.AddressCompleteGreenland.GreenlandBuildingIdentifier);
                                    //groen.AddressCompleteGreenland.MailDeliverySublocationIdentifier
                                    AddCriteriaField("KOMK", groen.AddressCompleteGreenland.MunicipalityCode);
                                    AddCriteriaField("POST", groen.AddressCompleteGreenland.PostCodeIdentifier);
                                    AddCriteriaField("HNR", groen.AddressCompleteGreenland.StreetBuildingIdentifier);
                                    AddCriteriaField("VEJK", groen.AddressCompleteGreenland.StreetCode);
                                    AddCriteriaField("VNVN", groen.AddressCompleteGreenland.StreetName);
                                    // TODO: Is this OK to add another VNVN
                                    AddCriteriaField("VNVN", groen.AddressCompleteGreenland.StreetNameForAddressingName);
                                    AddCriteriaField("SIDO", groen.AddressCompleteGreenland.SuiteIdentifier);
                                }
                                //groen.NoteTekst
                                //groen.SpecielVejkodeIndikator
                                //groen.UkendtAdresseIndikator
                            }
                            else if (cpr.FolkeregisterAdresse.Item is VerdenAdresseType)
                            {
                                // Unsupported
                            }
                        }
                        //cpr.ForskerBeskyttelseIndikator
                        //cpr.NavneAdresseBeskyttelseIndikator
                        //cpr.PersonCivilRegistrationIdentifier
                        if (cpr.PersonNationalityCode != null)
                        {
                            // TODO: Validate the scheme and convert value to the expected corresponding value
                            AddCriteriaField("STB", cpr.PersonNationalityCode.Value);
                        }
                        //cpr.PersonNummerGyldighedStatusIndikator
                        //cpr.TelefonNummerBeskyttelseIndikator                        
                    }
                    else if (oplys.Item is UdenlandskBorgerType)
                    {
                        // Not implemented
                    }
                    else if (oplys.Item is UkendtBorgerType)
                    {
                        // Not implemented
                    }
                }
            }
            #endregion
        }

        public void AddCriteriaField(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                CriteriaFields.Add(new KeyValuePair<string, string>(name, value));
        }

        public List<KeyValuePair<string, string>> CriteriaFields = new List<KeyValuePair<string, string>>();
    }
}
