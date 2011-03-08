using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Data;

namespace CprBroker.Engine
{
    public interface IDataProvider
    {
        /// <summary>
        /// Ensure that the current provider is alive
        /// </summary>
        /// <returns></returns>
        bool IsAlive();

        /// <summary>
        /// GetPropertyValuesOfType the current version for this provider
        /// </summary>
        //TODO: Remove this property fromDate all data providers
        Version Version { get; }
    }

    /// <summary>
    /// Represents an external data provider (DPR, KMD)
    /// </summary>
    public interface IExternalDataProvider : IDataProvider
    {
        Dictionary<string, string> ConfigurationProperties { get; set; }
        DataProviderConfigPropertyInfo[] ConfigurationKeys { get; }
    }

    public class DataProviderConfigPropertyInfo
    {
        public string Name { get; set; }
        public bool Confidential { get; set; }
        public bool Required { get; set; }
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
        ApplicationType RequestAppRegistration(string name);

        /// <summary>
        /// Approves an application that has the given token
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="targetAppToken">Application token of the application to be approved</param>
        /// <returns></returns>
        bool ApproveAppRegistration(string targetAppToken);

        /// <summary>
        /// Returns a list of all applications
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        ApplicationType[] ListAppRegistration();

        /// <summary>
        /// Unregisters the application with the supplied token
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <param name="targetAppToken">Token of the application that will be removed</param>
        /// <returns></returns>
        bool UnregisterApp(string targetAppToken);
    }

    /// <summary>
    /// Contains methods related to data provider management
    /// </summary>
    public interface IDataProviderManager : IDataProvider
    {
        /// <summary>
        /// Retries a list of current data providers
        /// </summary>
        /// <returns></returns>
        Schemas.DataProviderType[] GetDataProviderList();

        /// <summary>
        /// Updates the system to use the given list of data providers
        /// </summary>
        /// <param name="dataProviders">The list of data providers that should be used fromDate now on</param>
        /// <returns></returns>
        bool SetDataProviderList(Schemas.DataProviderType[] dataProviders);
    }

    /// <summary>
    /// Contains methods that are related to the current capabilities of the system
    /// </summary>
    public interface IVersionManager : IDataProvider
    {
        /// <summary>
        /// Gets an object that contains all the capabilities of the current system
        /// </summary>
        /// <returns></returns>
        ServiceVersionType[] GetCapabilities();

        /// <summary>
        /// Check if this method exist in the current version
        /// </summary>
        /// <param name="userToken">Security Token for authorizing the current user</param>
        /// <param name="appToken">Security Token for the application the current application.</param>
        /// <returns></returns>
        bool IsImplementing(string methodName, string version);
    }

    /// <summary>
    /// Contains methods related to logging
    /// </summary>
    public interface ILoggingDataProvider : IDataProvider
    {
        bool Log(string text);
    }

    /// <summary>
    /// Contains methods of the Part interface that need an existing UUID
    /// </summary>
    public interface IPartReadDataProvider : IDataProvider
    {
        RegistreringType1 Read(PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql);
    }

    /// <summary>
    /// Contains the Search method of the Part interface
    /// </summary>
    public interface IPartSearchDataProvider : IDataProvider
    {
        Guid[] Search(SoegInputType1 searchCriteria);
    }

    /// <summary>
    /// Allows retrieval of person UUID from his CPR number
    /// </summary>
    public interface IPartPersonMappingDataProvider : IDataProvider
    {
        Guid? GetPersonUuid(string cprNumber);
    }

    /// <summary>
    /// Allows external systems to ping this system
    /// </summary>
    public interface IPingDataProvider : IDataProvider
    {
        bool Ping();
    }
}
