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
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Tests.Data
{
    namespace PersonRegistrationTests
    {

        [TestFixture]
        public class CreateXQueryElements
        {
            [Test]
            public void CreateXQueryElements_MunicipalityCode_2([Values("851", "217", "905")] string municipalityCode)
            {
                var soegObject = new SoegObjektType()
                {
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        SoegRegisterOplysning = new RegisterOplysningType[] 
                            { 
                                new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } 
                        }
                    }
                };
                var ret = PersonRegistration.CreateXQueryElements(soegObject);
            }
        }

        [TestFixture]
        public class GetUuidsByCriteria
        {
            [Test]
            public void GetUuidsByCriteria_MunicipalityCode_NotZero([Values("851", "905")] string municipalityCode)
            {
                var soegObject = new SoegObjektType()
                {
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        SoegRegisterOplysning = new RegisterOplysningType[] 
                            { 
                                new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } 
                        }
                    }
                };
                using (var dataContext = new PartDataContext())
                {
                    var ret = PersonRegistrationKey.GetByCriteria(dataContext, soegObject, null);
                    var first = ret.First();
                    Assert.NotNull(first);
                }
            }

            [Test]
            public void GetUuidsByCriteria_MunicipalityCodeAndRandomPersonRegistratioId_Zero([Values("851", "905")] string municipalityCode)
            {
                var soegObject = new SoegObjektType()
                {
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        SoegRegisterOplysning = new RegisterOplysningType[] 
                            { 
                                new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } 
                        }
                    }
                };
                using (var dataContext = new PartDataContext())
                {
                    var ret = PersonRegistrationKey.GetByCriteria(dataContext, soegObject, Guid.NewGuid(), Guid.NewGuid());
                    Assert.IsEmpty(ret.ToArray());
                }
            }

            [Test]
            public void GetUuidsByCriteria_EmptyCriteriaWithRegistratioId_NonZero()
            {
                var soegObject = new SoegObjektType();

                using (var dataContext = new PartDataContext())
                {
                    var ret = PersonRegistrationKey.GetByCriteria(dataContext, soegObject, new Guid("CA7436F1-1D05-476B-BB6A-00005E121624"), new Guid("65D228C1-2F19-43BB-837C-00006CEE3D83"));
                    Assert.IsNotEmpty(ret.ToArray());
                }

            }
        }

        [TestFixture]
        public class GetUuidsByCriteriaAndIndexes
        {
            [Test]
            public void GetUuidsByCriteria_MunicipalityCode_5([Values("851", "905")] string municipalityCode)
            {
                var soegObject = new SoegObjektType()
                {
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        SoegRegisterOplysning = new RegisterOplysningType[] 
                            { 
                                new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } 
                        }
                    }
                };
                using (var dataContext = new PartDataContext())
                {
                    var ret = PersonRegistration.GetUuidsByCriteria(dataContext, soegObject, 0, 5).ToArray();
                    Assert.AreEqual(5, ret.Count());
                    var empty = ret.Where(r => r == null);
                    Assert.IsEmpty(empty);
                }
            }

            [Test]
            public void GetUuidsByCriteria_MunicipalityCodeIndexOutOfRange_Zero([Values("851", "217", "905")] string municipalityCode)
            {
                var soegObject = new SoegObjektType()
                {
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        SoegRegisterOplysning = new RegisterOplysningType[] 
                            { 
                                new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } 
                        }
                    }
                };
                using (var dataContext = new PartDataContext())
                {
                    var ret = PersonRegistration.GetUuidsByCriteria(dataContext, soegObject, 10000000, 10);
                    Assert.AreEqual(0, ret.Count());
                }
            }
        }
    }
}

