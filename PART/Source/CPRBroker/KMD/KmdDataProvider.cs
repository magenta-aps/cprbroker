using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Providers.KMD.WS_AN08002;
using CprBroker.Schemas;
using CprBroker.DAL;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider : IDataProvider, IExternalDataProvider
    {

        public enum ServiceTypes
        {
            AN08002,
            AN08010,
            AS78205,
            AS78206,
            AS78207,
            AN08300,
            AN08100,
        }
        #region PrivateMethods
        /// <summary>
        /// Sets the KMD web service url to the correct value based on the database object and the service name
        /// </summary>
        /// <param name="service">Web service proxy object</param>
        /// <param name="serviceType">Type of service</param>
        private void SetServiceUrl(System.Web.Services.Protocols.SoapHttpClientProtocol service, ServiceTypes serviceType)
        {
            string query = string.Format("?zservice={0}", serviceType);
            string url = Address.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0];
            url += query;
            service.Url = url;
        }

        /// <summary>
        /// Filters the list of a person's relations based on the requested relation type
        /// </summary>
        /// <typeparam name="TRelation">Type of OIO relation object</typeparam>
        /// <param name="persons">Related persons returned fromDate KMD service</param>
        /// <param name="typeFilters">The type codes that will be used to filter the relations</param>
        /// <returns>Filtered list of OIO relations</returns>
        private TRelation[] GetPersonRelations<TRelation>(WS_AN08010.ReplyPerson[] persons, params string[] typeFilters) where TRelation : BaseRelationshipType, new()
        {
            if (persons == null)
            {
                persons = new WS_AN08010.ReplyPerson[0];
            }

            var filteredPersons = from person in persons
                                  where Array.IndexOf<string>(typeFilters, person.Type) != -1
                                  && person.IsUnknown == false
                                  select person;
            List<TRelation> ret = new List<TRelation>();
            foreach (var person in filteredPersons)
            {
                SimpleCPRPersonType oioPerson = person.ToSimpleCprPerson();
                if (oioPerson.PersonNameStructure.IsEmpty)
                {
                    var an08002 = CallAN08002(oioPerson.PersonCivilRegistrationIdentifier);
                    oioPerson = an08002.ToSimpleCprPerson();
                }
                ret.Add(new TRelation() { SimpleCPRPerson = oioPerson });
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Searches for the return code in a list of error codes, throws an Exception if a match is found
        /// </summary>
        /// <param name="returnCode">Code returned fromDate web service</param>
        /// <param name="returnText">Text returned fromDate the web service, used as the Exception's message if thrown</param>
        private void ValidateReturnCode(string returnCode, string returnText)
        {
            string[] errorCodes = new string[] 
            {
                //"00",//	Everything ok
                "07",//	Person is unknown in the municipality
                "08",//	Person is unknown in the region
                "10",//	The person is inactive -- moved fromDate region
                //"15",//	Person number is invalid - former double issue
                //"16",//	Person number is invalid - the person is nynummereret
                //"17",//	The person is inactive - disappeared
                //"18",//	The person is inactive - emigrate
                //"19",//	The person is inactive - dead
                "22",//	The person is unknown in CPR
                "50",//	Bind error - contact DBA
                "51",//	Bind error - contact DBA
                "52",//	Bind error - contact DBA
                "53",//	Problems with connection to CPR - try again later
                "54",//	There are currently unable to carry on CPR
                "55",//	There is no such CICS through to DC. '
                "70",//municipal code / personal identification number is not numeric
                "78",// Error in personal / replacement personal
            };
            if (Array.IndexOf<string>(errorCodes, returnCode) != -1)
            {
                throw new Exception(returnText);
            }
        }
        #endregion

        #region Conversion methods

        //TODO: Remove this method
        private PersonDeathDateStructureType ToDeathDate(PersonCivilRegistrationStatusStructureType status)
        {
            if (status != null
                && status.PersonCivilRegistrationStatusCode == PersonCivilRegistrationStatusCodeType.Item90 // dead
                )
            {
                return new PersonDeathDateStructureType()
                {
                    PersonDeathDate = status.PersonCivilRegistrationStatusStartDate,
                    PersonDeathDateUncertaintyIndicator = false,
                };
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            try
            {
                System.Uri uri = new Uri(Address);
                uri = new Uri(uri, "wsdl/AN08002.wsdl");
                client.DownloadData(uri);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }

        public Version Version
        {
            get
            {
                return new Version(Versioning.Major, Versioning.Minor);
            }
        }

        #endregion

        #region IExternalDataProvider Members

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] 
                { 
                    new DataProviderConfigPropertyInfo(){Name="Address",Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Username",Required=true,Confidential=false},
                    new DataProviderConfigPropertyInfo(){Name="Password" ,Required=true,Confidential=true}
                };
            }
        }

        #endregion

        #region Configuration properties

        public string Address
        {
            get
            {
                return ConfigurationProperties["Address"];
            }
        }

        public string UserName
        {
            get
            {
                return ConfigurationProperties["Username"];
            }
        }

        public string Password
        {
            get
            {
                return ConfigurationProperties["Password"];
            }
        }

        #endregion
    }
}
