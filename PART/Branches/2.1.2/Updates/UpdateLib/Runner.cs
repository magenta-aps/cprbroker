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

using System.Security.Principal;
using System.Threading;

using GKApp2010.Common;
using GKApp2010.Core;
using GKApp2010.Threads;

namespace UpdateLib
{
    public delegate void HaltOperationDelegate(Exception ex, string auxMessage);

    // ================================================================================
    public class Runner : GKABase
    {
        private UpdateDetectionVariables _UpdateDetectionVariables = null;
        private HaltOperationDelegate _haltOperationFunc = null;

        private const int _WildRunningMinimumWaitInterval = 500;
        private WorkerThread _worker = null;
        private object _stoppedLock = new object();
        private bool _stopped = true;

        private IsStopRequestedFunc _stopRequestedFunc = null;

        private object _pollTimerLock = new object();
        private Timer _pollTimer = null;
        private int _pollInterval = 30; // In secs        

        private string _CPRBrokerServiceURL = "";
        private string _appToken = "";
        private string _runIdentity = "";

        private int _noOfBatchRunsWithAction = 0;
        private int _noOfBatchRuns = 0;

        private int _minorErrorThreshold = 15;
        private int _noOfMinorErrors = 0;

        #region constructors
        // -----------------------------------------------------------------------------
        public Runner(UpdateDetectionVariables updateDetectionVariables, HaltOperationDelegate haltOperationFunc)
        {
            _UpdateDetectionVariables = updateDetectionVariables;
            SetHaltOperation(haltOperationFunc);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        //// -----------------------------------------------------------------------------
        //public Runner(int pollInterval)
        //{
        //    _pollInterval = pollInterval;
        //}
        #endregion

        #region public methods and properties
        // -----------------------------------------------------------------------------
        public void SetCPRBrokerServiceURL(string serviceURL)
        {
            _CPRBrokerServiceURL = serviceURL;
        }

        // -----------------------------------------------------------------------------
        public void SetCPRBrokerAppToken(string appToken)
        {
            _appToken = appToken;
        }

        // -----------------------------------------------------------------------------
        public void Start()
        {
            try
            {
                // Read from konfig
                LoadMembersFromConfig();

                if (Disposed)
                {
                    throw new ObjectDisposedException("UpdateLib.Runner.Start()", "Object is disposed");
                }

                lock (_stoppedLock)
                {
                    if (_stopped == true)
                    {
                        LogHelper.LogToFile("-----------------------------------------------------------------------------");
                        LogHelper.LogToFile("INFO: Attempting to START " + _UpdateDetectionVariables.ServiceName + "...");

                        // Start process thread
                        _worker = new WorkerThread(DoOneBatch, _UpdateDetectionVariables.Tag);

                        _stopRequestedFunc = _worker.IsStopRequested;
                        _worker.Start();

                        _stopped = false;

                        LogHelper.LogToFile("INFO: CPRBroker service URL=[" + _CPRBrokerServiceURL + "]");
                        LogHelper.LogToFile("INFO: Application token=[" + _appToken + "]");
                        LogHelper.LogToFile("INFO: Runtime identity=[" + _runIdentity + "]");
                        LogHelper.LogToFile("INFO: Batch run attempted every " + _pollInterval.ToString() + " seconds!");

                        LogHelper.LogToFile("INFO: " + _UpdateDetectionVariables.ServiceName + "...");
                        LogHelper.LogToFile("INFO: ---");
                    }
                }

                // Force timer construction
                PollInterval = _pollInterval;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // -----------------------------------------------------------------------------
        public void Stop()
        {
            try
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException("UpdateLib.Runner.Stop()", "Object is disposed");
                }

                DisposePollTimer();

                lock (_stoppedLock)
                {
                    if (_stopped == false)
                    {
                        if (_worker != null)
                        {
                            _worker.Kill();
                        }

                        _stopped = true;

                        LogHelper.LogToFile("INFO: ---");
                        LogHelper.LogToFile("INFO: " + _UpdateDetectionVariables.ServiceName + " was stopped!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // -----------------------------------------------------------------------------
        public int PollInterval
        {
            get { return _pollInterval; }
            set
            {
                _pollInterval = value;

                lock (_pollTimerLock)
                {
                    if (_pollTimer == null)
                    {
                        _pollTimer = new Timer(PollTimeIsUp);
                    }

                    int timeInMSecs = (_pollInterval < 1) ? _WildRunningMinimumWaitInterval : _pollInterval * 1000;

                    _pollTimer.Change(timeInMSecs, timeInMSecs);
                }
            }
        }
        #endregion

        #region private methods
        // -----------------------------------------------------------------------------
        private void SetHaltOperation(HaltOperationDelegate haltOperationFunc)
        {
            _haltOperationFunc = haltOperationFunc;
        }

        // -----------------------------------------------------------------------------
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            _haltOperationFunc(ex, "*** FATAL: An uncaught exception was thrown from somewhere inside the UpdateLib.Runner class. ");
        }


        // -----------------------------------------------------------------------------
        private int DoOneBatch()
        {
            // TODO: Always read staging records from database just before calling CPR broker, to avoid calling deleted staging records
            UpdatedStagingBatch batch = null;
            List<string> updatedPersons = null;

            try
            {
                batch = new UpdatedStagingBatch(_UpdateDetectionVariables, _stopRequestedFunc);
                updatedPersons = batch.GetUpdatedPersonsList();

                int noOfPersonsProcessed = 0;
                foreach (string personCpr in updatedPersons)
                {
                    if (IsStopRequested())
                        break;

                    // Get person from CPRBroker, ignore data, and if succesfull, delete this person from staging
                    if (GetRefresh(personCpr))
                    {
                        batch.DeletePersonFromStaging(personCpr);
                        noOfPersonsProcessed++;
                        //LogHelper.LogToFile("*");

                        _noOfMinorErrors = 0;
                    }
                    else
                    {
                        throw new BrokerRequestException("*** ERROR: GetRefresh unexpected returned false. ", null);
                    }
                }

                // Write to log only, if one or more notfications happend
                if (noOfPersonsProcessed > 0)
                {
                    string msg = "INFO: A total of (" + noOfPersonsProcessed.ToString("0000") + ") update notifcations has been sent in batch (" + _noOfBatchRunsWithAction.ToString("00000") + "/" + _noOfBatchRuns.ToString("00000") + ")";
                    LogHelper.LogToFile(msg);

                    _noOfBatchRunsWithAction++;
                }
            }
            catch (GKAConfigException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                string msg = ExceptionMessageBuilder.Build("*** ERROR: An exception was thrown inside UpdateLib.Runner.DoOneBatch(). ", ex);
                LogHelper.LogToFile(msg);

                _noOfMinorErrors++;

                if (!(_noOfMinorErrors < _minorErrorThreshold))
                {
                    MailHelper.SendMessage(msg);
                }
            }

            _noOfBatchRuns++;

            //Console.WriteLine((_noOfBatchRuns++).ToString());

            return Timeout.Infinite;
        }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// This method actually calls the CPR-Broker service, to refresh() itself from database.
        /// </summary>
        /// <param name="cprNo"></param>
        /// <returns></returns>
        private bool GetRefresh(string cprNo)
        {
            bool success = false;

            try
            {
                CPRBroker.Part.ApplicationHeader headerValue = new CPRBroker.Part.ApplicationHeader();

                headerValue.ApplicationToken = _appToken;
                headerValue.UserToken = _runIdentity;

                CPRBroker.Part.Part client = new CPRBroker.Part.Part();

                client.Url = _CPRBrokerServiceURL;

                client.ApplicationHeaderValue = headerValue;

                client.UseDefaultCredentials = true;

                CPRBroker.Part.GetUuidOutputType uuidOutput = client.GetUuid(cprNo);
                if (uuidOutput.StandardRetur.StatusKode == "200")
                {
                    Guid g = new Guid(uuidOutput.UUID);

                    CPRBroker.Part.LaesInputType input = new CPRBroker.Part.LaesInputType();
                    input.UUID = g.ToString();

                    CPRBroker.Part.LaesOutputType outputType = client.RefreshRead(input);
                    CPRBroker.Part.StandardReturType stdRetur = outputType.StandardRetur;

                    if (outputType.StandardRetur.StatusKode == "200")
                    {
                        success = true;
                    }
                    else
                    {
                        string msg = string.Format("RefreshRead failed. {0} - {1}", outputType.StandardRetur.StatusKode, outputType.StandardRetur.FejlbeskedTekst);
                        throw new BrokerRequestException(msg);
                    }
                }
                else
                {
                    string msg = string.Format("GetUuid failed. {0} - {1}", uuidOutput.StandardRetur.StatusKode, uuidOutput.StandardRetur.FejlbeskedTekst);
                    throw new BrokerRequestException(msg);
                }
            }
            catch (Exception ex)
            {
                string msg = "Call to CPRBroker service (URL=[" + _CPRBrokerServiceURL + "]) failed inside UpdateLib.Runner.GetRefresh() .";
                throw new BrokerRequestException(msg, ex);
            }

            return success;
        }

        // -----------------------------------------------------------------------------
        private bool IsStopRequested()
        {
            return (_stopRequestedFunc != null ? _stopRequestedFunc() : false);
        }

        // -----------------------------------------------------------------------------
        private void DisposePollTimer()
        {
            lock (_pollTimerLock)
            {
                if (_pollTimer != null)
                {
                    _pollTimer.Dispose();
                    _pollTimer = null;
                }
            }
        }

        // -----------------------------------------------------------------------------
        private void PollTimeIsUp(Object stateInfo)
        {
            lock (_stoppedLock)
            {
                if (_worker != null)
                {
                    _worker.WakeUp();
                }
            }

        }

        // -----------------------------------------------------------------------------
        private void LoadMembersFromConfig()
        {
            _pollInterval = Properties.Settings.Default.PollInterval;
            _pollInterval = (_pollInterval < 1) ? 0 : _pollInterval;

            _CPRBrokerServiceURL = _CPRBrokerServiceURL.Trim();
            if (_CPRBrokerServiceURL.Length == 0)
            {
                _CPRBrokerServiceURL = Properties.Settings.Default.CPRBrokerPartServiceUrl;
            }

            _appToken = _appToken.Trim();
            if (_appToken.Length == 0)
            {
                _appToken = Properties.Settings.Default.ApplicationToken;
            }

            _runIdentity = System.Environment.MachineName + ":" + WindowsIdentity.GetCurrent().Name;

            // Force startup exception
            //int x = 0;
            //int y = 1 / x;
        }
        #endregion
    }
}
