using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        private static RelationListeType ToRelationListeType(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType()
            {
                Aegtefaelle = ToSpouses(citizen, cpr2uuidFunc),
                Boern = ToChildren(citizen, cpr2uuidFunc),
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Fader = ToFather(citizen, cpr2uuidFunc),
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                LokalUdvidelse = null,
                Moder = ToMother(citizen, cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(citizen, cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null
            };

            return ret;
        }

        private static PersonRelationType[] ToSpouses(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            //TODO: Revise the logic for start and end dates
            PersonRelationType ret = null;
            switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
            {
                case CivilStatusKodeType.Gift:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), null);
                    break;
                case CivilStatusKodeType.Separeret:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Skilt:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Enke:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                default:
                    return null;
            }
            return new PersonRelationType[] { ret };
        }

        private static PersonFlerRelationType[] ToChildren(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            var children = Converters.ToPersonGenderCodeType(citizen.Gender) == PersonGenderCodeType.male ? citizen.ChildrenAsFather : citizen.ChildrenAsMother;
            return children.Select(child => Child.ToPersonFlerRelationType(child, cpr2uuidFunc)).ToArray();
        }

        private static PersonRelationType[] ToRegisteredPartners(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            //TODO: Revise the logic for start and end dates
            PersonRelationType ret = null;
            switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
            {
                case CivilStatusKodeType.RegistreretPartner:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), null);
                    break;
                case CivilStatusKodeType.OphaevetPartnerskab:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Laengstlevende:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                default:
                    return null;
            }
            return new PersonRelationType[] { ret };
        }

        private static PersonRelationType[] ToFather(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.FatherPNR)));
        }

        private static PersonRelationType[] ToMother(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.MotherPNR)));
        }

        [TestFixture]
        partial class Tests
        {
            private Dictionary<string, Guid> uuids = new Dictionary<string, Guid>();
            [TearDown]
            public void ClearUUIDList()
            {
                uuids.Clear();
            }
            Guid ToUuid(string cprNumber)
            {
                var ret = Guid.NewGuid();
                uuids[cprNumber] = ret;
                return ret;
            }

            public decimal?[] RandomCprNumbers10
            {
                get { return UnitTests.RandomCprNumbers(10); }
            }
            public decimal?[] RandomCprNumbers1
            {
                get { return UnitTests.RandomCprNumbers(1); }
            }

            private void AssertCprNumbers(decimal?[] cprNumbers, PersonFlerRelationType[] result)
            {
                Assert.AreEqual(cprNumbers.Length, result.Length);
                for (int i = 0; i < cprNumbers.Length; i++)
                {
                    var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                    Assert.IsNotNull(result[i]);
                    Assert.IsNotNull(result[i].ReferenceID);
                    Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                    Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
                }
            }

            private void AssertCprNumbers(decimal?[] cprNumbers, PersonRelationType[] result)
            {
                Assert.AreEqual(cprNumbers.Length, result.Length);
                for (int i = 0; i < cprNumbers.Length; i++)
                {
                    var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                    Assert.IsNotNull(result[i]);
                    Assert.IsNotNull(result[i].ReferenceID);
                    Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                    Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
                }
            }

            [Test]
            [Combinatorial]
            public void TestToChildren(
                [Random(0, 100, 10)] int count,
                [Values('M', 'K')] char gender)
            {
                var citizen = new Citizen();
                var childCprNumbers = UnitTests.RandomCprNumbers(count);
                var children = childCprNumbers.Select(pnr => new Child() { PNR = pnr });
                citizen.Gender = gender;
                var childrenEntitySet = gender == 'M' ? citizen.ChildrenAsFather : citizen.ChildrenAsMother;
                childrenEntitySet.AddRange(children);
                var result = Citizen.ToChildren(citizen, ToUuid);
                AssertCprNumbers(childCprNumbers, result);
            }

            DateTime?[] TestDates=new DateTime?[]{null,new DateTime(2010,5,5), new DateTime(2011, 7, 7)};

            [Test]
            [Combinatorial]
            public void TestToSpouses(
                [ValueSource("RandomCprNumbers1")] decimal? cprNumber,
                [Values('G', 'F', 'E')]char? maritalStatus,
                [ValueSource("TestDates")] DateTime? maritalStatusDate,
                [Values('T')]char? maritalStatusDateUncertainty,
                [ValueSource("TestDates")]DateTime? maritalStatusEndDate,
                [Values('T')]char? maritalStatusEndDateUncertainty)
            {
                var citizen = new Citizen()
                {
                    SpousePNR = cprNumber,
                    MaritalStatus = maritalStatus,
                    MaritalStatusTimestamp = maritalStatusDate,
                    MaritalStatusTimestampUncertainty = maritalStatusDateUncertainty,
                    MaritalStatusTerminationTimestamp = maritalStatusEndDate,
                    MaritalStatusTerminationTimestampUncertainty = maritalStatusEndDateUncertainty                    
                };

                var result = Citizen.ToSpouses(citizen, ToUuid);
                AssertCprNumbers(new decimal?[] { cprNumber }, result);
            }


            [Test]
            [Combinatorial]
            public void TestToRegisteredPartners(
                [ValueSource("RandomCprNumbers1")] decimal? cprNumber,
                [Values('P', 'O', 'L')]char? maritalStatus,
                [ValueSource("TestDates")] DateTime? maritalStatusDate,
                [Values('T')]char? maritalStatusDateUncertainty,
                [ValueSource("TestDates")]DateTime? maritalStatusEndDate,
                [Values('T')]char? maritalStatusEndDateUncertainty)
            {
                var citizen = new Citizen()
                {
                    SpousePNR = cprNumber,
                    MaritalStatus = maritalStatus,
                    MaritalStatusTimestamp = maritalStatusDate,
                    MaritalStatusTimestampUncertainty = maritalStatusDateUncertainty,
                    MaritalStatusTerminationTimestamp = maritalStatusEndDate,
                    MaritalStatusTerminationTimestampUncertainty = maritalStatusEndDateUncertainty
                };

                var result = Citizen.ToRegisteredPartners(citizen, ToUuid);
                AssertCprNumbers(new decimal?[] { cprNumber }, result);
            }

            [Test]
            [TestCaseSource("RandomCprNumbers10")]
            public void TestToFather(decimal? cprNumber)
            {
                var citizen = new Citizen();
                citizen.FatherPNR = cprNumber;
                var result = Citizen.ToFather(citizen, ToUuid);
                AssertCprNumbers(new decimal?[] { cprNumber }, result);
            }

            [Test]
            [TestCaseSource("RandomCprNumbers10")]
            public void TestToMother(decimal? cprNumber)
            {
                var citizen = new Citizen();
                citizen.MotherPNR = cprNumber;
                var result = Citizen.ToMother(citizen, ToUuid);
                AssertCprNumbers(new decimal?[] { cprNumber }, result);
            }
        }
    }
}