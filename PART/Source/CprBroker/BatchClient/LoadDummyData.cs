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
using System.IO;
using CprBroker.Providers.DPR;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data;
using CprBroker.Data.Part;

namespace BatchClient
{
    public class LoadDummyData : PartClient
    {
        public override string[] LoadCprNumbers()
        {
            Utilities.UpdateConnectionString(this.BrokerConnectionString);
            return Utilities.LoadCprNumbersOneByOne(SourceFile);
        }

        public override void ProcessPerson(string cprNumberOrUuid)
        {
            using (var dataContext = new PartDataContext(this.BrokerConnectionString))
            {
                CprBroker.Engine.BrokerContext.Initialize(this.ApplicationToken, this.UserToken);
                CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Preparing dummy data for failed PNR: {0}", cprNumberOrUuid), "", "");
                RegistreringType1 reg = new RegistreringType1();
                UnikIdType uuid = new UnikIdType();
                uuid.Item = CprBroker.Utilities.Constants.FakeActorId.ToString();
                reg.AktoerRef = uuid;
                reg.Tidspunkt = TidspunktType.Create(new DateTime(2013, 9, 24));
                var dbReg = CprBroker.Data.Part.PersonRegistration.FromXmlType(reg);
                dbReg.BrokerUpdateDate = new DateTime(2013, 9, 24);
                dbReg.UUID = dataContext.PersonMappings.Where(pm => pm.CprNumber == cprNumberOrUuid).First().UUID;
                var pers = dataContext.Persons.Where(p => p.UUID == dbReg.UUID).FirstOrDefault();
                if (pers == null)
                {
                    dataContext.Persons.InsertOnSubmit(new Person()
                        {
                            UUID = dbReg.UUID,
                            UserInterfaceKeyText = cprNumberOrUuid,
                        }
                    );
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Adding {0} to Person", cprNumberOrUuid), "", "");
                }
                else
                {
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Ignoring {0} - is in Person", cprNumberOrUuid), "", "");
                }
                var persReg = dataContext.PersonRegistrations.Where(p => p.UUID == dbReg.UUID).FirstOrDefault();
                if (persReg == null)
                {
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Adding {0} to PersonRegistration", cprNumberOrUuid), "", "");
                    dataContext.PersonRegistrations.InsertOnSubmit(dbReg);
                    dataContext.SubmitChanges();
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Added dummy data for {0}", cprNumberOrUuid), "", "");
                }
                else
                {
                    CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, GetType().Name, string.Format("Ignoring {0} - is in PersonRegistration", cprNumberOrUuid), "", "");
                }
            }
        }
    }
}
