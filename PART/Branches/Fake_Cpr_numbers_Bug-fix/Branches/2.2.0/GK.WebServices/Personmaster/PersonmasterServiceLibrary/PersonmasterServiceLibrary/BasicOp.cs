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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

using System.Security.Principal;
using System.Threading;

using GKApp2010.DB;
using GKApp2010.RTE;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PersonmasterServiceLibrary
{
    /// <summary>
    /// BasicOp implements the IBasicOp interface againt the PersonMaster DB model
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]    
    public class BasicOp : IBasicOp
    {
        #region GUID-CPR operations
        // ================================================================================
        public Guid CreateObjectOwner(string context, string nameSpace, Guid objectOwnerID, ref string aux)
        {
            //CREATE PROCEDURE spGK_PM_GetOwnerIDFromNamespace
            //    @context                    VARCHAR(120),
            //    @objectOwnerID              uniqueidentifier    OUTPUT,
            //    @objectOwnerNamespace       VARCHAR(510),
            //    @aux                        VARCHAR(1020)       OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);
            aux = "CREATE-ON-NON-EXISTENCE";

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_GetOwnerIDFromNamespace", context, aux);

            spContext.AddInParameter("objectOwnerNamespace", DbType.String, nameSpace.Trim());
            spContext.AddInOutParameter("objectOwnerID", DbType.Guid, null);

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return spContext.GetParameterGuidValue("objectOwnerID");
        }

        // ================================================================================
        public Guid GetObjectOwnerIDFromNamespace(string context, string nameSpace, ref string aux)
        {
            //CREATE PROCEDURE spGK_PM_GetOwnerIDFromNamespace
            //    @context                    VARCHAR(120),
            //    @objectOwnerID              uniqueidentifier    OUTPUT,
            //    @objectOwnerNamespace       VARCHAR(510),
            //    @aux                        VARCHAR(1020)       OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_GetOwnerIDFromNamespace");

            spContext.AddInParameter("objectOwnerNamespace", DbType.String, nameSpace.Trim());
            spContext.AddOutParameter("objectOwnerID", DbType.Guid);

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return spContext.GetParameterGuidValue("objectOwnerID");
        }

        // ================================================================================
        public Guid GetObjectIDFromCpr(string context, string cprNo, ref string aux)
        {
            var ret = this.GetObjectIDsFromCprArrayImpl(context, new string[] { cprNo }, Guid.Empty, ref aux)[0];
            return ret == null ? Guid.Empty : ret.Value;
        }

        // ================================================================================
        public Guid?[] GetObjectIDsFromCprArray(string context, string[] cprNoArr, ref string aux)
        {
            return this.GetObjectIDsFromCprArrayImpl(context, cprNoArr, Guid.Empty, ref aux);
        }

        // ================================================================================
        public String[] GetCPRsFromObjectIDArray(string context, string[] objectIDArr, ref string aux)
        {
            return this.GetCPRsFromObjectIDArrayImpl(context, objectIDArr, ref aux);
        }

        // ================================================================================
        // This method is mainly meant for testing GetCPRsFromObjectIDArray with a commaseparated String
        public String[] GetCPRsFromObjectIDList(string context, string objectIDArr, ref string aux)
        {
            return this.GetCPRsFromObjectIDArray(context, objectIDArr.Split(','), ref aux);
        }

        // ================================================================================
        public Guid GetObjectIDFromCprWithOwner(string context, string cprNo, Guid objectOwnerID, ref string aux)
        {
            var ret = this.GetObjectIDsFromCprArrayImpl(context, new string[] { cprNo }, objectOwnerID, ref aux)[0];
            return ret == null ? Guid.Empty : ret.Value;
        }

        // ================================================================================
        public string GetCprFromObjectID(string context, Guid objectID, ref string aux)
        {
            //CREATE PROCEDURE spGK_PM_GetCPRFromObjectID
            //    @context            VARCHAR(120),
            //    @objectID           uniqueidentifier,
            //    @cprNo              VARCHAR(10)         OUTPUT,
            //    @aux                VARCHAR(1020)       OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_GetCPRFromObjectID");

            spContext.AddInParameter("objectID", DbType.Guid, objectID);
            spContext.AddOutParameter("cprNo", DbType.String, 10);

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return spContext.GetParameterStringValue("cprNo");
        }
        #endregion

        #region CPR-loginname operations
        // ================================================================================
        public void MapCpr2Loginname(string context, string cprNo, string loginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_SetCPRLoginNameMap
            //    @context            VARCHAR(120),
            //    @cprNo              VARCHAR(10),
            //    @loginName          VARCHAR(30),
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_SetCPRLoginNameMap");

            spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());
            spContext.AddInParameter("loginName", DbType.String, loginName.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
        }

        // ================================================================================
        public void RenameLoginname(string context, string oldLoginName, string newLoginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_RenameLoginName
            //    @context            VARCHAR(120),
            //    @oldLoginName       VARCHAR(30),
            //    @newLoginName       VARCHAR(30),
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_RenameLoginName");

            spContext.AddInParameter("oldLoginName", DbType.String, oldLoginName.Trim());
            spContext.AddInParameter("newLoginName", DbType.String, newLoginName.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
        }

        // ================================================================================
        public void DeleteLoginname(string context, string loginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_DeleteLoginName
            //    @context            VARCHAR(120),
            //    @loginName          VARCHAR(30),
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_DeleteLoginName");

            spContext.AddInParameter("loginName", DbType.String, loginName.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
        }

        // ================================================================================
        public bool LoginnameExist(string context, string loginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_LoginNameExist
            //    @context            VARCHAR(120),
            //    @loginName          VARCHAR(30),
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_LoginNameExist");

            spContext.AddInParameter("loginName", DbType.String, loginName.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return (spContext.ReturnValue == 1);
        }

        // ================================================================================
        //public string GetPrimaryLoginnameFromCpr(string context, string cprNo, ref string aux)
        //{
        //    string result = "";

        //    result = GetPreferredLoginnameFromCpr(context, cprNo, ref aux);

        //    aux += "This operation is deprecated (as of 20110106). Use operation GetPreferredLoginnameFromCpr() instead! ";

        //    return result;
        //}

        // ================================================================================
        public string GetPreferredLoginnameFromCpr(string context, string cprNo, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_GetLoginNameFromCPR
            //    @context            VARCHAR(120),
            //    @cprNo              VARCHAR(10),
            //    @loginName          VARCHAR(30)     OUTPUT,
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_GetLoginNameFromCPR");

            spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());
            spContext.AddOutParameter("loginName", DbType.String, 30);

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return spContext.GetParameterStringValue("loginName");
        }

        // ================================================================================
        public void SetPreferredLoginname(string context, string cprNo, string loginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_SetPreferredUA
            //    @context            VARCHAR(120),
            //    @loginName          VARCHAR(30),
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_SetPreferredUA");

            //spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());   TODO Not used atm
            spContext.AddInParameter("loginName", DbType.String, loginName.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
        }

        // ================================================================================
        public string[] GetAllLoginnamesFromCpr(string context, string cprNo, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_GetLoginNameFromCPR
            //    @context            VARCHAR(120),
            //    @cprNo              VARCHAR(10),
            //    @loginName          VARCHAR(30)     OUTPUT,
            //    @aux                VARCHAR(1020)   OUTPUT

            string[] sa = new string[0];

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_GetLoginNameFromCPR");

            spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());
            spContext.AddOutParameter("loginName", DbType.String, 30);

            DataSet ds = spContext.ExecuteDataSet();
            if (ds != null)
            {
                int tblCnt = ds.Tables.Count;
                if (tblCnt > 0)
                {
                    // It's the last table that contains login names
                    DataTable dt = ds.Tables[tblCnt - 1];
                    if (dt != null)
                    {
                        int rowCnt = dt.Rows.Count;
                        sa = new string[rowCnt];
                        for (int i = 0; i < rowCnt; i++)
                        {
                            sa[i] = (string)dt.Rows[i]["loginName"];
                        }
                    }
                }
            }

            aux = spContext.Aux;
            return sa;
        }

        // ================================================================================
        public string GetCprFromLoginname(string context, string loginName, ref string aux)
        {
            //CREATE PROCEDURE spGK_PMU_GetCPRFromLoginName
            //    @context            VARCHAR(120),
            //    @loginName          VARCHAR(30),
            //    @cprNo              VARCHAR(10)     OUTPUT,
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PMU_GetCPRFromLoginName");

            spContext.AddInParameter("loginName", DbType.String, loginName.Trim());
            spContext.AddOutParameter("cprNo", DbType.String, 10);

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return spContext.GetParameterStringValue("cprNo");
        }
        #endregion

        // ================================================================================
        public string Probe(string context, ref string aux)
        {
            //CREATE PROCEDURE spGK_PM_Probe
            //    @context        VARCHAR(120),
            //    @aux            VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            try
            {
                string result = "";

                IPrincipal threadPrincipal = Thread.CurrentPrincipal;
                IIdentity principalIdentity = threadPrincipal.Identity;

                result += "Probe execution context=[" + principalIdentity.Name + "], principal is authenticated=[" + principalIdentity.IsAuthenticated.ToString() + "] with authenticationtype=[" + principalIdentity.AuthenticationType + "].";

                result += "\n\nRuntime info=[" + Info.GetRuntimeInfo() + "].";
                result += "\nGK framework runtime info=[" + Info.GetGKAppRuntimeInfo() + "].";

                DateTime then = DateTime.Now;

                StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_Probe");

                int recCount = spContext.ExecuteNonQueryWithReturnValue();
                aux = spContext.Aux;

                TimeSpan dbQueryTime = DateTime.Now - then;

                result += "\n\nPersonmaster DB=[" + aux + "], no of personmaster records=[" + recCount.ToString() + "], querytime (ms)=[" + dbQueryTime.Milliseconds.ToString("000000") + "], SP return value=[" + spContext.ReturnValue.ToString() + "].";

                return result;
            }
            catch (Exception ex)
            {
                string innerMsg = "null";
                if (ex.InnerException != null)
                {
                    innerMsg = ex.InnerException.Message;
                }

                throw new Exception("Probe() call failed, message=[" + ex.Message + "], inner exception message=[" + innerMsg + "]");
            }
        }

        // ================================================================================
        public void GetDBRuntimeInfo(string context, ref string aux)
        {
            //CREATE PROCEDURE spGK_CORE_GetRuntimeInfo
            //    @context        VARCHAR(120),
            //    @aux            VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_CORE_GetRuntimeInfo");
            int retVal = spContext.ExecuteNonQueryWithReturnValue();
            aux = spContext.Aux;
        }

        // ================================================================================
        public void RegisterNonAdminUser(string context, string cprNo, ref string aux)
        {
            SetRegisterNonAdminUser(context, cprNo, true, ref aux);
        }

        // ================================================================================
        public void UnRegisterNonAdminUser(string context, string cprNo, ref string aux)
        {
            SetRegisterNonAdminUser(context, cprNo, false, ref aux);
        }

        // ================================================================================
        public bool IsRegisteredNonAdminUser(string context, string cprNo, ref string aux)
        {
            //CREATE PROCEDURE spGK_N2L_IsRegisteredNonAdminUser
            //    @context            VARCHAR(120),
            //    @cprNo              VARCHAR(10),
            //    @registered         INTEGER         OUTPUT,
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_N2L_IsRegisteredNonAdminUser");

            spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());
            spContext.AddOutParameter("registered", DbType.Int32);

            int registered = spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
            return (registered == 1);
        }

        #region Private methods
        // ================================================================================
        //private void ClearStandardParams(ref string context, ref string aux)
        //{
        //    context = string.Empty;
        //    aux = string.Empty;
        //}

        // ================================================================================
        private Guid?[] GetObjectIDsFromCprArrayImpl(string context, string[] cprNoArr, Guid objectOwnerID, ref string aux)
        {
            //CREATE PROCEDURE spGK_PM_GetObjectIDsFromCPRArray
            //    @context            VARCHAR(1020),
            //    @cprNoArray              VARCHAR(MAX),
            //    @objectOwnerID      uniqueidentifier,
            //    @aux                VARCHAR(1020)       OUTPUT


            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_GetObjectIDsFromCPRArray");

            var cprNoArrComma = Array.ConvertAll<string, string>(cprNoArr, s => string.Format("{0}", s).Trim().Replace(",", "") + ",");
            spContext.AddInParameter("cprNoArray", DbType.String, string.Join("", cprNoArrComma));

            if (objectOwnerID == Guid.Empty)
            {
                spContext.AddInParameter("objectOwnerID", DbType.Guid, null);
            }
            else
            {
                spContext.AddInParameter("objectOwnerID", DbType.Guid, objectOwnerID);
            }
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var dataSet = spContext.ExecuteDataSet();
            var returnTable = dataSet.Tables[0];
            var ret = new Guid?[returnTable.Rows.Count];
            for (int iRow = 0; iRow < returnTable.Rows.Count; iRow++)
            {
                var row = returnTable.Rows[iRow];
                if (row.IsNull("ObjectID"))
                {
                    string error = row["Aux"] as string;
                    string cprNumber = row["CprNo"] as string;
                    if (!errors.ContainsKey(error))
                    {
                        errors.Add(error, new List<string>());
                    }
                    errors[error].Add(cprNumber);
                }
                else
                {
                    ret[iRow] = (Guid)row["ObjectID"];
                }
            }
            aux = string.Join(
                Environment.NewLine,
                errors.Select(e => string.Format("{0}: {1}", e.Key, string.Join(", ", e.Value)))
                );
            return ret;
        }

        // ================================================================================
        private String[] GetCPRsFromObjectIDArrayImpl(string context, string[] objectIDArr, ref string aux)
        {
            /*
             *  CREATE PROCEDURE [dbo].[spGK_PM_GetCPRsFromObjectIDArray]
             *      @context            VARCHAR(1020),
             *      @objectIDArray      VARCHAR(MAX),
	         *      @aux                VARCHAR(1020)       OUTPUT	
             */


            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_PM_GetCPRsFromObjectIDArray");

            // We check if all Strings are indead Guids. In opposite situations we set an empty String at the given position(s)
            Regex isGuid = new Regex(@"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$", RegexOptions.Compiled);
            for (int i = 0; i < objectIDArr.Length; i++)
            {
                if (objectIDArr[i] != null)
                {
                    if (objectIDArr[i].Length != 36)
                        objectIDArr[i] = "";
                    else if (!isGuid.IsMatch(objectIDArr[i]))
                        objectIDArr[i] = "";
                }
                else
                    objectIDArr[i] = "";
            }
            var objectIDArrComma = Array.ConvertAll<string, string>(objectIDArr, s => string.Format("{0}", s).Trim().Replace(",", "") + ",");
            String commSepString = string.Join("", objectIDArrComma);
            // Checking output:
            Debug.WriteLine("Debug objectID array: " + commSepString);
            spContext.AddInParameter("objectIDArray", DbType.String, string.Join("", objectIDArrComma));
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var dataSet = spContext.ExecuteDataSet();
            var returnTable = dataSet.Tables[0];
            var ret = new String[returnTable.Rows.Count];
            for (int iRow = 0; iRow < returnTable.Rows.Count; iRow++)
            {
                var row = returnTable.Rows[iRow];
                if (row.IsNull("CprNo"))
                {
                    string error = row["Aux"] as string;
                    string objectID = row["ObjectID"] as string;
                    if (!errors.ContainsKey(error))
                    {
                        errors.Add(error, new List<string>());
                    }
                    errors[error].Add(objectID);
                }
                else
                {
                    ret[iRow] = (String)row["CprNo"];
                }
            }
            aux = string.Join(
                Environment.NewLine,
                errors.Select(e => string.Format("{0}: {1}", e.Key, string.Join(", ", e.Value)))
                );
            return ret;
        }

        // ================================================================================
        private void SetRegisterNonAdminUser(string context, string cprNo, bool value, ref string aux)
        {
            //CREATE PROCEDURE spGK_N2L_RegisterNonAdminUser
            //    @context            VARCHAR(120),
            //    @cprNo              VARCHAR(10),
            //    @register           INTEGER,
            //    @aux                VARCHAR(1020)   OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("CPRMapperDB", "spGK_N2L_RegisterNonAdminUser");

            spContext.AddInParameter("cprNo", DbType.String, cprNo.Trim());
            spContext.AddInParameter("register", DbType.Int32, (value ? 1 : 0));

            spContext.ExecuteNonQueryWithReturnValue();

            aux = spContext.Aux;
        }
        #endregion
    }
}
