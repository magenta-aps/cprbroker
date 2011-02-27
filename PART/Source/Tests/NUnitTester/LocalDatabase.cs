using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CPRBroker.Schemas;

namespace NUnitTester
{
    [TestFixture]
    public class LocalDatabase
    {
        Random _Random = new Random();
        string NewRandomPersonNumber()
        {
            return string.Format("{0}{1}", _Random.Next(10000, 99999), _Random.Next(10000, 99999));
        }
        string NewRandomName(string start)
        {
            return string.Format("{0} {1}", start, _Random.Next(10000));
        }

        SimpleCPRPersonType NewRandomSimpleCprPerson()
        {
            return new SimpleCPRPersonType()
                {
                    PersonCivilRegistrationIdentifier = NewRandomPersonNumber(),
                    PersonNameStructure = new PersonNameStructureType()
                    {
                        PersonGivenName = NewRandomName("First"),
                        PersonMiddleName = NewRandomName("Middle"),
                        PersonSurnameName = NewRandomName("Last")
                    }
                };
        }

        T NewRandomRelationship<T>() where T : BaseRelationshipType, new()
        {
            T relation = new T()
            {
                RelationStartDate = DateTime.Now,
                RelationEndDate = null,
                SimpleCPRPerson = NewRandomSimpleCprPerson()
            };
            return relation;
        }

        [Test]
        public void UpdateAndGetRelations()
        {
            PersonRelationsType relations = new PersonRelationsType();
            relations.SimpleCPRPerson = NewRandomSimpleCprPerson();

            relations.Parents.Add(NewRandomRelationship<ParentRelationshipType>());
            relations.Parents.Add(NewRandomRelationship<ParentRelationshipType>());

            for (int i = 0; i < _Random.Next(1, 5); i++)
            {
                relations.Children.Add(NewRandomRelationship<ChildRelationshipType>());
            }

            for (int i = 0; i < _Random.Next(0, 5); i++)
            {
                relations.Spouses.Add(NewRandomRelationship<MaritalRelationshipType>());
            }

            CPRBroker.Engine.Local.UpdateDatabase.UpdateCitizenRelations(relations.SimpleCPRPerson.PersonCivilRegistrationIdentifier, relations);

            CPRBroker.Providers.Local.DatabaseDataProvider prov = new CPRBroker.Providers.Local.DatabaseDataProvider();

            QualityLevel? qualityLevel;
            var dbRelations = prov.GetCitizenRelations("", "", relations.SimpleCPRPerson.PersonCivilRegistrationIdentifier, out qualityLevel);

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(PersonRelationsType));
            System.IO.StringWriter wSrc = new System.IO.StringWriter();
            ser.Serialize(wSrc, relations);

            System.IO.StringWriter wDb = new System.IO.StringWriter();
            ser.Serialize(wDb, relations);

            string xmlSrc = wSrc.ToString();
            string xmlDb = wDb.ToString();

            Console.WriteLine(xmlSrc);
            Console.WriteLine(xmlDb);
            Assert.AreEqual(xmlSrc, xmlDb);


        }
    }
}
