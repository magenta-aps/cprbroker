﻿using System;
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
