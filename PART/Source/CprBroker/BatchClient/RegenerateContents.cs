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
using System.Configuration;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data.Part;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using NUnit.Framework;
using System.Xml;
using CprBroker.Engine;

namespace BatchClient
{
    public abstract class RegenerateContents : ConsoleEnvironment
    {
        public abstract Guid ActorId { get; }

        public override string[] LoadCprNumbers()
        {
            Utilities.UpdateConnectionString(this.BrokerConnectionString);

            if (!string.IsNullOrEmpty(SourceFile))
            {
                return System.IO.File.ReadAllLines(SourceFile).Where(l => !string.IsNullOrEmpty(l)).ToArray();
            }
            else
            {
                using (var dataContext = new PartDataContext(this.BrokerConnectionString))
                {
                    return dataContext.PersonRegistrations
                        .Where(pr => pr.SourceObjects != null && pr.ActorRef.Value == ActorId.ToString())
                        .OrderBy(pr => pr.PersonRegistrationId)
                        .Select(pr => pr.PersonRegistrationId.ToString())
                        .ToArray();
                }
            }
        }

        public override void ProcessPerson(string personRegId)
        {
            BrokerContext.Initialize(this.ApplicationToken, this.UserToken);

            var personRegistrationId = new Guid(personRegId);

            using (var dataContext = new PartDataContext(this.BrokerConnectionString))
            {
                CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Processing registration {0}", personRegId), "", "");

                try
                {
                    var dbReg = dataContext.PersonRegistrations.Where(pr => pr.PersonRegistrationId == personRegistrationId).First();
                    var pnr = dataContext.PersonMappings.Where(pm => pm.UUID == dbReg.UUID).Select(pm => pm.CprNumber).First();
                    Func<string, Guid> cpr2uuidFunc = relPnr =>
                    {
                        relPnr = relPnr.PadLeft(10, ' ');
                        var ret = dataContext.PersonMappings.Where(pm => pm.CprNumber == relPnr).Select(pm => pm.UUID).FirstOrDefault();
                        if (ret.Equals(Guid.Empty))
                        {
                            var res = PartManager.GetUuid("", AppToken.ToString(), relPnr);
                            ret = new Guid(res.UUID);
                        }
                        return ret;
                    };

                    //string oldContentsXml, oldSourceXml, newContentsXml, newSourceXml;

                    //TakeCopies(dbReg, out oldContentsXml, out oldSourceXml);

                    var oioReg = CreateXmlType(pnr, dbReg, cpr2uuidFunc);

                    dbReg.SourceObjects = System.Xml.Linq.XElement.Parse(oioReg.SourceObjectsXml);
                    dbReg.SetContents(oioReg);

                    //TakeCopies(dbReg, out newContentsXml, out newSourceXml);

                    //CompareContents(oioReg, oldContentsXml, newContentsXml);
                    //CompareSource(oldSourceXml, newSourceXml);

                    dataContext.SubmitChanges();
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Finished registration {0}", personRegId), "", "");
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                    throw ex;
                }
            }
        }

        public void TakeCopies(PersonRegistration dbReg, out string contentsXml, out string sourceXml)
        {
            var oioReg = PersonRegistration.ToXmlType(dbReg);

            oioReg.AttributListe.Egenskab[0].FoedestedNavn = null;
            contentsXml = Strings.SerializeObject(oioReg);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(contentsXml);
            var nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("ns", "urn:oio:sagdok:2.0.0");
            var nodes = doc.SelectNodes("//ns:Virkning", nsmgr);
            Console.WriteLine("Nodes : {0}", nodes.Count);
            foreach (XmlNode node in nodes)
            {
                node.ParentNode.RemoveChild(node);
            }

            contentsXml = doc.ChildNodes[1].OuterXml;
            // Repeat serialization to avoid empty text
            oioReg = Strings.Deserialize<RegistreringType1>(contentsXml);
            contentsXml = Strings.SerializeObject(oioReg);

            sourceXml = dbReg.SourceObjects.ToString();
        }

        public void CompareContents(RegistreringType1 oioReg, string oldContentsXml, string newContentsXml)
        {
            if ((oioReg.AttributListe.RegisterOplysning[0].Item as CprBorgerType).FolkeregisterAdresse != null && (oioReg.AttributListe.RegisterOplysning[0].Item as CprBorgerType).FolkeregisterAdresse.Item is DanskAdresseType)
            {
                NUnit.Framework.Assert.AreNotEqual(oldContentsXml, newContentsXml);
            }
            else
            {
                NUnit.Framework.Assert.AreEqual(oldContentsXml, newContentsXml);
            }
        }

        public void CompareSource(string oldSourceXml, string newSourceXml)
        {
            Assert.AreEqual(oldSourceXml, newSourceXml);
        }
        public abstract RegistreringType1 CreateXmlType(string pnr, PersonRegistration dbReg, Func<string, Guid> cpr2uuidFunc);
    }
}
