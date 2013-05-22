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
