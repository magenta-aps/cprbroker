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
using CPR_Business_Application_Demo.PartService;

namespace CPR_Business_Application_Demo
{
    public class PartAdapter
    {
        #region Construction
        public PartAdapter(string cprPersonWSUrl)
        {
            // Make sure the provided URL points to the person web service.
            if (!cprPersonWSUrl.EndsWith("/"))
            {
                if (!cprPersonWSUrl.EndsWith("Part.asmx"))
                    cprPersonWSUrl += "/Part.asmx";
            }
            else
            {
                cprPersonWSUrl += "Part.asmx";
            }

            PartHandler = new PartService.PartSoap12Client("PartSoap12", cprPersonWSUrl);
            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            PartHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 15);
        }
        #endregion

        #region Methods
        public PartService.ApplicationHeader CreateApplicationHeader(string appToken, string userToken)
        {
            return new CPR_Business_Application_Demo.PartService.ApplicationHeader()
            {
                ApplicationToken = appToken,
                UserToken = userToken
            };
        }
        public PartService.ApplicationHeader CreateApplicationHeader(string appToken)
        {
            return CreateApplicationHeader(appToken, "");
        }

        public Guid GetUuid(string applicationToken, string cprNumber)
        {
            try
            {
                var ret = PartHandler.GetUuid(CreateApplicationHeader(applicationToken), cprNumber);
                return new Guid(ret.UUID);
            }
            catch (Exception)
            {

            }
            return Guid.Empty;
        }

        public PartService.LaesOutputType Read(string applicationToken, Guid uuid)
        {
            try
            {
                PartService.LaesInputType input = new CPR_Business_Application_Demo.PartService.LaesInputType()
                {
                    RegistreringFraFilter = null,
                    RegistreringTilFilter = null,
                    UUID = uuid.ToString(),
                    VirkningFraFilter = null,
                    VirkningTilFilter = null
                };

                LaesOutputType output;
                PartHandler.Read(CreateApplicationHeader(applicationToken), input, out output);
                return output;                
            }
            catch (Exception)
            {

            }
            return null;
        }

        public PartService.RegistreringType1[] List(string applicationToken, Guid[] uuids)
        {
            try
            {
                PartService.ListInputType input = new CPR_Business_Application_Demo.PartService.ListInputType()
                {
                    RegistreringFraFilter = null,
                    RegistreringTilFilter = null,
                    UUID = Array.ConvertAll<Guid, string>(uuids, id => id.ToString()),
                    VirkningFraFilter = null,
                    VirkningTilFilter = null
                };

                ListOutputType1 output;
                PartHandler.List(CreateApplicationHeader(applicationToken), input, out output);

                if (output != null && output.LaesResultat != null && Array.TrueForAll<PartService.LaesResultatType>(output.LaesResultat, lr => lr.Item is PartService.RegistreringType1))
                {
                    return Array.ConvertAll<PartService.LaesResultatType, PartService.RegistreringType1>(output.LaesResultat, lr => lr.Item as PartService.RegistreringType1);
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        public string[] Search(string applicationToken, string cprNumber)
        {
            try
            {
                PartService.SoegInputType1 input = new CPR_Business_Application_Demo.PartService.SoegInputType1()
                {
                    MaksimalAntalKvantitet = "10",
                    FoersteResultatReference = "0",
                    SoegObjekt = new CPR_Business_Application_Demo.PartService.SoegObjektType()
                    {
                        BrugervendtNoegleTekst = cprNumber
                    }
                };

                SoegOutputType output;
                PartHandler.Search(CreateApplicationHeader(applicationToken), input, out output);

                if (output != null && output.Idliste != null)
                {
                    return output.Idliste;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }



        public string[] SearchRelation(string applicationToken, string cprNumber)
        {
            try
            {
                PartService.SoegInputType1 input = new CPR_Business_Application_Demo.PartService.SoegInputType1()
                {
                    MaksimalAntalKvantitet = "10",
                    FoersteResultatReference = "0",
                    SoegObjekt = new CPR_Business_Application_Demo.PartService.SoegObjektType()
                    {
                        BrugervendtNoegleTekst = cprNumber

                    }
                };

                SoegOutputType output;
                PartHandler.Search(CreateApplicationHeader(applicationToken), input, out output);

                if (output != null && output.Idliste != null)
                {
                    return output.Idliste;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        #endregion

        #region Private Fields
        private readonly PartService.PartSoap12Client PartHandler;

        #endregion
    }
}
