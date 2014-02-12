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
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Providers.KMD.WS_AS78205;
using CprBroker.Providers.KMD.WS_AS78207;

namespace CprBroker.Providers.KMD
{
    public class KmdResponse : IPersonRelatedPnrSource
    {
        public AS78205Response AS78205Response { get; set; }
        public WS_AS78207.AS78207Response AS78207Response { get; set; }


        public RegistreringType1 ToRegistreringType1(Func<string, Guid> cpr2uuidFunc)
        {
            var addressResponse = new WS_AS78205.EnglishAS78205Response(AS78205Response);
            var detailsResponse = new WS_AS78207.EnglishAS78207Response(AS78207Response);

            var ret = new RegistreringType1()
            {
                AttributListe = detailsResponse.ToAttributListeType(addressResponse),
                TilstandListe = detailsResponse.ToTilstandListeType(),
                RelationListe = ToRelationListeType(detailsResponse, cpr2uuidFunc),

                AktoerRef = Constants.Actor,
                CommentText = Constants.CommentText,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                Tidspunkt = TidspunktType.Create(detailsResponse.GetRegistrationDate()),
                Virkning = null,
                SourceObjectsXml = Strings.SerializeObject(this)
            };
            ret.CalculateVirkning();
            return ret;
        }

        public RelationListeType ToRelationListeType(EnglishAS78207Response details, Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType();

            //Children
            if (details.ChildrenPNRs != null)
            {
                var childPnrs = (from pnr in details.ChildrenPNRs where pnr.Replace("-", "").Length > 0 select pnr.Replace("-", "")).ToArray();
                var uuids = Array.ConvertAll<string, Guid>(childPnrs, (cpr) => cpr2uuidFunc(cpr));
                ret.Boern = Array.ConvertAll<Guid, PersonFlerRelationType>
                (
                    uuids,
                    (pId) => PersonFlerRelationType.Create(pId, null, null)
                );
            }

            //Father
            if (Convert.ToDecimal(details.FatherPNR) > 0)
            {
                ret.Fader = new PersonRelationType[] { PersonRelationType.Create(cpr2uuidFunc(details.FatherPNR), null, null) };
            }
            //Mother
            if (Convert.ToDecimal(details.MotherPNR) > 0)
            {
                ret.Fader = new PersonRelationType[] { PersonRelationType.Create(cpr2uuidFunc(details.MotherPNR), null, null) };
            }

            // Spouse
            if (Convert.ToDecimal(details.SpousePNR) > 0)
            {
                var maritalStatus = Utilities.ToPartMaritalStatus(details.MaritallStatusCode[0]);
                var maritalStatusDate = Utilities.ToDateTime(details.MaritalStatusDate);
                bool isMarried = maritalStatus == CivilStatusKodeType.Gift || maritalStatus == CivilStatusKodeType.RegistreretPartner;
                var spouseUuid = cpr2uuidFunc(details.SpousePNR);
                ret.Aegtefaelle = new PersonRelationType[]
                    {
                        PersonRelationType.Create
                        (
                            spouseUuid,
                            isMarried? maritalStatusDate : null,
                            isMarried? null : maritalStatusDate
                       )
                    };
            }
            // TODO: Fill other relationships such as custody
            return ret;
        }

        public string[] RelatedPnrs
        {
            get
            {
                var details = new EnglishAS78207Response(this.AS78207Response);
                var ret = new List<string>(new string[]{
                    details.FatherPNR,
                    details.MotherPNR,
                    details.SpousePNR,
                });
                if (details.ChildrenPNRs != null)
                {
                    ret.AddRange(details.ChildrenPNRs);
                }
                return ret.Where(pnr => PartInterface.Strings.IsValidPersonNumber(pnr)).ToArray();
            }
        }
    }
}
