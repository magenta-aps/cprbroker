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

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType : IPersonRelatedPnrSource
    {
        public RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
        {
            return new RelationListeType()
            {
                Aegtefaelle = ToSpouses(cpr2uuidFunc),
                Boern = ToChildren(cpr2uuidFunc),
                Bopaelssamling = ToBopaelssamling(),
                ErstatningAf = ToErstatningAf(cpr2uuidFunc),
                ErstatningFor = ToErstatningFor(),
                Fader = ToFather(cpr2uuidFunc),
                Foraeldremyndighedsboern = ToForaeldremyndighedsboern(),
                Foraeldremyndighedsindehaver = ToForaeldremyndighedsindehaver(cpr2uuidFunc),
                LokalUdvidelse = ToLokalUdvidelseType(),
                Moder = ToMother(cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = ToRetligHandleevneVaergeForPersonen(),
                RetligHandleevneVaergemaalsindehaver = ToRetligHandleevneVaergemaalsindehaver(cpr2uuidFunc)
            };
        }

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToFather(cpr2uuidFunc);
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToMother(cpr2uuidFunc);
        }

        public PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            return ChildType.ToPersonFlerRelationType(this.Child, cpr2uuidFunc);
        }

        public PersonRelationType[] ToErstatningAf(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonInformation.ToReplacedByRelationType(cpr2uuidFunc);
        }

        private List<ICivilStatus> HistoricalCivilStatusAsInterface
        {
            get { return this.HistoricalCivilStatus.Select(h => h as ICivilStatus).ToList(); }
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return CivilStatusWrapper.ToRegisteredPartners(this.CurrentCivilStatus, this.HistoricalCivilStatusAsInterface, cpr2uuidFunc);
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return CivilStatusWrapper.ToSpouses(this.CurrentCivilStatus, this.HistoricalCivilStatusAsInterface, cpr2uuidFunc);
        }

        public PersonFlerRelationType[] ToRetligHandleevneVaergemaalsindehaver(Func<string, Guid> cpr2uuidFunc)
        {
            // Persons who have legal authority on current person
            return DisempowermentType.ToPersonRelationType(this.Disempowerment, cpr2uuidFunc);
        }

        public PersonRelationType[] ToForaeldremyndighedsindehaver(Func<string, Guid> cpr2uuidFunc)
        {
            // Parental authority owner
            return ParentalAuthorityType.ToPersonRelationType(this.ParentalAuthority, this.ParentsInformation, cpr2uuidFunc);
        }

        public string[] RelatedPnrs
        {
            get
            {
                var ret = new List<string>(new string[]{
                    this.PersonInformation.CurrentCprNumber,
                    this.CurrentCivilStatus.ToRelationPNROrNull(),
                    this.Disempowerment.ToRelationPNROrNull()
                });
                ret.AddRange(this.ParentsInformation.ToRelationPNRsOrNull());
                ret.AddRange(this.Child.Select(ch => ch.ChildPNR));
                ret.AddRange(this.HistoricalCivilStatus.Select(civ => civ.SpousePNR));
                ret.AddRange(ParentalAuthority.Select(pa => pa.RelationPNR));

                return ret.Where(pnr => Utilities.Strings.IsValidPersonNumber(pnr)).ToArray();
            }
        }
    }
}
