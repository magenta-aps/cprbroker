using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            PartHandler = new PartService.PartSoapClient("PartSoap", cprPersonWSUrl);
            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            PartHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);
        }
        #endregion

        #region Methods

        public Guid GetPersonUuid(PartService.ApplicationHeader applicationHeader, string cprNumber)
        {
            try
            {
                return PartHandler.GetPersonUuid(applicationHeader, cprNumber);
            }
            catch (Exception)
            {

            }
            return Guid.Empty;
        }

        public PartService.RegistreringType1 Read(PartService.ApplicationHeader applicationHeader, Guid uuid)
        {
            try
            {
                PartService.LaesOutputType output = null;
                PartService.LaesInputType input = new CPR_Business_Application_Demo.PartService.LaesInputType()
                {
                    RegistreringFraFilter = null,
                    RegistreringTilFilter = null,
                    UUID = uuid.ToString(),
                    VirkningFraFilter = null,
                    VirkningTilFilter = null
                };

                var ql = PartHandler.Read(applicationHeader, input, out output);

                if (output != null && output.LaesResultat != null && output.LaesResultat.Item is PartService.RegistreringType1)
                {
                    return output.LaesResultat.Item as PartService.RegistreringType1;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        public PartService.RegistreringType1[] List(PartService.ApplicationHeader applicationHeader, Guid[] uuids)
        {
            try
            {
                PartService.ListOutputType1 output = null;
                PartService.ListInputType input = new CPR_Business_Application_Demo.PartService.ListInputType()
                {
                    RegistreringFraFilter = null,
                    RegistreringTilFilter = null,
                    UUID = Array.ConvertAll<Guid, string>(uuids, id => id.ToString()),
                    VirkningFraFilter = null,
                    VirkningTilFilter = null
                };

                var ql = PartHandler.List(applicationHeader, input, out output);

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

        public string[] Search(PartService.ApplicationHeader applicationHeader, string cprNumber)
        {
            try
            {
                PartService.SoegOutputType output = null;
                PartService.SoegInputType1 input = new CPR_Business_Application_Demo.PartService.SoegInputType1()
                {
                    MaksimalAntalResultater = "10",
                    FoersteResultat = "0",
                    Soeg = new CPR_Business_Application_Demo.PartService.SoegObjektType()
                    {
                        BrugervendtNoegleTekst = cprNumber
                    }
                };

                var ql = PartHandler.Search(applicationHeader, input, out output);

                if (output != null && output.IdListe != null)
                {
                    return output.IdListe;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
        #endregion

        #region Private Fields
        private readonly PartService.PartSoapClient PartHandler;

        #endregion
    }
}
