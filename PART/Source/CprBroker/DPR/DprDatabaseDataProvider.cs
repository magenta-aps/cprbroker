/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Util;
using System.Linq.Expressions;


namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Implements the Read operation of Part standard
    /// </summary>
    public partial class DprDatabaseDataProvider : ClientDataProvider, IPartReadDataProvider
    {
        #region IPartReadDataProvider Members

        public RegistreringType1 Read(PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            CprBroker.Schemas.Part.RegistreringType1 ret = null;
            EnsurePersonDataExists(uuid.CprNumber);

            //TODO: GetPropertyValuesOfType values fromDate Input
            DateTime? effectDate = null;
            if (!effectDate.HasValue)
            {
                effectDate = DateTime.Today;
            }
            using (var dataContext = new DPRDataContext(this.ConnectionString))
            {
                var db =
                (
                    from personInfo in PersonInfo.PersonInfoExpression.Compile()(dataContext)
                    where personInfo.PersonTotal.PNR == Decimal.Parse(uuid.CprNumber)
                    select personInfo
                ).FirstOrDefault();

                if (db != null)
                {
                    ret = db.ToRegisteringType1(effectDate, cpr2uuidFunc, dataContext);
                }
            }
            ql = QualityLevel.DataProvider;
            return ret;
        }

        #endregion
    }
}
