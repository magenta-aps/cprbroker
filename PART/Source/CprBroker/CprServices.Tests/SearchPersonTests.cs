using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.CprServices.Responses;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CprServices
{
    namespace SearchPersonTests
    {
        [TestFixture]
        public class NameMatches
        {
            public class SearchPersonStub : SearchPerson
            {
                public string _Name;
                public SearchPersonStub(string name)
                    : base(null, null)
                {
                    _Name = name;
                }

                public override string ToNameString()
                {
                    return _Name;
                }
            }

            [Test]
            public void NameMatches_Null_True()
            {
                var name = new NavnStrukturType()
                {
                    PersonNameStructure = null
                };
                var searchPerson = new SearchPersonStub("first middle last");
                Assert.True(searchPerson.NameMatches(name));
            }

            [Test]
            public void NameMatches_Middle_True()
            {
                var name = new NavnStrukturType()
                {
                    PersonNameStructure = new PersonNameStructureType()
                    {
                        PersonMiddleName = "Middle"
                    }
                };
                var searchPerson = new SearchPersonStub("first middle last");
                Assert.True(searchPerson.NameMatches(name));
            }

            [Test]
            public void NameMatches_Last_True()
            {
                var name = new NavnStrukturType()
                {
                    PersonNameStructure = new PersonNameStructureType()
                    {
                        PersonSurnameName = "Middle"
                    }
                };
                var searchPerson = new SearchPersonStub("first middle last");
                Assert.True(searchPerson.NameMatches(name));
            }

            [Test]
            public void NameMatches_ArrayOneMatchOneNoMatch_False()
            {
                var names = new NavnStrukturType[]{
                    new NavnStrukturType(){ 
                        PersonNameStructure = new PersonNameStructureType()
                        {
                            PersonMiddleName = "Middle"
                        }
                    },
                    new NavnStrukturType(){ 
                        PersonNameStructure = new PersonNameStructureType()
                        {
                            PersonMiddleName = "dummy"
                        }
                    }
                };
                var searchPerson = new SearchPersonStub("first middle last");
                Assert.False(searchPerson.NameMatches(names));
            }

        }
    }
}
