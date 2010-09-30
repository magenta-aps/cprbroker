using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CPR_Business_Application_Demo.Adapters;
using CPR_Business_Application_Demo.Adapters.CPRPersonWS;

namespace CPR_Business_Application_Demo.Business
{
    public class CPRPersonController
    {
        #region Construction
        public CPRPersonController(ApplicationSettingsBase settings)
        {
            this.settings = settings;

            userToken = settings["UserToken"].ToString();
            appToken = settings["AppToken"].ToString();
            appName = settings["ApplicationName"].ToString();
            cprPersonWebServiceUrl = settings["CPRBrokerWebServiceUrl"].ToString();
        }
        #endregion

        #region Methods
        public PersonBasicStructureType GetCitizenBasic(string personCivilRegistrationIdentifier)
        {
            try
            {
                var adapter = new CPRPersonAdapter(cprPersonWebServiceUrl);

                return adapter.GetCitizenBasic(GetHeader(), personCivilRegistrationIdentifier);
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string personCivilRegistrationIdentifier)
        {
            try
            {
                var adapter = new CPRPersonAdapter(cprPersonWebServiceUrl);

                return adapter.GetCitizenNameAndAddress(GetHeader(), personCivilRegistrationIdentifier);

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        public PersonRelationsType GetCitizenRelations(string personCivilRegistrationIdentifier, out QualityHeader qualityHeader)
        {
            try
            {
                var adapter = new CPRPersonAdapter(cprPersonWebServiceUrl);

                return adapter.GetCitizenRelations(GetHeader(), personCivilRegistrationIdentifier, out qualityHeader);

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                qualityHeader = new QualityHeader()
                                    {
                                        QualityLevel = QualityLevel.LocalCache
                                    };
                return null;
            }
        }

        public SimpleCPRPersonType[] GetCitizenChildren(string personCivilRegistrationIdentifier, bool includeCustodies, out QualityHeader qualityHeader)
        {
            try
            {
                var adapter = new CPRPersonAdapter(cprPersonWebServiceUrl);
                return adapter.GetCitizenChildren(GetHeader(), personCivilRegistrationIdentifier, includeCustodies, out qualityHeader);

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                qualityHeader = new QualityHeader()
                {
                    QualityLevel = QualityLevel.LocalCache
                };
                return null;
            }
        }
        #endregion

        #region Private Methods

        ApplicationHeader GetHeader()
        {
            return new ApplicationHeader()
            {
                UserToken = userToken,
                ApplicationToken = appToken
            };
        }
        #endregion

        #region Private Properties

        private string ErrorMessage;
        private ApplicationSettingsBase settings;

        private readonly string userToken;
        private readonly string appToken;
        private readonly string appName;
        private readonly string cprPersonWebServiceUrl;

        #endregion
    }
}
