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
using System.Data.Linq;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the PersonRelationship table
    /// </summary>
    public partial class PersonRelationship
    {
        /// <summary>
        /// Contains all possible types of relationships between people
        /// </summary>
        public enum RelationshipTypes
        {
            Mother = 0,
            Father = 1,
            Children = 2,
            Spouse = 3,
            RegisteredPartner = 4,
            ResidenceCollection = 5,
            Custody = 6,
            ParentingAdultChildren = 7,
            GuardianOfPerson = 8,
            GuardianshipOwner = 9,
            ReplacementFor = 10,
            ReplacedBy = 11,
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonRelationship>(pr => pr.Effect);
            loadOptions.LoadWith<PersonRelationship>(pr => pr.RelationshipType);
        }

        #region Conversion to XML types
        public PersonRelationType ToPersonRelationType()
        {
            return new PersonRelationType
            {
                CommentText = this.CommentText,
                ReferenceID = UnikIdType.Create(RelatedPersonUuid),
                Virkning = Effect.ToVirkningType(Effect)
            };
        }
        public PersonFlerRelationType ToPersonFlerRelationType()
        {
            return new PersonFlerRelationType
            {
                CommentText = this.CommentText,
                ReferenceID = UnikIdType.Create(this.RelatedPersonUuid),
                Virkning = Effect.ToVirkningType(Effect)
            };
        }

        public static Schemas.Part.RelationListeType ToXmlType(IQueryable<PersonRelationship> relations)
        {
            return new CprBroker.Schemas.Part.RelationListeType()
            {
                Aegtefaelle = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Spouse),
                Boern = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.Children),
                Bopaelssamling = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ResidenceCollection),
                ErstatningFor = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ReplacementFor),
                ErstatningAf = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.ReplacedBy),
                Fader = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Father),
                Moder = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Mother),
                Foraeldremyndighedsboern = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ParentingAdultChildren),
                Foraeldremyndighedsindehaver = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Custody),
                LokalUdvidelse = null,
                RegistreretPartner = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.RegisteredPartner),
                RetligHandleevneVaergeForPersonen = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.GuardianOfPerson),
                RetligHandleevneVaergemaalsindehaver = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.GuardianshipOwner),
            };
        }

        private static TRelation[] FilterRelationsByType<TRelation>(IQueryable<PersonRelationship> relations, RelationshipTypes type) where TRelation : class
        {
            return
            (
                from rel in relations
                where rel.RelationshipTypeId == (int)type
                select typeof(TRelation) == typeof(PersonRelationType) ? rel.ToPersonRelationType() as TRelation : rel.ToPersonFlerRelationType() as TRelation
            ).ToArray();
        }
        #endregion

        #region Creation from XML types
        public static PersonRelationship[] FromXmlType(Schemas.Part.RelationListeType partRelations)
        {
            if (partRelations != null)
            {
                var ret = new List<PersonRelationship>();
                if (partRelations != null)
                {
                    ret.AddRange(ListFromXmlType(partRelations.Aegtefaelle, RelationshipTypes.Spouse));
                    ret.AddRange(ListFromXmlType(partRelations.Boern, RelationshipTypes.Children));
                    ret.AddRange(ListFromXmlType(partRelations.Bopaelssamling, RelationshipTypes.ResidenceCollection));
                    ret.AddRange(ListFromXmlType(partRelations.ErstatningFor, RelationshipTypes.ReplacementFor));
                    ret.AddRange(ListFromXmlType(partRelations.ErstatningAf, RelationshipTypes.ReplacedBy));
                    ret.AddRange(ListFromXmlType(partRelations.Fader, RelationshipTypes.Father));
                    ret.AddRange(ListFromXmlType(partRelations.Moder, RelationshipTypes.Mother));
                    ret.AddRange(ListFromXmlType(partRelations.Foraeldremyndighedsboern, RelationshipTypes.ParentingAdultChildren));
                    ret.AddRange(ListFromXmlType(partRelations.Foraeldremyndighedsindehaver, RelationshipTypes.Custody));
                    ret.AddRange(ListFromXmlType(partRelations.RegistreretPartner, RelationshipTypes.RegisteredPartner));
                    ret.AddRange(ListFromXmlType(partRelations.RetligHandleevneVaergeForPersonen, RelationshipTypes.GuardianOfPerson));
                    ret.AddRange(ListFromXmlType(partRelations.RetligHandleevneVaergemaalsindehaver, RelationshipTypes.GuardianshipOwner));
                }
                return ret.ToArray();
            }
            return new PersonRelationship[0];
        }

        private static PersonRelationship[] ListFromXmlType(PersonRelationType[] oio, RelationshipTypes relType)
        {
            if (oio != null)
            {
                return oio.AsQueryable()
                    .Where(o => o != null)
                    .Select(
                        r => new PersonRelationship()
                        {
                            CommentText = r.CommentText,
                            Effect = Effect.FromVirkningType(r.Virkning),
                            PersonRelationshipId = Guid.NewGuid(),
                            RelatedPersonUuid = new Guid(r.ReferenceID.Item),
                            RelationshipTypeId = (int)relType
                        }
                    )
                    .ToArray();
            }
            return new PersonRelationship[0];
        }

        private static PersonRelationship[] ListFromXmlType(PersonFlerRelationType[] oio, RelationshipTypes relType)
        {
            if (oio != null)
            {
                return oio.AsQueryable()
                    .Where(r => r != null)
                    .Select(
                        r => new PersonRelationship()
                        {
                            CommentText = r.CommentText,
                            Effect = Effect.FromVirkningType(r.Virkning),
                            PersonRelationshipId = Guid.NewGuid(),
                            RelatedPersonUuid = new Guid(r.ReferenceID.Item),
                            RelationshipTypeId = (int)relType
                        }
                    )
                    .ToArray();
            }
            return new PersonRelationship[0];
        }
        #endregion
    }
}
