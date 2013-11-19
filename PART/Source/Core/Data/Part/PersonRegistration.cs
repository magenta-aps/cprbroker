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
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CprBroker.Schemas.Part;
using System.Xml.Linq;
using System.IO;
using CprBroker.Utilities;
using CprBroker.Utilities.WhereConditions;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the PersonRegistration table
    /// </summary>
    public partial class PersonRegistration
    {
        public static Schemas.Part.RegistreringType1 ToXmlType(PersonRegistration db)
        {
            if (db != null)
            {
                var xml = db.Contents.ToString();
                // Deserialize the stored XML for maximum performance
                var ret = Strings.Deserialize<RegistreringType1>(xml);
                if (db.SourceObjects != null)
                {
                    ret.SourceObjectsXml = db.SourceObjects.ToString();
                }
                return ret;
            }
            return null;
        }

        [Obsolete("Child tables no longer used")]
        public static void SetChildLoadOptions(PartDataContext dataContext)
        {
            // Do Nothing, because setting load options slows down the loading
            return;
            /*
            DataLoadOptions loadOptions = new DataLoadOptions();
            SetChildLoadOptions(loadOptions);
            dataContext.LoadOptions = loadOptions;
            */
        }

        [Obsolete("Child tables no longer used")]
        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            return;
            /*
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonAttributes);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonRelationships);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonState);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.ActorRef);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.LifecycleStatus);
            

            Effect.SetChildLoadOptions(loadOptions);
            CountryRef.SetChildLoadOptions(loadOptions);

            CprBroker.Data.Part.PersonAttributes.SetChildLoadOptions(loadOptions);
            PersonRelationship.SetChildLoadOptions(loadOptions);
            PersonState.SetChildLoadOptions(loadOptions);
            */
        }

        public static PersonRegistration FromXmlType(CprBroker.Schemas.Part.RegistreringType1 partRegistration)
        {
            PersonRegistration ret = null;
            if (partRegistration != null)
            {
                ret = new PersonRegistration()
                {
                    PersonRegistrationId = Guid.NewGuid(),

                    ActorRef = ActorRef.FromXmlType(partRegistration.AktoerRef),
                    CommentText = partRegistration.CommentText,
                    LifecycleStatusId = LifecycleStatus.GetCode(partRegistration.LivscyklusKode),
                    RegistrationDate = partRegistration.Tidspunkt.ToDateTime().Value,

                    //PersonState = PersonState.FromXmlType(partRegistration.TilstandListe)
                };
                /*
                if (partRegistration.AttributListe != null)
                {
                    ret.PersonAttributes.AddRange(CprBroker.Data.Part.PersonAttributes.FromXmlType(partRegistration.AttributListe));
                }

                if (partRegistration.RelationListe != null)
                {
                    ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
                }
                */
                ret.SetContents(partRegistration);

                if (!string.IsNullOrEmpty(partRegistration.SourceObjectsXml))
                {
                    ret.SourceObjects = System.Xml.Linq.XElement.Parse(partRegistration.SourceObjectsXml);
                }

            }
            return ret;
        }

        public void SetContents(RegistreringType1 partRegistration)
        {
            var xml = Strings.SerializeObject(partRegistration);
            this.Contents = System.Xml.Linq.XElement.Load(new StringReader(xml));
        }

        public bool Equals(RegistreringType1 oio)
        {
            var xml = Strings.SerializeObject(oio);
            // Repeat serialization to avoid empty text
            oio = Strings.Deserialize<RegistreringType1>(xml);
            xml = Strings.SerializeObject(oio);

            var thisOio = ToXmlType(this);
            var thisXml = Strings.SerializeObject(thisOio);
            return string.Equals(xml, thisXml);
        }

        [Obsolete("Replaced by the newer GetBy...() methods")]
        public static Expression<Func<PersonRegistration, bool>> CreateWhereExpression(PartDataContext dataContext, CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            var pred = PredicateBuilder.True<Data.Part.PersonRegistration>();
            if (searchCriteria.SoegObjekt != null)
            {
                if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.UUID))
                {
                    var personUuid = new Guid(searchCriteria.SoegObjekt.UUID);
                    pred = pred.And(p => p.UUID == personUuid);
                }
                // Search by cpr number
                if (!string.IsNullOrEmpty(searchCriteria.SoegObjekt.BrugervendtNoegleTekst))
                {
                    pred = pred.And(pr => pr.Person.UserInterfaceKeyText == searchCriteria.SoegObjekt.BrugervendtNoegleTekst);
                }
                if (searchCriteria.SoegObjekt.SoegAttributListe != null)
                {
                    if (searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab != null)
                    {
                        foreach (var prop in searchCriteria.SoegObjekt.SoegAttributListe.SoegEgenskab)
                        {
                            if (prop != null && prop.NavnStruktur != null)
                            {
                                if (prop.NavnStruktur.PersonNameStructure != null)
                                {
                                    // Search by name
                                    var name = prop.NavnStruktur.PersonNameStructure;
                                    if (!name.IsEmpty)
                                    {
                                        // TODO: Test name lookup after new struture (multiple attribuutes)
                                        /*
                                        var cprNamePred = PredicateBuilder.True<Data.Part.PersonRegistration>();
                                        if (!string.IsNullOrEmpty(name.PersonGivenName))
                                        {
                                            cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.Where(attr => attr.PersonProperties != null && attr.PersonProperties.PersonName.FirstName == name.PersonGivenName).FirstOrDefault() != null);
                                        }
                                        if (!string.IsNullOrEmpty(name.PersonMiddleName))
                                        {
                                            cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.Where(attr => attr.PersonProperties != null && attr.PersonProperties.PersonName.MiddleName == name.PersonMiddleName).FirstOrDefault() != null);
                                        }
                                        if (!string.IsNullOrEmpty(name.PersonSurnameName))
                                        {
                                            cprNamePred = cprNamePred.And((pt) => pt.PersonAttributes.Where(attr => attr.PersonProperties != null && attr.PersonProperties.PersonName.LastName == name.PersonSurnameName).FirstOrDefault() != null);
                                        }
                                        pred = pred.And(cprNamePred);
                                        */
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return pred;
        }

        public static List<WhereCondition> CreateXQueryElements(SoegObjektType soegObject)
        {
            var personRegistration = soegObject.ToRegistreringType1();
            var xml = CprBroker.Utilities.Strings.SerializeObject(personRegistration);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            return XQueryCondition.CreateXQueryElements(doc.DocumentElement, "Contents");
        }

        public static IEnumerable<Guid> GetUuidsByCriteria(PartDataContext dataContext, SoegObjektType soegObject, int startIndex, int maxCount)
        {
            var elements = PersonRegistration.CreateXQueryElements(soegObject);

#if Mono
            var byCriteriaStr = WhereCondition.GetMatchingObjects<object>(dataContext, elements, "PersonRegistration", true, new string[] { "UUID" }, startIndex, maxCount, "UUID");
            var byCriteria = byCriteriaStr.Select(o => new Guid(o.ToString()));
#else
            var byCriteria = WhereCondition.GetMatchingObjects<Guid>(dataContext, elements, "PersonRegistration", true, new string[] { "UUID" }, startIndex, maxCount, "UUID");
#endif
            
            return byCriteria;
        }

    }
}
