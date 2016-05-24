using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Tests.PartInterface;
using CprBroker.PartInterface;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect.Partials
{
    [TestFixture]
    public class ReadTests : TestBase
    {
        public class DummyCprDirectClientDataProvider : CPRDirectClientDataProvider, IPartPersonMappingDataProvider
        {
            public override IndividualResponseType GetPerson(string cprNumber)
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var pers = all.Where(p => p.PersonInformation.PNR == cprNumber).First();
                PartialRecordTests.ClearOptionalRecords(pers);
                return pers;
            }
            public override DataProviderConfigPropertyInfo[] ConfigurationKeys
            {
                get
                {
                    return new DataProviderConfigPropertyInfo[] { };
                }
            }

            public static UuidCache UuidCache = new UuidCache()
            {
                GetUuidMethod = (pnr) => Guid.NewGuid(),
                GetUuidArrayMethod = (pnrs) => pnrs.Select(pnr => Guid.NewGuid() as Guid?).ToArray(),
            };

            public Guid? GetPersonUuid(string cprNumber)
            {
                return Guid.NewGuid();
            }

            public Guid?[] GetPersonUuidArray(string[] cprNumberArray)
            {
                return cprNumberArray.Select(pnr => UuidCache.GetUuid(pnr) as Guid?).ToArray();
            }

        }

        [Test]
        public void Read_OK([ValueSource(typeof(Utilities), "PNRs")]string pnr)
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            var pers = all.Where(p => p.PersonInformation.PNR == pnr).First();

            base.InitLogging();
            CprBroker.Schemas.QualityLevel? ql;
            base.RegisterDataProviderType<DummyCprDirectClientDataProvider>(true);
            base.RegisterDataProviderType<CprBroker.Engine.Local.AdminDataProvider>(false);

            var dpRet = new AdminManager().SetDataProviderList("", CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), new Schemas.DataProviderType[]{
                new Schemas.DataProviderType(){ Attributes = new Schemas.AttributeType[]{}, Enabled = true, TypeName = typeof(DummyCprDirectClientDataProvider).AssemblyQualifiedName}
            });

            var mgr = new CprBroker.PartInterface.PartManager();
            var uuid = mgr.GetUuid("", CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), pnr);
            var ret = mgr.Read("",
                CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                new Schemas.Part.LaesInputType() { UUID = uuid.UUID },
                 Schemas.SourceUsageOrder.LocalThenExternal,
                 out ql
                );

            var registration = ret.LaesResultat.Item as RegistreringType1;
            Assert.NotNull(registration);
            registration.OrderByStartDate(false);
            Assert.NotNull(registration.AttributListe, "No attributes");
            
            var borgerType = (CprBorgerType)registration.AttributListe.RegisterOplysning[0].Item;

            if (borgerType.PersonNummerGyldighedStatusIndikator) // The CPR number is still active
            {
                Assert.IsNotNull(borgerType.PersonCivilRegistrationIdentifier);
                StringAssert.AreEqualIgnoringCase(pnr, borgerType.PersonCivilRegistrationIdentifier);
                
                if (pers.PersonInformation.Status != 70 && pers.PersonInformation.Status != 80 && pers.PersonInformation.Status != 90) // Not disappeared, emigrated or dead
                    Assert.IsNotNull(borgerType.FolkeregisterAdresse);
            }
            if (!PartialRecordTests.PersonsWithoutNames.Contains(pnr)) // two persons with no current name
                Assert.False(registration.AttributListe.Egenskab[0].NavnStruktur.PersonNameStructure.IsEmpty, "Name not found");
            Assert.IsNotNull(registration.TilstandListe.LivStatus, "Life status not found");
        }
    }
}
