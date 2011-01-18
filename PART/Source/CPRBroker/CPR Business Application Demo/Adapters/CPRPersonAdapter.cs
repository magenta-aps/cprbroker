using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPR_Business_Application_Demo.Adapters.CPRPersonWS;

namespace CPR_Business_Application_Demo.Adapters
{
    public class CPRPersonAdapter
    {
        #region Construction
        public CPRPersonAdapter(string cprPersonWSUrl)
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

            PersonHandler = new CPRPersonWSSoapClient("CPRPersonWSSoap", cprPersonWSUrl);
            // Set the timeout to avoid hanging the application for too long when wrong urls were entered
            PersonHandler.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 5);
        }
        #endregion

        #region Methods
        public PersonBasicStructureType GetCitizenBasic(ApplicationHeader applicationHeader, string personCivilRegistrationIdentifier)
        {
            try
            {
                PersonBasicStructureType person;
                QualityHeader qualityHeader = PersonHandler.GetCitizenBasic(applicationHeader, personCivilRegistrationIdentifier, out person);

                return person;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public PersonFullStructureType GetCitizenFull(ApplicationHeader applicationHeader, string personCivilRegistrationIdentifier)
        {
            try
            {
                PersonFullStructureType person;
                QualityHeader qualityHeader = PersonHandler.GetCitizenFull(applicationHeader, personCivilRegistrationIdentifier, out person);

                return person;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(ApplicationHeader applicationHeader, string personCivilRegistrationIdentifier)
        {
            try
            {
                PersonNameAndAddressStructureType personNameAndAddress;
                QualityHeader qualityHeader = PersonHandler.GetCitizenNameAndAddress(applicationHeader, personCivilRegistrationIdentifier, out personNameAndAddress);

                return personNameAndAddress;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PersonRelationsType GetCitizenRelations(ApplicationHeader applicationHeader, string personCivilRegistrationIdentifier, out QualityHeader qualityHeader)
        {
            try
            {
                PersonRelationsType personRelations;
                qualityHeader = PersonHandler.GetCitizenRelations(applicationHeader, personCivilRegistrationIdentifier, out personRelations);
                return personRelations;
            }
            catch (Exception)
            {
                qualityHeader = new QualityHeader(){QualityLevel = QualityLevel.LocalCache};
                return null;
            }
        }

        public SimpleCPRPersonType[] GetCitizenChildren(ApplicationHeader applicationHeader, string personCivilRegistrationIdentifier, bool includeCustodies, out QualityHeader qualityHeader)
        {
            try
            {
                SimpleCPRPersonType[] simpleCprPersons;
                qualityHeader = PersonHandler.GetCitizenChildren(applicationHeader, personCivilRegistrationIdentifier, includeCustodies, out simpleCprPersons);
                return simpleCprPersons;
            }
            catch (Exception)
            {
                qualityHeader = new QualityHeader() { QualityLevel = QualityLevel.LocalCache };
                return new SimpleCPRPersonType[] {};
            }
        }
        #endregion

        #region Private Fields
        private readonly CPRPersonWSSoapClient PersonHandler;

        #endregion

    }
}
