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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PersonmasterServiceLibrary
{
    /// <summary>
    /// IBasicOp declares the standard operations exposed by the personmaster interface.
    /// </summary>
    [ServiceContract(Namespace = "http://personmaster.gentofte.dk/BasicOp/01")]
    public interface IBasicOp
    {
        #region GUID-CPR operations
        /// <summary>
        /// Creates a new object owner OR return an existing one. An object owner owns personmaster master records.
        /// If the owner already exists, the corresponding object owner ID is returned,
        /// UNLESS there is a MISMATCH between the specified objectOwnerId parameter and the guid found in the DB.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="nameSpace">Namespace of the object owner. Typical a URI like string (up to 500 chars in length), ie.
        /// gentofte.dk, dr.dk etc. Namespaces must be "worldwide" unique.</param>
        /// <param name="objectOwnerID">A caller supplied ID/guid for the new object owner. If this parameter
        /// is initialized with the Guid.Empty value, the operation creates the guid or return the guid from an existing owner record.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>Object owner guid is returned on success.</returns>
        [OperationContract]
        Guid CreateObjectOwner(string context, string nameSpace, Guid objectOwnerID, ref string aux);

        /// <summary>
        /// Retrieves the guid for the object owner with the specified nanmespace.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="nameSpace">Namespace of the object owner. Typical a URL like string (up to 500 chars in length). Namespaces must be "worldwide" unique.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>Object owner guid is returned on success.</returns>
        [OperationContract]
        Guid GetObjectOwnerIDFromNamespace(string context, string nameSpace, ref string aux);

        /// <summary>
        /// Retrieve the unique objectID attached to a specific person. If the person is not already
        /// assigned an objectID, one is created, persisted and returned. Therefore, this method will
        /// always succeed (under normal operation conditions) because of the lazy instantiation pattern used.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person to be found (10 digits precisely).</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The new or existing objectID/guid for the person in question.</returns>
        [OperationContract]
        Guid GetObjectIDFromCpr(string context, string cprNo, ref string aux);

        /// <summary>
        /// Retrieve the unique objectIDs attached to specific persons. If any person is not already
        /// assigned an objectID, one is created, persisted and returned. Therefore, this method will
        /// always succeed (under normal operation conditions) because of the lazy instantiation pattern used.
        /// Invalid cpr numbers will return Guid.Empty and will be reported in <paramref name="aux"/>
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNoArr">CPR number of the persons to be found (10 digits precisely for each). Method will fail if this is null or contains invalid elements</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The new or existing array of objectIDs/guids for the persons in question.</returns>
        [OperationContract]
        Guid?[] GetObjectIDsFromCprArray(string context, string[] cprNoArr, ref string aux);

        /// <summary>
        /// Retrieve the unique objectID attached to a specific person. Works in the same way as the
        /// GetObjectIDsFromCpr() operation above, except that a mandatory object owner ID,
        /// ie. the owner of the person master record, MUST be specified. The object owner ID value, is used
        /// if the person master record do not exists and as a consequence, is has to be created.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person to be found (10 digits precisely).</param>
        /// <param name="objectOwnerID">Object owner ID/guid of the owner, that should own the personmaster object.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The new or existing objectID/guid for the person in question.</returns>
        [OperationContract]
        Guid GetObjectIDFromCprWithOwner(string context, string cprNo, Guid objectOwnerID, ref string aux);

        /// <summary>
        /// Retrieve the CPR numbers attached to specific unique object IDs. Empty strings will be
        /// returned where no matches are found and this will also be reported in <param name="aux">.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNoArr">Object IDs of the persons to be found</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The an array of strings representing CPR numbers for the persons in question.</returns>
        [OperationContract]
        String[] GetCPRsFromObjectIDArray(string context, string[] objectIDArr, ref string aux);

        /// <summary>
        /// Retrieve the CPR numbers attached to specific unique object IDs. Empty strings will be
        /// returned where no matches are found and this will also be reported in <param name="aux">.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNoArr">Object IDs of the persons to be found</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The an array of strings representing CPR numbers for the persons in question.</returns>
        [OperationContract]
        String[] GetCPRsFromObjectIDList(string context, string objectIDArr, ref string aux);

        /// <summary>
        /// Retrieve the current CPR number attached to the specified objectID (iff one exists).
        /// Note that not all persons with a objectID might have a CPR number, ie. non-danish citizens etc.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="objectID">The objectID of the person, </param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The current CPR number attached to the objectID or the empty string iff no relation (mapping) exists.</returns>
        [OperationContract]
        string GetCprFromObjectID(string context, Guid objectID, ref string aux);
        #endregion

        #region CPR-loginname operations
        /// <summary>
        /// Establish a mapping between a persons CPR number and her corresponding user account loginname in
        /// the enterprice system/directory (ie Active Directory, etc). At present, only one system/directory is supported, but
        /// by using fully qualified loginnames, this restriction may be easily circumvented.
        /// Several loginnames may be mapped to one person, whereas NO account (loginname) can be mapped to multiple persons.
        /// Also, all loginnames must be unique within the enterprice (just as in the directory).
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person to be mapped to the directory user account.</param>
        /// <param name="loginName">Loginname of the user account to be mapped to the personmaster record.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        [OperationContract]
        void MapCpr2Loginname(string context, string cprNo, string loginName, ref string aux);

        /// <summary>
        /// Rename an existing loginname. If the loginname to be renamed dosen't exist,
        /// the operation completes silently without warnings.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="oldLoginName">Loginname to be renamed</param>
        /// <param name="newLoginName">Name of the new login</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        [OperationContract]
        void RenameLoginname(string context, string oldLoginName, string newLoginName, ref string aux);

        /// <summary>
        /// Deletes an existing loginname. If the loginname to be deleted renamed dosen't exist,
        /// the operation completes silently without warnings.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="loginName">Loginname to be deleted</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        [OperationContract]
        void DeleteLoginname(string context, string loginName, ref string aux);

        /// <summary>
        /// Evaluates wheather a loginname exists in the database. As opposed to the 
        /// GetPrimaryLoginnameFromCpr() operation, LoginnameExist may be called without
        /// CPR as searchkey.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="loginName">Loginname to be tested for existence</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>If the specified loginname exists, true is returned. Otherwise false is returned.</returns>
        [OperationContract]
        bool LoginnameExist(string context, string loginName, ref string aux);

        /// <summary>
        /// Retrieves the primary loginname for a specific person. At present, the primary loginname is defined
        /// to be the one, that was first mapped to a specific person (ie. the oldest one).
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person for which the primary loginname is requested.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The primary loginname is returned as a string. If no mapping exists, the empty string is returned.</returns>
        //[OperationContract]
        //string GetPrimaryLoginnameFromCpr(string context, string cprNo, ref string aux);

        /// <summary>
        /// Retrieves the preferred loginname for a specific person. By definition the preferred loginname is the login first
        /// registered for the person in question. Subsekvent CPR-AD mappings for the same person dosen't change the
        /// preferred login. To actullay change the preferred login, call the SetPreferredLoginname() operation below.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person for which the preferred loginname is requested.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The preferred loginname is returned as a string. If no mapping exists, the empty string is returned.</returns>
        [OperationContract]
        string GetPreferredLoginnameFromCpr(string context, string cprNo, ref string aux);

        /// <summary>
        /// Set a login to be the preferred one.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">CPR number of the person for which the preferred loginname should be specified in order
        /// to provide for future CPR validation, BUT verification of this parameter atm. is NOT done!</param>
        /// <param name="loginName">The new preferred loginname</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        [OperationContract]
        void SetPreferredLoginname(string context, string cprNo, string loginName, ref string aux);

        /// <summary>
        /// Retrieves ALL loginnames for a specific person. Works in the same way as GetPrimaryLoginnameFromCpr() operation.
        /// </summary>
        /// <param name="context">Not used at the moment.</param>
        /// <param name="cprNo">CPR number of the person for which ALL loginnames should be returned.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>Array of string with loginnames for the specified person. The item at index 0 in the array equals the primary loginname.
        /// If no mapping exists, a empty string array is returned.</returns>
        [OperationContract]
        string[] GetAllLoginnamesFromCpr(string context, string cprNo, ref string aux);

        /// <summary>
        /// Retrieves the CPR number for a person by using one of the persons loginnames.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="loginName">Loginname of the person for which the mapped CPR number should be returned.</param>
        /// <param name="aux">Standard auxiliary input/output.</param>
        /// <returns>The CPR number is returned as a string if a match was found. Otherwise, the empty string is returned.</returns>
        [OperationContract]
        string GetCprFromLoginname(string context, string loginName, ref string aux);
        #endregion

        /// <summary>
        /// Extract user, application, timing and other service status info. This operation may be used to monitor the availability and generel health of the service.
        /// </summary>
        /// <returns>Current thread user and misc timing info.</returns>
        [OperationContract]
        string Probe(string context, ref string aux);

        /// <summary>
        /// Query the DB for misc info, including DB versioning af runtime info.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="aux">Standard auxiliary input/output. On return from SP aux contains a XML <config/> element
        /// in which version, runtime, etc info is embedded as child elements</param>
        [OperationContract]
        void GetDBRuntimeInfo(string context, ref string aux);

        /// <summary>
        /// Register a person as a non administrative user, ie. a user with no AD account. This indicates,
        /// that the user has accepted/signed the rules, that give access to GK's LAN. 
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">The CPR number of the person, which should be registered as a non administrative user</param>
        /// <param name="aux">Standard auxiliary input/output</param>
        [OperationContract]
        void RegisterNonAdminUser(string context, string cprNo, ref string aux);

        /// <summary>
        /// Unregister a person as a non administrative user, ie. a user with no AD account
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">The CPR number of the person, which should be unregistered as a non administrative user</param>
        /// <param name="aux">Standard auxiliary input/output</param>
        [OperationContract]
        void UnRegisterNonAdminUser(string context, string cprNo, ref string aux);

        /// <summary>
        /// Evaluates if a person is a registered user.
        /// </summary>
        /// <param name="context">Not used atm.</param>
        /// <param name="cprNo">The CPR number of the person, which should be unregistered as a non administrative user</param>
        /// <param name="aux">Standard auxiliary input/output</param>
        /// <returns>True if the user is registered as a non administrative user. False otherwise.</returns>
        [OperationContract]
        bool IsRegisteredNonAdminUser(string context, string cprNo, ref string aux);

    }
}
