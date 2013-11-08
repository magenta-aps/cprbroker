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
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR.PersonTotalTests.Converters
{
    [TestFixture]
    public class ToBirthdate
    {
        [Test]
        public void ToBirthdate_Date_Correct()
        {
            var date = DateTime.Today.AddDays(1);
            var total = new PersonTotal() { PNR = Utilities.RandomCprNumber(), DateOfBirth = decimal.Parse(date.ToString("yyyyMMdd")) };
            var result = total.ToBirthdate();
            Assert.AreEqual(date, result);
        }

        [Test]
        public void ToBirthdate_NoDate_FromPNR()
        {
            var date = DateTime.Today.AddDays(-100);
            var total = new PersonTotal() { PNR = decimal.Parse(date.ToString("ddMMyy4234")), DateOfBirth = 0 };
            var result = total.ToBirthdate();
            Assert.AreEqual(date, result);
        }

        [Test]
        public void ToBirthdate_NoDateOrPNR_Null(
            [Values(121208, 0)] decimal pnr)
        {
            var total = new PersonTotal() { PNR = pnr, DateOfBirth = 0 };
            var result = total.ToBirthdate();
            Assert.Null(result);
        }
    }

    [TestFixture]
    public class ToChurchMembershipIndicator
    {
        [Test]
        public void ToChurchMembershipIndicator_F_ReturnsTrue(
            [Values('F', 'f')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.True(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_NonF_ReturnsFalse(
            [Values('A', 'a', 'U', 'u', 'M', 'm', 'S', 's')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.False(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_OtherValues_ReturnsFalse(
            [Values(null, 'w', '2', 'w', 'p')]char? churchStatus)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = churchStatus };
            var result = personTotal.ToChurchMembershipIndicator();
            Assert.False(result);
        }
    }


    [TestFixture]
    public class ToCivilStatusCodeType
    {
        private Separation CreateSeparation(bool create)
        {
            if (create)
            {
                return new Separation();
            }
            else
            {
                return null;
            }
        }
        [Test]
        public void ToCivilStatusCodeType_U_NotMarried(
            [Values('u', 'U')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.Ugift, result);
        }

        [Test]
        public void ToCivilStatusCodeType_G_NoSeparation_Married(
            [Values('g', 'G')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(null);
            Assert.AreEqual(CivilStatusKodeType.Gift, result);
        }

        [Test]
        public void ToCivilStatusCodeType_G_InActiveSeparation_Married(
            [Values('g', 'G')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var separation = new Separation() { EndDate = DateTime.Today };
            var result = personTotal.ToCivilStatusCodeType(separation);
            Assert.AreEqual(CivilStatusKodeType.Gift, result);
        }

        [Test]
        public void ToCivilStatusCodeType_GActiveSeparation_Separated(
            [Values('g', 'G')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var separation = new Separation() { EndDate = null };
            var result = personTotal.ToCivilStatusCodeType(separation);
            Assert.AreEqual(CivilStatusKodeType.Separeret, result);
        }

        [Test]
        public void ToCivilStatusCodeType_F_Divorced(
            [Values('f', 'F')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.Skilt, result);
        }

        [Test]
        public void ToCivilStatusCodeType_E_Widow(
            [Values('e', 'E')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.Enke, result);
        }

        [Test]
        public void ToCivilStatusCodeType_P_NoSeparation_RegPartner(
            [Values('p', 'P')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(null);
            Assert.AreEqual(CivilStatusKodeType.RegistreretPartner, result);
        }

        [Test]
        public void ToCivilStatusCodeType_P_InActiveSeparation_RegPartner(
            [Values('p', 'P')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var separation = new Separation() { EndDate = DateTime.Today };
            var result = personTotal.ToCivilStatusCodeType(separation);
            Assert.AreEqual(CivilStatusKodeType.RegistreretPartner, result);
        }

        [Test]
        public void ToCivilStatusCodeType_P_ActiveSeparation_RegPartner(
            [Values('p', 'P')]char maritalStatus)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var separation = new Separation() { EndDate = null };
            var result = personTotal.ToCivilStatusCodeType(separation);
            Assert.AreEqual(CivilStatusKodeType.Separeret, result);
        }

        [Test]
        public void ToCivilStatusCodeType_U_AbolitionOfRegisteredPartnership(
            [Values('o', 'O')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.OphaevetPartnerskab, result);
        }

        [Test]
        public void ToCivilStatusCodeType_L_LongestLivingPartner(
            [Values('l', 'L')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.Laengstlevende, result);
        }

        [Test]
        public void ToCivilStatusCodeType_D_DeadIsNotMarried(
            [Values('d', 'D')]char maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            var result = personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
            Assert.AreEqual(CivilStatusKodeType.Ugift, result);
        }

        [Test]
        [ExpectedException]
        public void ToCivilStatusCodeType_OtherValues_ThrowsException(
            [Values(null, '1', 'q', 'A', 'T', 'w')]char? maritalStatus,
            [Values(true, false)] bool hasSeparation)
        {
            var personTotal = new PersonTotalStub() { MaritalStatus = maritalStatus };
            personTotal.ToCivilStatusCodeType(CreateSeparation(hasSeparation));
        }
    }

    [TestFixture]
    public class ToLivStatusKodeType : BaseTests
    {
        [Test]
        public void ToLivStatusKodeType_CorrectValues_ReturnsCorrect(
            [Values(1, 3, 5, 7, 20, 30, 50, 60, 70, 80, 90)]decimal status,
            [ValueSource("RandomDecimalDates5")]decimal decimalBirthDate)
        {
            var personTotal = new PersonTotalStub() { Status = status, DateOfBirth = decimalBirthDate };
            var result = personTotal.ToLivStatusKodeType();
            var birthDate = Providers.DPR.Utilities.DateFromDecimal(decimalBirthDate);
            Assert.AreEqual(Schemas.Util.Enums.ToLifeStatus(status, birthDate), result);
        }

        [Test]
        public void ToLivStatusKodeType_CorrectValuesWithZeroBirthDateBirthDate_ThrowsException(
            [Values(1, 3, 5, 7, 20, 30, 50, 60, 70, 80, 90)]decimal status)
        {
            var personTotal = new PersonTotalStub() { Status = status, DateOfBirth = 0 };
            var result = personTotal.ToLivStatusKodeType();
            Assert.AreEqual(Schemas.Util.Enums.ToLifeStatus(status, null), result);
        }

        [Test]
        [ExpectedException]
        public void ToLivStatusKodeType_WrongValues_ThrowsException(
            [Values(12, -22, 58, 111, 0)]decimal status)
        {
            var personTotal = new PersonTotalStub() { Status = status };
            personTotal.ToLivStatusKodeType();
        }
    }

    [TestFixture]
    public class ToDirectoryProtectionIndicator
    {
        [Test]
        public void ToDirectoryProtectionIndicator_1_True()
        {
            var personTotal = new PersonTotalStub() { DirectoryProtectionMarker = '1' };
            var result = personTotal.ToDirectoryProtectionIndicator();
            Assert.True(result);
        }

        [Test]
        public void ToDirectoryProtectionIndicator_OtherValues_False(
            [Values(null, '2', '0', 'w', 'A')]char? protectionIndicator)
        {
            var personTotal = new PersonTotalStub() { DirectoryProtectionMarker = protectionIndicator };
            var result = personTotal.ToDirectoryProtectionIndicator();
            Assert.False(result);
        }
    }

    [TestFixture]
    public class ToCivilRegistrationValidityStatusIndicator
    {
        [Test]
        public void ToCivilRegistrationValidityStatusIndicator_ActiveValues_ReturnsTrue(
            [Values(1, 3, 5, 7, 20, 70, 80, 90)]decimal status)
        {
            var personTotal = new PersonTotalStub() { Status = status };
            var result = personTotal.ToCivilRegistrationValidityStatusIndicator();
            Assert.True(result);
        }

        [Test]
        public void ToCivilRegistrationValidityStatusIndicator_InActiveValues_ReturnsFalse(
            [Values(30, 50, 60)]decimal status)
        {
            var personTotal = new PersonTotalStub() { Status = status };
            var result = personTotal.ToCivilRegistrationValidityStatusIndicator();
            Assert.False(result);
        }

        [Test]
        [ExpectedException]
        public void ToCivilRegistrationValidityStatusIndicator_WrongValues_ThrowsException(
            [Values(12, -22, 58, 111, 0)]decimal status)
        {
            var personTotal = new PersonTotalStub() { Status = status };
            personTotal.ToLivStatusKodeType();
        }
    }

    [TestFixture]
    public class ToAddressProtectionIndicator
    {
        [Test]
        public void ToAddressProtectionIndicator_1_True()
        {
            var personTotal = new PersonTotalStub() { AddressProtectionMarker = '1' };
            var result = personTotal.ToAddressProtectionIndicator();
            Assert.True(result);
        }

        [Test]
        public void ToAddressProtectionIndicator_OtherValues_False(
            [Values(null, '2', '0', 'w', 'A')]char? protectionIndicator)
        {
            var personTotal = new PersonTotalStub() { AddressProtectionMarker = protectionIndicator };
            var result = personTotal.ToAddressProtectionIndicator();
            Assert.False(result);
        }

    }





}

