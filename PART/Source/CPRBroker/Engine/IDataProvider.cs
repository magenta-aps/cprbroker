using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.DAL;

namespace CPRBroker.Engine
{
    public interface IDataProvider
    {
        /// <summary>
        /// Ensure that the current provider is alive
        /// </summary>
        /// <returns></returns>
        bool IsAlive();

        /// <summary>
        /// Get the current version for this provider
        /// </summary>
        //TODO: Remove this property from all data providers
        Version Version { get; }
    }

    /// <summary>
    /// Represents an external data provider (DPR, KMD)
    /// </summary>
    public interface IExternalDataProvider
    {
        DAL.DataProvider DatabaseObject { get; set; }
    }

    /// <summary>
    /// Contains methods related to a person's name and address data
    /// </summary>
    public interface IPersonNameAndAddressDataProvider : IDataProvider
    {
        /// <summary>
        /// Reads Name and address for a citizen used on a letter. Must be aware of Death and address protection.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="qualityLevel">The quality of the data object returned</param>
        /// <returns></returns>
        PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel);
    }

    /// <summary>
    /// Contains methods related to a person's basic data
    /// </summary>
    public interface IPersonBasicDataProvider : IDataProvider
    {
        /// <summary>
        /// Reads basic information about the citizen e.g. Name, Address,Civil status,Address protection, Municipality,
        /// Citizenship, Nationality, etc.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="qualityLevel">The quality of the data object returned</param>
        /// <returns></returns>
        PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel);
    }

    /// <summary>
    /// Contains methods related to a person's full data
    /// </summary>
    public interface IPersonFullDataProvider : IDataProvider
    {
        /// <summary>
        /// Reads all information about a particular citizen: GetCitizenBasic() and district, school, election, church etc., spouse,
        /// number of children. Other relevant information.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="qualityLevel">The quality of the data object returned</param>
        /// <returns></returns>
        PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel);
    }

    /// <summary>
    /// Contains methods related tp a person's relations
    /// </summary>
    public interface IPersonRelationsDataProvider : IDataProvider
    {
        /// <summary>
        /// Reads a particular citizens relations to parents, spouse, children and guardian etc.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <returns></returns>
        PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel);
    }

    /// <summary>
    /// Contains methods related to a person's children
    /// </summary>
    public interface IPersonChildrenDataProvider : IDataProvider
    {
        /// <summary>
        /// Reads a particular citizens relations to own children and takes also reads children in custody
        /// using an argument.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="includeCustodies">Whether to include custodies in the returned list</param>
        /// /// <param name="qualityLevel">The quality of the data object returned</param>
        /// <returns></returns>
        SimpleCPRPersonType[] GetCitizenChildren(string userToken, string appToken, string cprNumber, bool includeCustodies, out QualityLevel? qualityLevel);
    }

    /// <summary>
    /// Contains methods related to custody
    /// </summary>
    public interface IPersonCustodyDataProvider : IPersonRelationsDataProvider
    {
        /// <summary>
        /// Writes a change that removes a citizens custody over a child.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="cprChildNumber">Child registeration number.</param>
        /// <returns></returns>
        bool RemoveParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber);

        /// <summary>
        /// Writes a citizens custody over a child.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Person number to search for.</param>
        /// <param name="cprChildNumber">Child registeration number.</param>
        /// <param name="authorityCode">Code of the authority that authorize this relation.</param>
        /// <param name="custodyAuthorityCode">Custody authority code.</param>
        /// <param name="uncertainityMarker">Boolean</param>
        /// <returns></returns>
        bool SetParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber);

        /// <summary>
        /// Reads a list of changes to custody changes done by this component.
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="cprNumber">Child number to search for.</param>
        /// <returns></returns>
        ParentAuthorityRelationshipType[] GetParentAuthorityOverChildChanges(string userToken, string appToken, string cprChildNumber);
    }

    /// <summary>
    /// Contains methods related to application management
    /// </summary>
    public interface IApplicationManager : IDataProvider
    {
        /// <summary>
        /// Creates a new application with the given name
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>        
        /// <param name="name">Application name</param>
        /// <returns></returns>
        ApplicationType RequestAppRegistration(string userToken, string name);

        /// <summary>
        /// Approves an application that has the given token
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="targetAppToken">Application token of the application to be approved</param>
        /// <returns></returns>
        bool ApproveAppRegistration(string userToken, string appToken, string targetAppToken);

        /// <summary>
        /// Returns a list of all applications
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        ApplicationType[] ListAppRegistration(string userToken, string appToken);

        /// <summary>
        /// Unregisters the application with the supplied token
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="targetAppToken">Token of the application that will be removed</param>
        /// <returns></returns>
        bool UnregisterApp(string userToken, string appToken, string targetAppToken);
    }

    /// <summary>
    /// Contains methods related to test citizens
    /// </summary>
    public interface ITestCitizenManager : IDataProvider
    {
        /// <summary>
        /// Adds a new test citizen to the database
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="oioPerson">Full person object</param>
        /// <returns></returns>
        bool CreateTestCitizen(string userToken, string appToken, PersonFullStructureType oioPerson);
    }

    /// <summary>
    /// Contains methods related to data provider management
    /// </summary>
    public interface IDataProviderManager : IDataProvider
    {
        /// <summary>
        /// Retries a list of current data providers
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        Schemas.DataProviderType[] GetCPRDataProviderList(string userToken, string appToken);

        /// <summary>
        /// Updates the system to use the given list of data providers
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="dataProviders">The list of data providers that should be used from now on</param>
        /// <returns></returns>
        bool SetCPRDataProviderList(string userToken, string appToken, Schemas.DataProviderType[] dataProviders);
    }

    /// <summary>
    /// Contains methods that are related to the current capabilities of the system
    /// </summary>
    public interface IVersionManager : IDataProvider
    {
        /// <summary>
        /// Gets an object that contains all the capabilities of the current system
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        ServiceVersionType[] GetCapabilities(string userToken, string appToken);

        /// <summary>
        /// Check if this method exist in the current version
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="methodName">Method name to check its version</param>
        /// <param name="version">version number as string formated as(Num.Num) e.g "1.0"</param>
        /// <returns></returns>
        bool IsImplementing(string userToken, string appToken, string methodName, string version);
    }

    /// <summary>
    /// Contains methods related to logging
    /// </summary>
    public interface ILoggingDataProvider : IDataProvider
    {
        bool Log(string userToken, string appToken, string text);
    }

    /// <summary>
    /// Contains methods of the Part interface that need an existing UUID
    /// </summary>
    public interface IPartReadDataProvider : IDataProvider
    {
        PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql);
        PersonRegistration[] List(PersonIdentifier[] uuids, DateTime? effectDate, out QualityLevel? ql);
    }

    /// <summary>
    /// Contains the Search method of the Part interface
    /// </summary>
    public interface IPartSearchDataProvider : IDataProvider
    {
        Guid[] Search(PersonSearchCriteria searchCriteria, DateTime? effectDate, out QualityLevel? ql);
    }

    // Allows retrieval of person UUID from his CPR number
    public interface IPartPersonMappingDataProvider : IDataProvider
    {
        Guid GetPersonUuid(string cprNumber);
    }
}
