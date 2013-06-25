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
using System.Data.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Data;
using CprBroker.Data.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IPartReadDataProvider, IPartSearchDataProvider, IPartPersonMappingDataProvider
    {

        #region IPartSearchDataProvider Members

        public Guid[] Search(CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            Guid[] ret = null;
            using (var dataContext = new PartDataContext())
            {
                int firstResults = 0;
                int.TryParse(searchCriteria.FoersteResultatReference, out firstResults);

                int maxResults = 0;
                int.TryParse(searchCriteria.MaksimalAntalKvantitet, out maxResults);
                if (maxResults <= 0)
                {
                    maxResults = 1000;
                }

                var result = PersonRegistration.GetUuidsByCriteria(dataContext, searchCriteria.SoegObjekt, firstResults, maxResults);
                ret = result.ToArray();
            }
            // TODO: filter by effect date            
            return ret;
        }

        #endregion

        #region IPartReadDataProvider Members

        public CprBroker.Schemas.Part.RegistreringType1 Read(PersonIdentifier uuid, CprBroker.Schemas.Part.LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            Schemas.Part.RegistreringType1 ret = null;
            var fromRegistrationDate = TidspunktType.ToDateTime(input.RegistreringFraFilter);
            var toRegistrationDate = TidspunktType.ToDateTime(input.RegistreringTilFilter);

            var fromEffectDate = TidspunktType.ToDateTime(input.VirkningFraFilter);
            var ToEffectDate = TidspunktType.ToDateTime(input.VirkningTilFilter);

            using (var dataContext = new PartDataContext())
            {
                Data.Part.PersonRegistration.SetChildLoadOptions(dataContext);
                ret =
                (
                    from personReg in dataContext.PersonRegistrations
                    where personReg.UUID == uuid.UUID
                        // Filter by registration date
                    && (!fromRegistrationDate.HasValue || personReg.RegistrationDate >= fromRegistrationDate)
                    && (!toRegistrationDate.HasValue || personReg.RegistrationDate <= toRegistrationDate)
                    // TODO: Filter by effect date
                    orderby personReg.RegistrationDate descending
                    orderby personReg.BrokerUpdateDate descending
                    select Data.Part.PersonRegistration.ToXmlType(personReg)
                ).FirstOrDefault();
            }
            ql = QualityLevel.LocalCache;
            return ret;
        }

       
        #endregion

        #region IPartPersonMappingDataProvider Members

        public Guid? GetPersonUuid(string cprNumber)
        {
            return Data.Part.PersonMapping.AssignGuids(new string[] { cprNumber })[0];
        }

        public Guid?[] GetPersonUuidArray(string[] cprNumberArray)
        {
            return Data.Part.PersonMapping.AssignGuids(cprNumberArray);
        }

        #endregion

        #region IPartPeriodDataProvider members
        public FiltreretOejebliksbilledeType ReadPeriod(DateTime fromDate, DateTime toDate, PersonIdentifier pId, Func<string, Guid> cpr2uuidFunc)
        {
            var targetVirkning = VirkningType.Create(fromDate, toDate);

            using (var dataContext = new PartDataContext())
            {
                var oioRegs = dataContext
                    .PersonRegistrations
                    // TODO: Shall we also filter by ActorRef.Value to only include CPR Direct?
                    .Where(pr => pr.UUID == pId.UUID)
                    .OrderBy(pr => pr.RegistrationDate)
                    .Select(pr => PersonRegistration.ToXmlType(pr))
                    .ToArray();

                return RegistreringType1.Merge(pId, targetVirkning, oioRegs);
            }
        }
        #endregion
    }
}
