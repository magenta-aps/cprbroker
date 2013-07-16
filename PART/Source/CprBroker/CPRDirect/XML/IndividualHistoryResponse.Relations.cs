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

        public RelationListeType ToRelationListeType(Func<string, Guid> cpr2UuidFunc)
        {
            // Marriages
            var civilStates = Interval
                .CreateFromData<TimedTypeWrapper<ICivilStatus>>(this.ItemsAsTimedType, new DataTypeTags[] { DataTypeTags.CivilStatus })
                .Select(w => w.TimedObject)
                .ToList();
            var spouses = CivilStatusWrapper.ToSpouses(null, civilStates, cpr2UuidFunc);
            var partners = CivilStatusWrapper.ToRegisteredPartners(null, civilStates, cpr2UuidFunc);

            // Now fill the return object
            return new RelationListeType()
            {
                Aegtefaelle = spouses,
                RegistreretPartner = partners,
                Fader = FromLatestRegistration<ParentsInformationType>().ToFather(cpr2UuidFunc),
                Moder = FromLatestRegistration<ParentsInformationType>().ToMother(cpr2UuidFunc),
                Boern = ChildType.ToPersonFlerRelationType(LatestRegistration.Child, cpr2UuidFunc),
                RetligHandleevneVaergemaalsindehaver = ToRetligHandleevneVaergemaalsindehaver(cpr2UuidFunc),
                Foraeldremyndighedsindehaver = ToForaeldremyndighedsindehaver(cpr2UuidFunc),
                Bopaelssamling = null,
                ErstatningAf = ToErstatningAf(cpr2UuidFunc),
                ErstatningFor = null,
                Foraeldremyndighedsboern = null,
                LokalUdvidelse = null,
                RetligHandleevneVaergeForPersonen = null
            };
        }

        public PersonFlerRelationType[] ToRetligHandleevneVaergemaalsindehaver(Func<string, Guid> cpr2UuidFunc)
        {
            var disempowerments = Interval
                .CreateFromData<TimedTypeWrapper<DisempowermentType>>(this.ItemsAsTimedType)
                .SelectMany(w => DisempowermentType.ToPersonRelationType(w.TimedObject as DisempowermentType, cpr2UuidFunc))
                .ToArray();
            return disempowerments;
        }

        public PersonRelationType[] ToForaeldremyndighedsindehaver(Func<string, Guid> cpr2UuidFunc)
        {
            var custodyOwners = Overwrite
                .Filter(ItemsAsTimedType.Where(i => i is ParentalAuthorityType))
                .Select(auth => (auth as ParentalAuthorityType).ToPersonRelationType(cpr2UuidFunc))
                .ToArray();
            return custodyOwners;
        }

        public PersonRelationType[] ToErstatningAf(Func<string, Guid> cpr2UuidFunc)
        {
            var personInformation = FromLatestRegistration<PersonInformationType>();
            return personInformation.ToReplacedByRelationType(cpr2UuidFunc);
        }
    }
}