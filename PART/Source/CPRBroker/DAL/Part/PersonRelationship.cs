using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonRelationship
    {
        public enum RelationshipTypes
        {
            //Parents,
            Children,
            Spouses,
            ResidenceCollection,
            ReplacedBy,
            ReplacementFor,
            Father,
            Mother,
            ParentingAdultChildren,
            Custody,
            RegisteredPartner,
            GuardianOfPerson,
            GuardianshipOwner
        }

        public PersonRelationType ToPersonRelationType()
        {
            return new PersonRelationType
            {
                CommentText = this.CommentText,
                ReferenceIDTekst = this.RelatedPersonUuid.ToString(),
                //TODO: Handle null Effect
                Virkning = Effect.ToXmlType()
            };
        }
        public PersonFlerRelationType ToPersonFlerRelationType()
        {
            return new PersonFlerRelationType
            {
                CommentText = this.CommentText,
                ReferenceIDTekst = this.RelatedPersonUuid.ToString(),
                //TODO: Handle null Effect
                Virkning = Effect.ToXmlType()
            };
        }

        public static Schemas.Part.RelationListeType ToXmlType(IQueryable<PersonRelationship> relations)
        {
            return new CprBroker.Schemas.Part.RelationListeType()
            {
                Aegtefaelle = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Spouses),
                Boern = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.Children),
                Bopaelssamling = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ResidenceCollection),
                ErstatningFor = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.ReplacementFor),
                ErstattesAf = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ReplacedBy),
                Fader = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Father),
                Moder = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.Mother),
                ForaeldremyndgihdedsBoern = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.ParentingAdultChildren),
                ForaeldremyndgihdedsIndehaver = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.Custody),
                LokalUdvidelse = new LokalUdvidelseType(),
                RegistreretPartner = FilterRelationsByType<PersonRelationType>(relations, RelationshipTypes.RegisteredPartner),
                RetligHandleevneVaergeForPersonen = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.GuardianOfPerson),
                RetligHandleevneVaergemaalsIndehaver = FilterRelationsByType<PersonFlerRelationType>(relations, RelationshipTypes.GuardianshipOwner),
            };
        }

        private static List<TRelation> FilterRelationsByType<TRelation>(IQueryable<PersonRelationship> relations, RelationshipTypes type) where TRelation : class
        {
            return
            (
                from rel in relations
                where rel.RelationshipTypeId == (int)type
                select typeof(TRelation) == typeof(PersonRelationType) ? rel.ToPersonRelationType() as TRelation : rel.ToPersonFlerRelationType() as TRelation
            ).ToList();
        }

        public static PersonRelationship[] FromXmlType(Schemas.Part.PersonRelations partRelations)
        {
            // TODO: implement PersonRelation.FromXmlType()
            return new PersonRelationship[0];
        }
    }
}
