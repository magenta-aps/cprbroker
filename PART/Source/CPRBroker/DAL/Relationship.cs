using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.DAL
{

    public partial class Relationship
    {
        #region Expressions
        /// <summary>
        /// Expression to get the relationships of a person. Based on the parameter isForward, it returns Relations, Relations1 or the union of both
        /// Used only once to make an initial list of relations that will be filtered later based on type , time,...etc
        /// </summary>
        private static System.Linq.Expressions.Expression<Func<Person, bool?, IEnumerable<Relationship>>> PersonRelationsByDirectionExpression =
                (per, isForward) =>
                (isForward.HasValue ?
                    (isForward.Value ? per.Relationships : per.Relationships1)
                    : (per.Relationships.Union(per.Relationships1)));

        /// <summary>
        /// Expression to get the relationships of a person based on the type, direction,and current date
        /// </summary>
        private static System.Linq.Expressions.Expression<Func<Person, RelationshipType.RelationshipTypes, bool?, DateTime?, IEnumerable<Relationship>>> PersonRelationsByTypeAndDirectionExpression =
            (per, relationType, isForward, today) =>
                from rel in PersonRelationsByDirectionExpression.Compile()(per, isForward)
                orderby rel.RegistrationDate
                where rel.RelationshipTypeId == (int)relationType
                && (
                    (today == null)
                    ||
                    (rel.TimedRelationship == null)
                    ||
                    (rel.TimedRelationship != null && (rel.TimedRelationship.EndDate == null || rel.TimedRelationship.EndDate.Value > today.Value))
                )
                select rel;

        /// <summary>
        /// Expression to get the related person of a relation based on the direction
        /// </summary>
        private static Func<Person, Relationship, bool?, Person> RelatedDatabasePersonGetter =
            (person, rel, isForward) => isForward.HasValue
                ? (isForward.Value ? rel.RelatedPerson : rel.Person)
                : (rel.Person == person ? rel.RelatedPerson : rel.Person);

        #endregion

        #region Getters
        /// <summary>
        /// Gets an array of TimedRelationship objects
        /// </summary>
        /// <typeparam name="TOioRelationship"></typeparam>
        /// <param name="context"></param>
        /// <param name="cprNumber"></param>
        /// <param name="relationType"></param>
        /// <param name="isForward"></param>
        /// <param name="today"></param>
        /// <param name="customOioRelationshipSetMethod"></param>
        /// <returns></returns>
        public static TOioRelationship[] GetTimedRelationships<TOioRelationship>(
            CPRBrokerDALDataContext context,
            string cprNumber,
            RelationshipType.RelationshipTypes relationType,
            bool? isForward,
            DateTime? today,
            Action<Relationship, TOioRelationship> customOioRelationshipSetMethod
            )
            where TOioRelationship : TimedRelationshipType, new()
        {
            Action<Relationship, TOioRelationship> newCustomOioRelationshipMethod = (dbRel, oioRel) =>
                {
                    oioRel.RelationStartDate = dbRel.TimedRelationship.StartDate;
                    oioRel.RelationEndDate = dbRel.TimedRelationship.EndDate;

                    if (customOioRelationshipSetMethod != null)
                    {
                        customOioRelationshipSetMethod(dbRel, oioRel);
                    }
                };

            return GetRelationships<TOioRelationship>(
                context,
                cprNumber,
                relationType,
                isForward,
                today,
                newCustomOioRelationshipMethod
                );
        }

        /// <summary>
        /// Gets a list of a person's relationships
        /// </summary>
        /// <typeparam name="TOioRelationship"></typeparam>
        /// <param name="context"></param>
        /// <param name="cprNumber"></param>
        /// <param name="relationType"></param>
        /// <param name="isForward"></param>
        /// <param name="today"></param>
        /// <param name="customOioRelationshipMethod"></param>
        /// <returns>Array of TOioRelationship (possibly empty) if person exists, null otherwise</returns>
        public static TOioRelationship[] GetRelationships<TOioRelationship>(
            CPRBrokerDALDataContext context,
            string cprNumber,
            RelationshipType.RelationshipTypes relationType,
            bool? isForward,
            DateTime? today,
            Action<Relationship, TOioRelationship> customOioRelationshipMethod
            )
            where TOioRelationship : BaseRelationshipType, new()
        {
            var person = (from p in context.Persons
                          where p.PersonNumber == cprNumber
                          select p).FirstOrDefault();

            if (person != null)
            {
                var oioAndDbRelationPairs = (
                    from rel in PersonRelationsByTypeAndDirectionExpression.Compile()(person, relationType, isForward, today)
                    select new
                    {
                        OioRelation = new TOioRelationship()
                        {
                            SimpleCPRPerson = Relationship.RelatedDatabasePersonGetter(person, rel, isForward).ToSimpleCPRPerson()
                        },
                        DbRelationship = rel
                    }
                     ).ToArray();

                if (customOioRelationshipMethod != null)
                {
                    Array.ForEach(
                        oioAndDbRelationPairs,
                        (pair) => customOioRelationshipMethod(pair.DbRelationship, pair.OioRelation)
                    );
                }
                return (from r in oioAndDbRelationPairs select r.OioRelation).ToArray();
            }
            return null;
        }
        #endregion

        #region Updates
        /// <summary>
        /// Updates the database with a more recent version of relationships
        /// Only handles the Relationship table. Other tables should be handled in a method passed in customRelationCreationMethod parameter
        /// Does not submit changes
        /// </summary>
        /// <typeparam name="TOioRelation">OIO relationship type</typeparam>
        /// <param name="context">Database context. Should be committed externally</param>
        /// <param name="person">Person object for which the relationships are being merged</param>
        /// <param name="relationType">Type of relationship</param>
        /// <param name="isForward">Relationship direction. Null for those that holds same meaning in any direction (Marriage)</param>
        /// <param name="oioRelationships">Array of OIO relationship objects </param>
        /// <param name="deleteIfExtraInDatabase">True to delete relationships found in database if not found in oioRelationships (Replace). False to keen them (Merge)</param>
        /// <param name="customRelationCreationMethod">Custom method to update the database Relationship object. This could crate a child row (like TimedRelationship...etc)</param>
        private static void MergePersonRelationships<TOioRelation>(
            CPRBrokerDALDataContext context,
            Person person,
            RelationshipType.RelationshipTypes relationType,
            bool? isForward,
            TOioRelation[] oioRelationships,
            bool deleteIfExtraInDatabase,
            Action<TOioRelation, Relationship> customRelationCreationMethod
        )
        where TOioRelation : BaseRelationshipType
        {
            // Base getter delegate definitions            
            DateTime today = DateTime.Today;

            // Get the Relationship objects already in database
            var dbRelations = PersonRelationsByTypeAndDirectionExpression.Compile()(person, relationType, isForward, today);

            // This delegate sets the Person and RelatedPerson properties of the Relationship object
            Action<Relationship, Person> relationPersonSetter = (rel, relatedPerson) =>
            {
                if (!isForward.HasValue || isForward.Value)
                {
                    rel.Person = person;
                    rel.RelatedPerson = relatedPerson;
                }
                else
                {
                    rel.Person = relatedPerson;
                    rel.RelatedPerson = person;
                }
            };

            // Delete extra relations
            if (deleteIfExtraInDatabase)
            {
                var relationsToDelete =
                    (
                    from rel in dbRelations
                    where !(from oioRelation in oioRelationships select oioRelation.SimpleCPRPerson.PersonCivilRegistrationIdentifier).Contains(RelatedDatabasePersonGetter(person, rel, isForward).PersonNumber)
                    select rel
                    );
                context.Relationships.AttachAll(relationsToDelete);
                context.Relationships.DeleteAllOnSubmit(relationsToDelete);
            }

            // Add new related persons
            var oioNewRelations =
                (
                from oioRelationship in oioRelationships
                where !(from rel in dbRelations select RelatedDatabasePersonGetter(person, rel, isForward).PersonNumber).Contains(oioRelationship.SimpleCPRPerson.PersonCivilRegistrationIdentifier)
                select oioRelationship
                );

            foreach (var oioNewRelationship in oioNewRelations)
            {
                var relatedPerson = Person.GetPerson(context, oioNewRelationship.SimpleCPRPerson.PersonCivilRegistrationIdentifier, oioNewRelationship.SimpleCPRPerson.PersonNameStructure, false);
                relatedPerson.UpgradeDetailLevel(DetailLevel.DetailLevelType.Number);
                Relationship newRelationship = new Relationship()
                {
                    RelationshipId = Guid.NewGuid(),
                    RegistrationDate = DateTime.Today,
                    RelationshipTypeId = (int)relationType
                };

                relationPersonSetter(newRelationship, relatedPerson);

                if (customRelationCreationMethod != null)
                {
                    customRelationCreationMethod(oioNewRelationship, newRelationship);
                }
            }
        }

        /// <summary>
        /// Merges a person's relationships with the current person
        /// </summary>
        /// <typeparam name="TOioRelation">Type of OIO relationship</typeparam>
        /// <param name="context">Data context</param>
        /// <param name="person">Person</param>
        /// <param name="relationType">Type of relationship</param>
        /// <param name="isForward">Relationship direction</param>
        /// <param name="oioRelationships">OIO relationship objects</param>
        /// <param name="customRelationCreationMethod">Custom delegate to call for newly created relationship objects</param>
        public static void MergePersonRelationshipsByType<TOioRelation>(
           CPRBrokerDALDataContext context,
           Person person,
           RelationshipType.RelationshipTypes relationType,
           bool? isForward,
           TOioRelation[] oioRelationships,
           Action<TOioRelation, Relationship> customRelationCreationMethod
           )
            where TOioRelation : BaseRelationshipType
        {
            MergePersonRelationships<TOioRelation>(
                context,
                person,
                relationType,
                isForward,
                oioRelationships,
                true,
                customRelationCreationMethod
                );
        }

        /// <summary>
        /// Creates a new relationship in database if not already there
        /// </summary>
        /// <typeparam name="TOioRelation">Type of OIO relationship</typeparam>
        /// <param name="context">Data context</param>
        /// <param name="person">Person</param>
        /// <param name="relationType">type of relationship</param>
        /// <param name="isForward">Relationship direction</param>
        /// <param name="oioRelationships">OIO objects representing the new relationships</param>
        /// <param name="customRelationCreationMethod">Delegate to do a custom action for a new relationship object</param>
        public static void AddPersonRelationshipIfNotExist<TOioRelation>(
           CPRBrokerDALDataContext context,
           Person person,
           RelationshipType.RelationshipTypes relationType,
           bool? isForward,
           TOioRelation[] oioRelationships,
           Action<TOioRelation, Relationship> customRelationCreationMethod
            )
            where TOioRelation : BaseRelationshipType
        {
            MergePersonRelationships<TOioRelation>(
                context,
                person,
                relationType,
                isForward,
                oioRelationships,
                false,
                customRelationCreationMethod
                );
        }

        /// <summary>
        /// Marks a relationship as ended by setting EndDate
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="person">Person</param>
        /// <param name="relationType">Type of relationship</param>
        /// <param name="isForward">Relationship direction</param>
        /// <param name="oioRelatedPersons">OIO objects representing the related persons</param>
        /// <returns>Success or failure if relationship is not found</returns>
        public static bool EndPersonRelationship(
            Person person,
            RelationshipType.RelationshipTypes relationType,
            bool? isForward,
            SimpleCPRPersonType[] oioRelatedPersons
            )
        {
            DateTime today = DateTime.Today;

            var rel = PersonRelationsByTypeAndDirectionExpression.Compile()(person, relationType, isForward, today).FirstOrDefault();
            if (rel == null || rel.TimedRelationship == null)
            {
                return false;
            }
            else
            {
                rel.TimedRelationship.EndDate = today;
                return true;
            }
        }
        #endregion
    }
}
