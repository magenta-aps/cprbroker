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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.Net.Sockets;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectClientDataProvider
    {
        public IndividualResponseType GetResponse(IndividualRequestType request)
        {
            string response;
            string error;
            if (Send(request.PNR, request.Contents, out response, out error))
            {
                var ret = new IndividualResponseType() { Contents = response };
                ret.FillFromFixedLengthString(ret.Data, Constants.DataObjectMap);
                ret.SourceObject = ret.Data;
                return ret;
            }
            else
            {
                if (error != null)
                    throw new Exception(error);
                else
                    return null;
            }
        }

        protected bool Send(decimal pnr, string message, out string response, out string error)
        {
            if (modulus11OK(pnr.ToDecimalString()))
            {
                error = null;
                NetworkStream stream = null;

                TcpClient client = new TcpClient(Address, Port);
                Byte[] data = Constants.TcpClientEncoding.GetBytes(message);

                stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                data = new Byte[Constants.ResponseLengths.MaxResponseLength];

                int bytes = stream.Read(data, 0, data.Length);
                response = Constants.TcpClientEncoding.GetString(data, 0, bytes);

                string errorCode = response.Substring(Constants.ResponseLengths.ErrorCodeIndex, Constants.ResponseLengths.ErrorCodeLength);
                Admin.LogFormattedSuccess("CPR client: PNR <{0}>, status code <{1}>", pnr.ToPnrDecimalString(), errorCode);

                if (Constants.ErrorCodes.ContainsKey(errorCode))
                {
                    error = Constants.ErrorCodes[errorCode];
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Admin.LogFormattedSuccess("PNR <{0}> is invalid, CPR client will not be invoked.", pnr.ToPnrDecimalString());
                response = null;
                error = null;
                return false;
            }
        }

        protected bool modulus11OK(String CprNumber)
        {
            if (CprNumber.Length == 9)
                CprNumber = "0" + CprNumber;
            bool result = false;
            int[] multiplyBy = { 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };
            int Sum = 0;
            // We test if the length of the CPR number is right and if the number does not conatain tailing 0's
            if (CprNumber.Length == 10 && CprNumber.Substring(6, 4) != "0000")
            {
                /*
                 * We cannot do modulus control on people with birth dates 19650101 or 19660101,
                 * thus those dates just pass through with no control at all.
                 */
                if (CprNumber.Substring(6) == "010165" || CprNumber.Substring(6) == "010166")
                {
                    result = true;
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Sum += Convert.ToInt32(CprNumber.Substring(i, 1)) * multiplyBy[i];
                    }
                    if ((Sum % 11) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
