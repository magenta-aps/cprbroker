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
using CprBroker.Utilities;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen : IPersonRelatedPnrSource
    {
        public virtual RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType()
            {
                Aegtefaelle = this.ToSpouses(cpr2uuidFunc),
                Boern = this.ToChildren(cpr2uuidFunc),
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Fader = this.ToFather(cpr2uuidFunc),
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                LokalUdvidelse = null,
                Moder = this.ToMother(cpr2uuidFunc),
                RegistreretPartner = this.ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null
            };

            return ret;
        }

        public PersonRelationType[] ToSpouses(CivilStatusKodeType existingStatusCode, CivilStatusKodeType[] terminatedStatusCodes, bool sameGenderSpouseForDead, Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                if (this.SpousePNR > 0)
                {
                    var status = Converters.ToCivilStatusKodeType(this.MaritalStatus);
                    if (status == CivilStatusKodeType.Ugift // Dead
                        && this.Spouse != null
                        && sameGenderSpouseForDead == (this.Gender == this.Spouse.Gender)
                        )
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                    }
                    else if (status == existingStatusCode) // Married or registered partner
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), this.ToMaritalStatusDate(), null);
                    }
                    else if (terminatedStatusCodes.Contains(status)) // Terminated relationship (divorced, widow...)
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                    }
                }
                return new PersonRelationType[0];
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(
                CivilStatusKodeType.Gift,
                new CivilStatusKodeType[]{
                    CivilStatusKodeType.Separeret,
                    CivilStatusKodeType.Skilt,
                    CivilStatusKodeType.Enke},
                false,
                cpr2uuidFunc
            );
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(
               CivilStatusKodeType.RegistreretPartner,
               new CivilStatusKodeType[]{
                    CivilStatusKodeType.OphaevetPartnerskab,
                    CivilStatusKodeType.Laengstlevende},
               true,
               cpr2uuidFunc
           );
        }

        public PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                var gender = Converters.ToPersonGenderCodeType(this.Gender);
                Func<System.Data.Linq.EntitySet<Child>, PersonFlerRelationType[]> converter =
                    (children) =>
                        children.Select(child => child.ToPersonFlerRelationType(cpr2uuidFunc)).ToArray();
                switch (gender)
                {
                    case PersonGenderCodeType.male:
                        return converter(this.ChildrenAsFather);
                    case PersonGenderCodeType.female:
                        return converter(this.ChildrenAsMother);
                }
                return new PersonFlerRelationType[0];
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            if (this.FatherPNR > 0)
            {
                if (Converters.IsValidCprNumber(this.FatherPNR))
                {
                    if (cpr2uuidFunc != null)
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(this.FatherPNR)));
                    }
                    else
                    {
                        throw new ArgumentNullException("cpr2uuidFunc");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid FatherPNR", "citizen.FatherPNR");
                }
            }
            return new PersonRelationType[0];
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            if (this.MotherPNR > 0)
            {
                if (Converters.IsValidCprNumber(this.MotherPNR))
                {
                    if (cpr2uuidFunc != null)
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(this.MotherPNR)));
                    }
                    else
                    {
                        throw new ArgumentNullException("cpr2uuidFunc");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid MotherPNR", "citizen.MotherPNR");
                }
            }
            return new PersonRelationType[0];
        }


        public string[] RelatedPnrs
        {
            get
            {
                var ret = new List<decimal>(new decimal[] {
                    this.FatherPNR,
                    this.MotherPNR,
                    this.SpousePNR
                });
                ret.AddRange(this.ChildrenAsFather.Select(ch => ch.PNR));
                ret.AddRange(this.ChildrenAsMother.Select(ch => ch.PNR));

                return ret
                    .Select(pnr => pnr.ToPnrDecimalString())
                    .Where(pnr => PartInterface.Strings.IsValidPersonNumber(pnr))
                    .ToArray();
            }
        }
    }
}