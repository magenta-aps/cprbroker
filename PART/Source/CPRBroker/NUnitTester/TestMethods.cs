using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    [NUnit.Framework.TestFixture]
    public class TestWebServiceMethods
    {
        private string appToken = "1de09be5-8382-4dc1-a89b-8f86fc67f97a";
        private string userToken = "peter";
        string cprNumber = "123";
        string cprChildNumber = "123456";
        string authorityCode = "AAAA";

        private NUnitTester.CPRPersonWS.CPRPersonWS CreatePersonWebService()
        {
            CPRPersonWS.CPRPersonWS person = new NUnitTester.CPRPersonWS.CPRPersonWS();
            person.ApplicationHeaderValue = new NUnitTester.CPRPersonWS.ApplicationHeader()
            {
                ApplicationToken = appToken,
                UserToken = userToken
            };
            return person;
        }

        [NUnit.Framework.Test]
        public void TestGetBasicCitizen()
        {
            CPRPersonWS.PersonBasicStructureType output = null;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            Console.WriteLine(person.Url);
            output = person.GetCitizenBasic(cprNumber);
            Assert.IsNotNull(output);
        }

        [NUnit.Framework.Test]
        public void TestGetCitizenNameAndAddress()
        {
            CPRPersonWS.PersonNameAndAddressStructureType output = null;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.GetCitizenNameAndAddress(cprNumber);
            Assert.IsNotNull(output);
        }

        [NUnit.Framework.Test]
        public void TestGetCitizenFull()
        {
            object output = string.Empty; ;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.GetCitizenFull(cprNumber);
            Assert.IsNotNull(output);
        }

        //[NUnit.Framework.Test]
        //public void TestGetCitizenRelations()
        //{
        //    List<CPRPersonWS.BaseRetationshipType> list = new List<NUnitTester.CPRPersonWS.BaseRetationshipType>();
        //    CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
        //    list.AddRange(person.GetCitizenRelations(cprNumber));
        //    Assert.Greater(list.Count, 0);
        //}

        [NUnit.Framework.Test]
        public void TestGetCitizenChildren()
        {
            CPRPersonWS.SimpleCPRPersonType[] output;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.GetCitizenChildren(cprNumber,false);
            Assert.Greater(output.Count(), 0);
        }

        [NUnit.Framework.Test]
        public void TestRemoveParentAuthorityOverChild()
        {
            bool output;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.RemoveParentAuthorityOverChild(cprNumber, cprChildNumber);
            Assert.IsTrue(output);
        }

        [NUnit.Framework.Test]
        public void TestSetParentAuthorityOverChild()
        {
            bool output;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.SetParentAuthorityOverChild(cprNumber, cprChildNumber);
            Assert.IsTrue(output);
        }

        [NUnit.Framework.Test]
        public void TestGetParentAuthorityOverChildChanges()
        {
            CPRPersonWS.ParentAuthorityRelationshipType[] output;
            CPRPersonWS.CPRPersonWS person = CreatePersonWebService();
            output = person.GetParentAuthorityOverChildChanges(cprNumber);
            Assert.IsNotNull(output);
        }
    }
}
