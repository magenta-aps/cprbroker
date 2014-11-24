using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CprServices;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CprServices
{
    namespace SearchPlanTests
    {
        [TestFixture]
        public class _SearchPlan
        {
            [Test]
            public void SearchPlan_AllEmpty_True()
            {
                var p = new SearchPlan(new SearchRequest(new SoegAttributListeType()));
                Assert.True(p.IsSatisfactory);
            }

            [Test]
            public void SearchPlan_Criteria_NoMethods_False()
            {
                var req = new SearchRequest(new SoegAttributListeType());
                req.AddCriteriaField("DDD", "jkqhd");
                var p = new SearchPlan(req);
                Assert.False(p.IsSatisfactory);
            }

            [Test]
            public void SearchPlan_Criteria_MethodNotFulfilled_False()
            {
                var req = new SearchRequest(new SoegAttributListeType());
                req.AddCriteriaField("DDD", "jkqhd");
                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DDD", Required = false });
                m.InputFields.Add(new InputField() { Name = "ddqw", Required = true });

                var p = new SearchPlan(req, m);
                Assert.False(p.IsSatisfactory);
            }

            [Test]
            public void SearchPlan_NoCriteria_ExtraExpectedOptionalInput_True()
            {
                var req = new SearchRequest(new SoegAttributListeType());
                
                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DDD", Required = false });
                m.InputFields.Add(new InputField() { Name = "ddqw", Required = false });

                var p = new SearchPlan(req, m);
                Assert.True(p.IsSatisfactory);
            }

            [Test]
            public void SearchPlan_NoCriteria_ExtraExpectedOptionalInput_HasCalls()
            {
                var req = new SearchRequest(new SoegAttributListeType());

                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DDD", Required = false });
                m.InputFields.Add(new InputField() { Name = "ddqw", Required = false });

                var p = new SearchPlan(req, m);
                Assert.IsNotEmpty(p.PlannedCalls);
            }
        }
    }
}
