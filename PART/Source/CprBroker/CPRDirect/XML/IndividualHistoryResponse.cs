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
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualHistoryResponseType
    {

        public PersonIdentifier PersonIdentifier { get; set; }
        public IndividualResponseType[] IndividualResponseObjects { get; private set; }

        public IQueryable<ITimedType> ItemsAsTimedType
        {
            get
            {
                var ret = new List<ITimedType>();
                ret.AddRange(IndividualResponseObjects.SelectMany(resp => resp.GetChildrenAsTimedObjects()));
                // TODO: Test with historical PNR objects to see if corrections work
                ret.AddRange(IndividualResponseObjects.Select(resp => new CurrentPnrTypeAdaptor(resp.PersonInformation, resp.HistoricalPNR) as ITimedType));
                ret.AddRange(IndividualResponseObjects.Select(resp => resp.GetFolkeregisterAdresseSource(false) as ITimedType));
                return ret.Where(i => i != null).Distinct().AsQueryable();
            }
        }

        public string[] AllRelationPNRs
        {
            get
            {
                var ret = new List<string>();
                ret.AddRange(
                    IndividualResponseObjects
                    .SelectMany(resp => resp.GetChildrenAsType<IRelationship>())
                    .Select(o => o.RelationPNR)
                    );

                ret.AddRange(
                    IndividualResponseObjects
                    .SelectMany(resp => resp.GetChildrenAsType<IMultipleRelationship>())
                    .SelectMany(o => o.RelationPNRs)
                    );

                return ret
                    .Where(pnr => !string.IsNullOrEmpty(Converters.ToPnrStringOrNull(pnr)))
                    .Distinct()
                    .ToArray();
            }
        }

        public IndividualResponseType LatestRegistration
        {
            get
            {
                return IndividualResponseObjects.OrderByDescending(resp => resp.RegistrationDate).FirstOrDefault();
            }
        }

        public T FromLatestRegistration<T>() where T : class
        {
            var latest = LatestRegistration;
            if (latest != null)
            {
                return LatestRegistration.GetChildrenAsType<T>().Where(o => o is T).FirstOrDefault() as T;
            }
            return null;
        }

        public IndividualHistoryResponseType(PersonIdentifier pId, IQueryable<IndividualResponseType> individualResponseObjects)
        {
            this.PersonIdentifier = pId;
            this.IndividualResponseObjects = individualResponseObjects.ToArray();
        }

        public FiltreretOejebliksbilledeType ToFiltreretOejebliksbilledeType(Func<string, Guid> cpr2UuidFunc)
        {
            return new FiltreretOejebliksbilledeType()
            {
                AttributListe = ToAttributListeType(),
                UUID = PersonIdentifier.UUID.ToString(),
                BrugervendtNoegleTekst = PersonIdentifier.CprNumber,
                RelationListe = ToRelationListeType(cpr2UuidFunc),
                TilstandListe = ToTilstandListeType()
            };
        }

        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = ToEgenskabType(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                RegisterOplysning = ToRegisterOplysningType(),
                SundhedOplysning = ToSundhedOplysning()
            };
        }

        private RegisterOplysningType[] ToRegisterOplysningType()
        {
            return Interval
                .CreateFromData<RegisterOplysningInterval>(this.ItemsAsTimedType, RegisterOplysningInterval.Tags)
                .Select(i => i.ToRegisterOplysningType())
                .ToArray();
        }

        private EgenskabType[] ToEgenskabType()
        {
            var intervals = Interval
                .CreateFromData<EgenskabInterval>(this.ItemsAsTimedType, EgenskabInterval.Tags);

            return intervals
                .Select(i => i.ToEgenskabType())
                .ToArray();
        }

        public TilstandListeType ToTilstandListeType()
        {
            return null;
        }

    }
}