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
            Parents,
            Children,
            Spouses,
            ReplacedBy,
            SubstituteFor
        }

        public Effect<Schemas.Part.PersonRelation> ToXmlType222()
        {
            return new Effect<CprBroker.Schemas.Part.PersonRelation>()
            {
                //StartDate = this.StartDate,
                //EndDate = this.EndDate,
                //Value = new CprBroker.Schemas.Part.PersonRelation()
                //{
                //    TargetUUID = this.RelatedPersonUUID
                //}
            };
        }

        public static Schemas.Part.PersonRelations ToXmlType222(IQueryable<PersonRelationship> relations)
        {
            return new CprBroker.Schemas.Part.PersonRelations()
            {
                //Children = FilterRelationsByType(relations, RelationshipTypes.Children),
                //Parents = Array.ConvertAll<Effect<PersonRelation>, PersonRelation>(FilterRelationsByType(relations, RelationshipTypes.Children), (rel) => rel.Value),
                //Spouses = FilterRelationsByType(relations, RelationshipTypes.Spouses),
                //ReplacedBy = FilterRelationsByType(relations, RelationshipTypes.ReplacedBy).FirstOrDefault(),
                //SubstituteFor = FilterRelationsByType(relations, RelationshipTypes.SubstituteFor),
            };
        }

        // TODO: ensure that the database is not queried in this method when filtering relations by type
        private static Effect<PersonRelation>[] FilterRelationsByType222(IQueryable<PersonRelationship> relations, RelationshipTypes type)
        {
            return
            (
                from rel in relations
                where rel.RelationshipTypeId == (int)type
                select null as Effect<PersonRelation>// rel.ToXmlType()
            ).ToArray();
        }

        public static PersonRelationship[] FromXmlType(Schemas.Part.PersonRelations partRelations)
        {
            // TODO: implement PersonRelation.FromXmlType()
            return new PersonRelationship[0];
        }
    }
}
