using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to persons' data
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = ServiceNames.Person.Service, Description = ServiceDescription.Person.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class CPRPersonWS : GKApp.WS.wsBaseV2
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        public QualityHeader qualityHeader = new QualityHeader();
        private const string QualityHeaderName = "qualityHeader";

        public CPRPersonWS()
        {
            BaseInit();
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetCitizenNameAndAddress, Description = ServiceDescription.Person.GetCitizenNameAndAddress)]
        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string PersonCivilRegistrationIdentifier)
        {
            return Manager.Citizen.GetCitizenNameAndAddress(applicationHeader.UserToken, applicationHeader.ApplicationToken, PersonCivilRegistrationIdentifier, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetCitizenBasic, Description = ServiceDescription.Person.GetCitizenBasic)]
        public PersonBasicStructureType GetCitizenBasic(string PersonCivilRegistrationIdentifier)
        {
            return Manager.Citizen.GetCitizenBasic(applicationHeader.UserToken, applicationHeader.ApplicationToken, PersonCivilRegistrationIdentifier, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetCitizenFull, Description = ServiceDescription.Person.GetCitizenFull)]
        public PersonFullStructureType GetCitizenFull(string PersonCivilRegistrationIdentifier)
        {
            return Manager.Citizen.GetCitizenFull(applicationHeader.UserToken, applicationHeader.ApplicationToken, PersonCivilRegistrationIdentifier, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetCitizenRelations, Description = ServiceDescription.Person.GetCitizenRelations)]
        public PersonRelationsType GetCitizenRelations(string PersonCivilRegistrationIdentifier)
        {
            return Manager.Citizen.GetCitizenRelations(applicationHeader.UserToken, applicationHeader.ApplicationToken, PersonCivilRegistrationIdentifier, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetCitizenChildren, Description = ServiceDescription.Person.GetCitizenChildren)]
        public SimpleCPRPersonType[] GetCitizenChildren(string PersonCivilRegistrationIdentifier, bool IncludeCustodies)
        {
            return Manager.Citizen.GetCitizenChildren(applicationHeader.UserToken, applicationHeader.ApplicationToken, PersonCivilRegistrationIdentifier, IncludeCustodies, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.RemoveParentAuthorityOverChild, Description = ServiceDescription.Person.RemoveParentAuthorityOverChild)]
        public bool RemoveParentAuthorityOverChild(string ParentCivilRegistrationIdentifier, string ChildCivilRegistrationIdentifier)
        {
            return Manager.Citizen.RemoveParentAuthorityOverChild(applicationHeader.UserToken, applicationHeader.ApplicationToken, ParentCivilRegistrationIdentifier, ChildCivilRegistrationIdentifier);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.SetParentAuthorityOverChild, Description = ServiceDescription.Person.SetParentAuthorityOverChild)]
        public bool SetParentAuthorityOverChild(string ParentCivilRegistrationIdentifier, string ChildCivilRegistrationIdentifier)
        {
            return Manager.Citizen.SetParentAuthorityOverChild(applicationHeader.UserToken, applicationHeader.ApplicationToken, ParentCivilRegistrationIdentifier, ChildCivilRegistrationIdentifier);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Person.MethodNames.GetParentAuthorityOverChildChanges, Description = ServiceDescription.Person.GetParentAuthorityOverChildChanges)]
        public ParentAuthorityRelationshipType[] GetParentAuthorityOverChildChanges(string ChildCivilRegistrationIdentifier)
        {
            return Manager.Citizen.GetParentAuthorityOverChildChanges(applicationHeader.UserToken, applicationHeader.ApplicationToken, ChildCivilRegistrationIdentifier);
        }
    }

}
