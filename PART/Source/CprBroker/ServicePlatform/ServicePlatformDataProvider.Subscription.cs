using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Engine.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : IPutSubscriptionDataProvider
    {

        protected enum ReturnCodePNR { ADDED, REMOVED, ALREADY_EXISTED, NON_EXISTING_PNR };

        public bool PutSubscription(PersonIdentifier personIdentifier)
        {
            var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);

            using (var callContext = this.BeginCall("AddPNRSubscription", personIdentifier.CprNumber))
            {
                try
                {

                    var request = new CprSubscriptionService.AddPNRSubscriptionType()
                    {
                        InvocationContext = GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID),
                        PNR = personIdentifier.CprNumber
                    };

                    var resultWrp = service.AddPNRSubscription(request);
                    if (resultWrp != null)
                    {
                        ReturnCodePNR returnCode = (ReturnCodePNR)Enum.Parse(typeof(ReturnCodePNR), resultWrp.Result); //will throw an overflow exception in case of unknown value.
                        switch (returnCode)
                        {
                            case ReturnCodePNR.ADDED:
                                //success
                                break;
                            case ReturnCodePNR.ALREADY_EXISTED:
                                //success
                                break;
                            case ReturnCodePNR.NON_EXISTING_PNR:
                                throw new Exception(String.Format("Error placing subscription for PNR [%s], service platform returns NON_EXISTING_PNR.", personIdentifier.CprNumber));
                            default:
                                throw new Exception(String.Format("Error placing subscription for PNR [%s], service platform returns unexpected code [%s].", personIdentifier.CprNumber,returnCode));
                        }
                        //Admin.LogSuccess(String.Format("Placed service platform subscription on PNR [%s], returned value [%s] ",personIdentifier.CprNumber, returnCode)); //TODO: Remove this log line
                        callContext.Succeed();
                        return true;
                    }
                    else
                    {
                        throw new Exception(String.Format("Null value returned by service api call AddPNRSubscription, when trying to place subscription for PNR: [%s]", personIdentifier.CprNumber));
                    }

                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                    callContext.Fail();
                    return false;
                }
            }
        }

    }
}