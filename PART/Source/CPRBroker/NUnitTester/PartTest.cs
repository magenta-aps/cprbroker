using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using CprBroker.NUnitTester.Part;

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class PartTest : BaseTest
    {
        #region Part laesResultat methods

        private void Validate(Guid uuid, LaesOutputType laesOutput, Part.Part service)
        {
            Assert.IsNotNull(laesOutput, "Laes output is null{0}", uuid);
            Validate(uuid, laesOutput.LaesResultat, service);
        }

        private void Validate(Guid uuid, LaesResultatType laesResultat, Part.Part service)
        {
            Assert.IsNotNull(laesResultat, "Person not found : {0}", uuid);
            if (laesResultat.Item is RegistreringType1)
            {
                var reg = laesResultat.Item as RegistreringType1;
                Assert.AreNotEqual("", reg.AktoerTekst, "Empty actor text");
                Assert.AreNotEqual(Guid.Empty.ToString(), reg.AktoerTekst, "Empty actor text");

                Assert.NotNull(reg.AttributListe, "Attributes");
                Assert.NotNull(reg.AttributListe.Egenskaber, "Attributes");
                Assert.Greater(reg.AttributListe.Egenskaber.Length, 0, "Attributes");

                Assert.NotNull(reg.AttributListe.Egenskaber[0].PersonBirthDateStructure, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskaber[0].PersonNameStructure, "Name");
                //Assert.NotNull(reg.AttributListe.Egenskaber[0].RegisterOplysninger, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskaber[0].Virkning, "Effect");

                Assert.NotNull(reg.TilstandListe, "States");

                Assert.NotNull(reg.RelationListe, "Relations");
            }
            else
            {
                Assert.Fail("Unknown laesResultat object type");
            }

            //Assert.AreNotEqual(String.Empty, laesResultat.LaesResultat..ActorId);

            Assert.IsNotNull(service.QualityHeaderValue, "Quality header");
            Assert.IsNotNull(service.QualityHeaderValue.QualityLevel, "Quality header value");
        }
        #endregion

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_GetPersonUuid(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            var uuid2 = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreEqual(uuid, uuid2);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_Read(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            LaesInputType input = new LaesInputType()
            {
                UUID = uuid.ToString(),
            };
            var person = TestRunner.PartService.Read(input);
            Validate(uuid, person, TestRunner.PartService);
        }

        
        [Test]
        [TestCaseSource(typeof(TestData), TestData.EmptyReadInputFieldName)]
        public void T210_Read_Empty(LaesInputType inp)
        {
            var person = TestRunner.PartService.Read(inp);
            Assert.IsNotNull(person);
            Assert.IsNotNull(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.InvalidReadInputFieldName)]
        public void T220_Read_InvalidUuid(string uuid)
        {
            LaesInputType input = new LaesInputType()
            {
                UUID = uuid,
            };
            var person = TestRunner.PartService.Read(input);
            Assert.IsNotNull(person);
            Assert.IsNotNull(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.NonExistingUuidReadInputFieldName)]
        public void T220_Read_NonexistingUuid(string uuid)
        {
            LaesInputType input = new LaesInputType()
            {
                UUID = uuid,
            };
            var person = TestRunner.PartService.Read(input);
            Assert.IsNotNull(person);
            Assert.IsNotNull(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }


        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T250_RefreshRead(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            LaesInputType input = new LaesInputType()
            {
                UUID = uuid.ToString(),
            };

            // Call read to ensure laesResultat is actually in the database
            var person = TestRunner.PartService.Read(input);
            Assert.NotNull(person);
            Assert.NotNull(person.LaesResultat);

            var freshPerson = TestRunner.PartService.RefreshRead(input);
            Validate(uuid, freshPerson, TestRunner.PartService);

            Assert.AreNotEqual(TestRunner.PartService.QualityHeaderValue.QualityLevel.Value, Part.QualityLevel.LocalCache);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T300_List_Single(string cprNumber)
        {
            string[] cprNumbers = new string[] { cprNumber };
            ListInputType input = new ListInputType()
            {
                UUID = Array.ConvertAll<string, string>(cprNumbers, (cpr) => TestRunner.PartService.GetPersonUuid(cpr).ToString()),
            };

            var persons = TestRunner.PartService.List(input);
            Assert.IsNotNull(persons, "Persons array is null");
            Assert.AreEqual(cprNumbers.Length, persons.LaesResultat.Length, "Incorrect length of returned array");
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T310_List_All(string[] cprNumbers)
        {
            ListInputType input = new ListInputType()
            {
                UUID = cprNumbers == null ? null : Array.ConvertAll<string, string>(cprNumbers, (cpr) => TestRunner.PartService.GetPersonUuid(cpr).ToString()),
            };

            var persons = TestRunner.PartService.List(input);
            
            Assert.IsNotNull(persons);
            
            if (cprNumbers != null)
            {
                Assert.IsNotNull(persons, "Persons array is null");
                Assert.AreEqual(cprNumbers.Length, persons.LaesResultat.Length, "Incorrect length of returned array");
                for (int i = 0; i < cprNumbers.Length; i++)
                {
                    Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T400_Search_CprNumber(string cprNumber)
        {
            var personUuid = TestRunner.PartService.GetPersonUuid(cprNumber);

            var searchCriteria = new Part.SoegInputType1()
            {
                Soeg = new SoegObjektType()
                {
                    BrugervendtNoegleTekst = cprNumber
                }
            };

            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.NotNull(result, "Search result");
            Assert.IsNotNull(result.IdListe, "Search result ids");

            Assert.AreEqual(1, result.IdListe.Length, "Number of search results");
            Assert.AreNotEqual(Guid.Empty, result.IdListe[0], "Empty laesResultat uuid from search");
            Assert.AreEqual(personUuid.ToString(), result.IdListe[0], "Search result returns wrong uuids");
        }

        //[Test]
        //[TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T410_Search_Name(string cprNumber)
        {
            var personUuid = TestRunner.PartService.GetPersonUuid(cprNumber);

            LaesInputType input = new LaesInputType()
            {
                UUID = personUuid.ToString(),
            };
            var personObject = TestRunner.PartService.Read(input);

            var pp = personObject.LaesResultat.Item as Part.RegistreringType1;
            Schemas.PersonNameStructureType name = new CprBroker.Schemas.PersonNameStructureType()
            {
                PersonGivenName = pp.AttributListe.Egenskaber[0].PersonNameStructure.PersonGivenName,
                PersonMiddleName = pp.AttributListe.Egenskaber[0].PersonNameStructure.PersonMiddleName,
                PersonSurnameName = pp.AttributListe.Egenskaber[0].PersonNameStructure.PersonSurnameName,
            };

            var searchCriteria = new Part.SoegInputType1()
            {
                Soeg = new SoegObjektType()
                {
                    Attributter = new SoegAttributListeType()
                    {
                        SoegEgenskab = new SoegEgenskabType[]
                        {
                            new SoegEgenskabType()
                            {
                                NavnTekst = name.ToString()
                            }
                        }
                    }
                }
            };

            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.NotNull(result, "Search result");
            Assert.IsNotNull(result.IdListe, "Search result ids");

            Assert.AreEqual(1, result.IdListe.Length, "Number of search results");
            Assert.AreNotEqual(Guid.Empty, result.IdListe[0], "Empty laesResultat uuid from search");
            Assert.AreEqual(personUuid.ToString(), result.IdListe[0], "Search result returns wrong uuids");
        }

        // TODO: Add more methods to test Search for criteria other than CPR number


        //[Test]
        //[TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T500_Database_Update(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            LaesInputType input = new LaesInputType()
            {
                UUID = uuid.ToString(),
            };
            var fresh = TestRunner.PartService.RefreshRead(input);
            Validate(uuid, fresh, TestRunner.PartService);
            Assert.AreNotEqual(Part.QualityLevel.LocalCache, TestRunner.PartService.QualityHeaderValue.QualityLevel.Value);

            var cached = TestRunner.PartService.Read(input);
            Validate(uuid, cached, TestRunner.PartService);
            Assert.AreEqual(Part.QualityLevel.LocalCache, TestRunner.PartService.QualityHeaderValue.QualityLevel.Value);

            // Now validate contents
            Assert.True(Utilities.AreEqual<Part.RegistreringType1>(fresh.LaesResultat.Item as Part.RegistreringType1, cached.LaesResultat.Item as Part.RegistreringType1), "Equal response from data provider and local cache");
        }





    }
}
