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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using CprBroker.Utilities;

namespace CprBroker.Engine.Part
{
    /// <summary>
    /// Facade method for Search
    /// </summary>
    public class SearchFacadeMethodInfo : FacadeMethodInfo<SoegOutputType, string[]>
    {
        SoegInputType1 Input;

        private SearchFacadeMethodInfo()
        { }

        public SearchFacadeMethodInfo(SoegInputType1 input, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = input;
        }

        public override void Initialize()
        {
            this.SubMethodInfos = new SubMethodInfo[] { new SearchSubMethodInfo(Input) };
        }

        public override StandardReturType ValidateInput()
        {
            if (Input == null)
            {
                return StandardReturType.NullInput();
            }

            if (Input.SoegObjekt == null)
            {
                return StandardReturType.NullInput("SoegObjekt");
            }

            if (!string.IsNullOrEmpty(Input.SoegObjekt.UUID) && !Strings.IsGuid(Input.SoegObjekt.UUID))
            {
                return StandardReturType.InvalidUuid(Input.SoegObjekt.UUID);
            }
            // Start index & max results
            if (!string.IsNullOrEmpty(Input.FoersteResultatReference))
            {
                int startResult;
                if (!int.TryParse(Input.FoersteResultatReference, out startResult))
                {
                    return StandardReturType.InvalidValue("FoersteResultatReference", Input.FoersteResultatReference);
                }
                if (startResult < 0)
                {
                    return StandardReturType.ValueOutOfRange("FoersteResultatReference", Input.FoersteResultatReference);
                }
            }

            if (!string.IsNullOrEmpty(Input.MaksimalAntalKvantitet))
            {
                int maxResults;
                if (!int.TryParse(Input.MaksimalAntalKvantitet, out maxResults))
                {
                    return StandardReturType.InvalidValue("MaksimalAntalKvantitet", Input.MaksimalAntalKvantitet);
                }
                if (maxResults < 0)
                {
                    return StandardReturType.ValueOutOfRange("MaksimalAntalKvantitet", Input.MaksimalAntalKvantitet);
                }
            }

            // Not implemented criteria
            if (Input.SoegObjekt.SoegRegistrering != null)
            {
                return StandardReturType.NotImplemented("SoegRegistrering");
            }
            if (Input.SoegObjekt.SoegRelationListe != null)
            {
                return StandardReturType.NotImplemented("SoegRelationListe");
            }

            if (Input.SoegObjekt.SoegTilstandListe != null)
            {
                return StandardReturType.NotImplemented("SoegTilstandListe");
            }

            if (Input.SoegObjekt.SoegVirkning != null)
            {
                return StandardReturType.NotImplemented("SoegVirkning");
            }

            // Now validate attribute lists
            if (Input.SoegObjekt.SoegAttributListe != null)
            {
                if (Input.SoegObjekt.SoegAttributListe.LokalUdvidelse != null)
                {
                    return StandardReturType.NotImplemented("SoegAttributListe.LokalUdvidelse");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegRegisterOplysning != null)
                {
                    return StandardReturType.NotImplemented("SoegAttributListe.SoegRegisterOplysning");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegSundhedOplysning != null)
                {
                    return StandardReturType.NotImplemented("SoegAttributListe.SoegSundhedOplysning");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegEgenskab != null)
                {
                    foreach (var egen in Input.SoegObjekt.SoegAttributListe.SoegEgenskab)
                    {
                        if (egen.AndreAdresser != null)
                        {
                            return StandardReturType.NotImplemented("AndreAdresser");
                        }
                        if (!string.IsNullOrEmpty(egen.FoedestedNavn))
                        {
                            return StandardReturType.NotImplemented("FoedestedNavn");
                        }

                        if (!string.IsNullOrEmpty(egen.FoedselsregistreringMyndighedNavn))
                        {
                            return StandardReturType.NotImplemented("FoedselsregistreringMyndighedNavn");
                        }
                        if (egen.KontaktKanal != null)
                        {
                            return StandardReturType.NotImplemented("KontaktKanal");
                        }
                        if (egen.NaermestePaaroerende != null)
                        {
                            return StandardReturType.NotImplemented("NaermestePaaroerende");
                        }
                        if (egen.SoegVirkning != null)
                        {
                            return StandardReturType.NotImplemented("SoegVirkning");
                        }
                        if (egen.SoegVirkning != null)
                        {
                            return StandardReturType.NotImplemented("SoegVirkning");
                        }
                    }
                }
            }
            return StandardReturType.OK();
        }

        public override string[] Aggregate(object[] results)
        {
            var foundIds = results[0] as Guid[];
            return (from id in foundIds select id.ToString()).ToArray();
        }
    }
}
