using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    static class RelationListeTypeTests
    {
        private static Dictionary<string, Guid> UuidMap = new Dictionary<string, Guid>();
        public static Guid CprToUuid(string cpr)
        {
            if (!UuidMap.ContainsKey(cpr))
                UuidMap[cpr] = Guid.NewGuid();
            return UuidMap[cpr];
        }
        static string[] FieldNames = new string[]{
                "Aegtefaelle",
                "Boern",
                "Bopaelssamling",
                "ErstatningAf",
                "ErstatningFor",
                "Fader",
                "Foraeldremyndighedsboern",
                "Foraeldremyndighedsindehaver",
                "Moder",
                "RegistreretPartner",
                "RetligHandleevneVaergeForPersonen",
                "RetligHandleevneVaergemaalsindehaver"
            };

        internal static IPersonRelationType[] GetRelation(this RelationListeType rel, string name)
        {
            return rel.GetType().InvokeMember(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance, null, rel, null) as IPersonRelationType[];
        }

        internal static void SetRelation(this RelationListeType rel, string name, IPersonRelationType[] relations)
        {
            var prop = rel.GetType().GetProperty(name);
            if (relations != null)
            {
                var rels = Array.CreateInstance(prop.PropertyType.GetElementType(), relations.Length);
                for (int i = 0; i < relations.Length; i++)
                {
                    rels.SetValue(relations[i], i);
                }
                prop.SetValue(rel, rels, null);
            }
        }

        internal static IPersonRelationType CreateRelation(this RelationListeType rel, string name)
        {
            var memberType = rel.GetType().GetProperty(name).PropertyType.GetElementType();
            return memberType.InvokeMember(null, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance, null, rel, null) as IPersonRelationType;
        }



        [TestFixture]
        public class AssignUuids
        {
            [Test]
            public void AssignUuids_AllNull_OK()
            {
                new RelationListeType().AssignUuids(CprToUuid);
            }

            [Test]
            public void AssignUuids_NullSpouses_OK(
                [ValueSource(typeof(RelationListeTypeTests), "FieldNames")]string relationName)
            {
                var rel = new RelationListeType();
                rel.SetRelation(relationName, new IPersonRelationType[] { null });
                rel.AssignUuids(CprToUuid);
            }

            [Test]
            [ExpectedException]
            public void AssignUuids_EmptySpousePnr_Exception(
                [ValueSource(typeof(RelationListeTypeTests), "FieldNames")]string relationName)
            {
                var rel = new RelationListeType();
                var subRel = rel.CreateRelation(relationName);
                rel.SetRelation(relationName, new IPersonRelationType[] { subRel });
                rel.AssignUuids(CprToUuid);
            }

            [Test]
            public void AssignUuids_ValidSpousePnr_CorrectUuid(
                [ValueSource(typeof(RelationListeTypeTests), "FieldNames")]string relationName)
            {
                var pnr = Utilities.RandomCprNumber();
                var rel = new RelationListeType();
                var subRel = rel.CreateRelation(relationName);
                subRel.CprNumber = pnr;
                rel.SetRelation(relationName, new IPersonRelationType[] { subRel });
                rel.AssignUuids(CprToUuid);
                Assert.AreEqual(UuidMap[pnr].ToString(), rel.GetRelation(relationName)[0].ReferenceID.Item);
            }
        }
    }
}
