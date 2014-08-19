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
using NUnit.Framework;
using NUnit.Framework.Constraints;
using CprBroker.NUnitTester.Part;

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class PartTest : PartBaseTest
    {

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_GetUuid(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetUuid(cprNumber);
            Validate(uuid);

            var uuid2 = TestRunner.PartService.GetUuid(cprNumber);
            Validate(uuid2);
            Assert.AreEqual(uuid.UUID, uuid2.UUID);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.InvalidCprNumbersFieldName)]
        public void T210_GetUuid_Invalid(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetUuid(cprNumber);
            ValidateInvalid(uuid);
        }

        [Test]
        public void T220_GetUuidArray([Values(1, 10, 100, 200, 300, 400, 500)]int count)
        {
            string[] cprNumberArray = Utilities.RandomCprNumbers(count);
            var uuids = new string[cprNumberArray.Length];
            uuids = TestRunner.PartService.GetUuidArray(cprNumberArray).UUID;
            for (int i = 0; i < cprNumberArray.Length; i++)
            {
                var rrr = TestRunner.PartService.GetUuid(cprNumberArray[i]);
                uuids[i] = rrr.UUID;
            }


            var uuidBatch = TestRunner.PartService.GetUuidArray(cprNumberArray);
            Validate(uuidBatch.StandardRetur);
            Assert.AreEqual(uuids.Length, uuidBatch.UUID.Length);
            for (int i = 0; i < cprNumberArray.Length; i++)
            {
                Assert.AreEqual(uuids[i], uuidBatch.UUID[i]);
            }
        }

        [Test]
        public void T220_GetUuidArray_MixedExistingAndNew_AllFound([Values(1, 10, 100, 200, 300, 400, 500)]int newCount)
        {
            List<string> cprNumberArray = new List<string>();
            cprNumberArray.AddRange(TestData.cprNumbers);
            cprNumberArray.AddRange(Utilities.RandomCprNumbers(newCount));

            var uuidBatch = TestRunner.PartService.GetUuidArray(cprNumberArray.ToArray());
            Validate(uuidBatch.StandardRetur);
            Assert.AreEqual(cprNumberArray.Count, uuidBatch.UUID.Length);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T300_Read(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetUuid(cprNumber);
            Validate(uuid);

            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = uuid.UUID,
            };
            LaesOutputType person = TestRunner.PartService.Read(input);
            Validate(new Guid(uuid.UUID), person, TestRunner.PartService);
        }


        [Test]
        [TestCaseSource(typeof(TestData), TestData.EmptyReadInputFieldName)]
        public void T310_Read_Empty(Part.LaesInputType inp)
        {
            var person = TestRunner.PartService.Read(inp);
            Assert.IsNotNull(person);
            ValidateInvalid(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.InvalidReadInputFieldName)]
        public void T320_Read_InvalidUuid(string uuid)
        {
            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = uuid,
            };
            TestRunner.PartService.SourceUsageOrderHeaderValue = new SourceUsageOrderHeader() { SourceUsageOrder = SourceUsageOrder.LocalThenExternal };
            var person = TestRunner.PartService.Read(input);
            Assert.IsNotNull(person);
            ValidateInvalid(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.NonExistingUuidReadInputFieldName)]
        public void T330_Read_NonexistingUuid(string uuid)
        {
            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = uuid,
            };
            var person = TestRunner.PartService.Read(input);
            Assert.IsNotNull(person);
            ValidateInvalid(person.StandardRetur);
            Assert.IsNull(person.LaesResultat);
        }


        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T350_RefreshRead(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetUuid(cprNumber);
            Validate(uuid);

            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = uuid.UUID,
            };
            var freshPerson = TestRunner.PartService.RefreshRead(input);
            Validate(new Guid(uuid.UUID), freshPerson, TestRunner.PartService);

            //Assert.AreNotEqual(TestRunner.PartService.QualityHeaderValue.QualityLevel.Value, Part.QualityLevel.LocalCache);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T400_List_Single(string cprNumber)
        {
            string[] cprNumbers = new string[] { cprNumber };
            Part.ListInputType input = new Part.ListInputType()
            {
                UUID = Array.ConvertAll<string, string>(cprNumbers, (cpr) => TestRunner.PartService.GetUuid(cpr).UUID),
            };

            var persons = TestRunner.PartService.List(input);
            Assert.IsNotNull(persons, "List response is null");
            Validate(persons.StandardRetur);
            Assert.IsNotNull(persons.LaesResultat, "Persons array is null");
            Assert.AreEqual(cprNumbers.Length, persons.LaesResultat.Length, "Incorrect length of returned array");
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T405_List_SingleWithPartial(string cprNumber)
        {
            string[] cprNumbers = new string[] { cprNumber };
            var uuids = new List<string>(Array.ConvertAll<string, string>(cprNumbers, (cpr) => TestRunner.PartService.GetUuid(cpr).UUID));
            uuids.Add(Guid.NewGuid().ToString());
            Part.ListInputType input = new Part.ListInputType()
            {
                UUID = uuids.ToArray(),
            };

            var persons = TestRunner.PartService.List(input);
            Assert.IsNotNull(persons, "List response is null");
            Assert.AreEqual("206", persons.StandardRetur.StatusKode);
            Assert.IsNotNull(persons.LaesResultat, "Persons array is null");
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
            }
            Assert.IsNull(persons.LaesResultat[1].Item);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T410_List_All(string[] cprNumbers)
        {
            Part.ListInputType input = new Part.ListInputType()
            {
                UUID = cprNumbers == null ? null : Array.ConvertAll<string, string>(cprNumbers, (cpr) => GetUuid(cpr).UUID),
            };

            var persons = TestRunner.PartService.List(input);

            Assert.IsNotNull(persons);
            if (cprNumbers != null)
            {
                Validate(persons.StandardRetur);
                Assert.IsNotNull(persons, "List response is null");
                Assert.IsNotNull(persons.LaesResultat, "Persons array is null");
                Assert.AreEqual(cprNumbers.Length, persons.LaesResultat.Length, "Incorrect length of returned array");
                for (int i = 0; i < cprNumbers.Length; i++)
                {
                    Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
                }
            }
            else
            {
                ValidateInvalid(persons.StandardRetur);
                Assert.IsNull(persons.LaesResultat);
            }
        }

        Dictionary<string, GetUuidOutputType> GetUuidResults = new Dictionary<string, GetUuidOutputType>();
        GetUuidOutputType GetUuid(string cpr)
        {
            if (GetUuidResults.ContainsKey(cpr))
                return GetUuidResults[cpr];
            else
            {
                var ret = TestRunner.PartService.GetUuid(cpr);
                GetUuidResults[cpr] = ret;
                return ret;
            }
        }
        [Test]
        public void T410_List_Mixed(
            [Values(1, 10, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000)] int count)
        {
            var uuids = new List<string>();
            for (int i = 0; i < count; i++)
            {
                uuids.Add(Guid.NewGuid().ToString());
            }
            uuids.AddRange(Array.ConvertAll<string, string>(TestData.cprNumbers, (cpr) => GetUuid(cpr).UUID));

            Part.ListInputType input = new Part.ListInputType()
            {
                UUID = uuids.ToArray()
            };

            var persons = TestRunner.PartService.List(input);

            Assert.IsNotNull(persons, "List response is null");
            Assert.AreEqual("206", persons.StandardRetur.StatusKode);
            Assert.IsNotNull(persons.LaesResultat, "Persons array is null");
            Assert.AreEqual(uuids.Count, persons.LaesResultat.Length, "Incorrect length of returned array");

            var partCount = persons.StandardRetur.FejlbeskedTekst.Split(',').Length;
            Assert.AreEqual(count, partCount);
            for (int i = 0; i < count; i++)
            {
                Assert.NotNull(persons.LaesResultat[i]);
                Assert.Null(persons.LaesResultat[i].Item);
            }
            for (int i = count; i < uuids.Count; i++)
            {
                Validate(new Guid(input.UUID[i]), persons.LaesResultat[i], TestRunner.PartService);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T500_Search_CprNumber(string cprNumber)
        {
            var personUuid = TestRunner.PartService.GetUuid(cprNumber).UUID;

            var searchCriteria = new Part.SoegInputType1()
            {
                SoegObjekt = new Part.SoegObjektType()
                {
                    BrugervendtNoegleTekst = cprNumber
                }
            };

            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.NotNull(result, "Search result");
            Validate(result.StandardRetur);
            Assert.IsNotNull(result.Idliste, "Search result ids");

            Assert.AreEqual(1, result.Idliste.Length, "Number of search results");
            Assert.AreNotEqual(Guid.Empty, result.Idliste[0], "Empty laesResultat uuid from search");
            Assert.AreEqual(personUuid.ToString(), result.Idliste[0], "Search result returns wrong uuids");
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T510_Search_Name(string cprNumber)
        {
            var personUuid = TestRunner.PartService.GetUuid(cprNumber);
            Validate(personUuid);

            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = personUuid.UUID,
            };
            var personObject = TestRunner.PartService.Read(input);
            Validate(new Guid(personUuid.UUID), personObject, TestRunner.PartService);

            var personRegistration = personObject.LaesResultat.Item as Part.RegistreringType1;


            var searchCriteria = new Part.SoegInputType1()
            {
                SoegObjekt = new Part.SoegObjektType()
                {
                    SoegAttributListe = new Part.SoegAttributListeType()
                    {
                        SoegEgenskab = new Part.SoegEgenskabType[]
                        {
                            new Part.SoegEgenskabType()
                            {
                                NavnStruktur=new Part.NavnStrukturType()
                                {
                                    PersonNameStructure=new Part.PersonNameStructureType()
                                    {
                                        PersonGivenName= personRegistration .AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.PersonGivenName,
                                        PersonMiddleName= personRegistration .AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.PersonMiddleName,
                                        PersonSurnameName= personRegistration .AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.PersonSurnameName,
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.NotNull(result, "Search result");
            Validate(result.StandardRetur);
            Assert.IsNotNull(result.Idliste, "Search result ids");

            Assert.AreEqual(1, result.Idliste.Length, "Number of search results");
            Assert.AreNotEqual(Guid.Empty, result.Idliste[0], "Empty laesResultat uuid from search");
            Assert.AreEqual(personUuid.UUID, result.Idliste[0], "Search result returns wrong uuids");
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.InvalidSearchCriteriaFieldName)]
        public void T520_Search_Invalid(Part.SoegInputType1 searchCriteria)
        {
            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.NotNull(result);
            Assert.IsNull(result.Idliste);
            ValidateInvalid(result.StandardRetur);
        }

        // TODO: Add more methods to test Search for criteria other than CPR number


        //[Test]
        //[TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T600_Database_Update(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetUuid(cprNumber);
            Part.LaesInputType input = new Part.LaesInputType()
            {
                UUID = uuid.UUID.ToString(),
            };
            var fresh = TestRunner.PartService.RefreshRead(input);
            Validate(new Guid(uuid.UUID), fresh, TestRunner.PartService);
            //Assert.AreNotEqual(Part.QualityLevel.LocalCache, TestRunner.PartService.QualityHeaderValue.QualityLevel.Value);

            var cached = TestRunner.PartService.Read(input);
            Validate(new Guid(uuid.UUID), cached, TestRunner.PartService);
            //Assert.AreEqual(Part.QualityLevel.LocalCache, TestRunner.PartService.QualityHeaderValue.QualityLevel.Value);

            // Now validate contents
            Assert.True(Utilities.AreEqual<Part.RegistreringType1>(fresh.LaesResultat.Item as Part.RegistreringType1, cached.LaesResultat.Item as Part.RegistreringType1), "Equal response from data provider and local cache");
        }





    }
}
