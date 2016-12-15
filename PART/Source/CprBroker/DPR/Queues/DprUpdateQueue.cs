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
using CprBroker.Engine.Part;
using CprBroker.Utilities;
using CprBroker.Engine.Local;
using CprBroker.Data.Part;
using System.Threading;

namespace CprBroker.Providers.DPR.Queues
{
    public class DprUpdateQueue : CprBroker.Engine.Queues.Queue<DprUpdateQueueItem>
    {
        public override Engine.DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new Engine.DataProviderConfigPropertyInfo[] { };
            }
        }

        public override DprUpdateQueueItem[] Process(DprUpdateQueueItem[] items)
        {
            var ret = new List<DprUpdateQueueItem>();
            var cache = new UuidCache();
            var factory = new CprBroker.Engine.DataProviderFactory();
            var groups = items.GroupBy(i => i.DataProviderId);

            foreach (var group in groups)
            {
                try // group level
                {
                    var prov = factory.GetDataProvider<DprDatabaseDataProvider>(group.Key);
                    if (prov != null)
                    {
                        using (var dprDataContext = new DPRDataContext(prov.ConnectionString))
                        {
                            foreach (var item in group.ToArray())
                            {
                                Mutex personMutex = null;
                                try // item level
                                {
                                    var uuid = cache.GetUuid(item.Pnr.ToPnrDecimalString());

                                    // Establish a person based critical section
                                    personMutex = new Mutex(false, CprBroker.Utilities.Strings.GuidToString(uuid));
                                    personMutex.WaitOne();

                                    if (PersonExists(item.Pnr))
                                    {
                                        var info = PersonInfo.GetPersonInfo(dprDataContext, item.Pnr);
                                        if (info != null)
                                        {
                                            var oioReg = info.ToRegisteringType1(cache.GetUuid, dprDataContext);
                                            var pId = new CprBroker.Schemas.PersonIdentifier()
                                            {
                                                CprNumber = item.Pnr.ToPnrDecimalString(),
                                                UUID = uuid
                                            };
                                            CprBroker.Engine.Local.UpdateDatabase.UpdatePersonRegistration(pId, oioReg);
                                            ret.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        // We consider this as a failure, so that the update can be attempted at a later point in time
                                        // Log the decision
                                        CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}:{1} skipping irrelevant person <{2}>", this.GetType().Name, prov.ToString(), item.Pnr.ToPnrDecimalString());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Admin.LogException(ex);
                                }
                                finally
                                {
                                    // Release the lock
                                    if (personMutex != null)
                                        personMutex.ReleaseMutex();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                }
            }
            return ret.ToArray();
        }

        public virtual bool PersonExists(decimal pnr)
        {
            using (var partDataContext = new PartDataContext())
            {
                var pnrString = pnr.ToPnrDecimalString();
                var uuid = partDataContext.PersonMappings.FirstOrDefault(pm => pm.CprNumber == pnrString)?.UUID;
                var personExists = uuid.HasValue &&
                    partDataContext.PersonRegistrations.FirstOrDefault(pr => pr.UUID == uuid.Value) != null;
                return personExists;
            }
        }
    }
}
