using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    class ChurchInformationTests
    {
        [TestFixture]
        public class ToFolkekirkeMedlemIndikator
        {
            [Test]
            public void ToFolkekirkeMedlemIndikator_FM_True(
                [Values('F', 'M', 'f', 'm')]char churchRelation)
            {
                var info = new ChurchInformationType() { ChurchRelationship = churchRelation };
                var ret = info.ToFolkekirkeMedlemIndikator();
                Assert.True( ret);
            }

            [Test]
            public void ToFolkekirkeMedlemIndikator_ASD_False(
                [Values('A', 'S', 'U', 'a', 's', 'u')]char churchRelation)
            {
                var info = new ChurchInformationType() { ChurchRelationship = churchRelation };
                var ret = info.ToFolkekirkeMedlemIndikator();
                Assert.False(ret);
            }

            [Test]
            [ExpectedException]
            public void ToFolkekirkeMedlemIndikator_Other_Exception(
                [Values('p', ' ', '2')]char churchRelation)
            {
                var info = new ChurchInformationType() { ChurchRelationship = churchRelation };
                var ret = info.ToFolkekirkeMedlemIndikator();
            }
        }
    }
}
