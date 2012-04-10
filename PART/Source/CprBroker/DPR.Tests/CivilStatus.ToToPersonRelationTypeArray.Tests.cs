using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DPR.CivilStatusTests
{
    [TestFixture]
    public class ToToPersonRelationTypeArray : BaseTests
    {
        public char[] AllMaritalStates = new char[] { 'U', 'G', 'F', 'D', 'E', 'P', 'O', 'L', 'u', 'g', 'f', 'd', 'e', 'p', 'o', 'l' };
        [Test]
        [ExpectedException]
        [Ignore]
        public void ToToPersonRelationTypeArray_Null_ThrowsException()
        {
            CivilStatus.ToToPersonRelationTypeArray(null, UuidMap.CprToUuid, 'U', 'G','W');
        }

        [Test]
        [Ignore]
        public void ToToPersonRelationTypeArray_CorrectStatus_Returns2(
            [ValueSource("AllMaritalStates")] char maritalStatus)
        {
            var civilStates = AllMaritalStates.Select(c => new CivilStatusStub() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = c } as CivilStatus).ToArray();
            var result = CivilStatus.ToToPersonRelationTypeArray(civilStates, UuidMap.CprToUuid, maritalStatus,maritalStatus,maritalStatus);
            // 2 are expected because of upper and lower cases
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        [Ignore]
        public void ToToPersonRelationTypeArray_NoSpouse_ReturnsEmpty(
            [ValueSource("AllMaritalStates")] char maritalStatus)
        {
            var civilStates = AllMaritalStates.Select(c => new CivilStatusStub() { SpousePNR = 0, MaritalStatus = c } as CivilStatus).ToArray();
            var result = CivilStatus.ToToPersonRelationTypeArray(civilStates, UuidMap.CprToUuid, maritalStatus,maritalStatus,maritalStatus);            
            Assert.AreEqual(0, result.Length);
        }
    }
}
