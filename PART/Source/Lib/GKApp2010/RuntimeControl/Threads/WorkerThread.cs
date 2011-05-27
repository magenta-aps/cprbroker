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
using System.Diagnostics;
using System.Threading;

using GKApp2010.Core;

namespace GKApp2010.Threads
{
    // ================================================================================
    public delegate int DoPieceOfWorkHandler();

    // ================================================================================
    public delegate bool IsStopRequestedFunc();

    // ================================================================================
    public class WorkerThread : GKApp2010.Core.GKABase
    {
        private Guid _instanceGuid = Guid.NewGuid();

        private object _endLoopLock = new object();
        private bool _endLoop = false;

        private ManualResetEvent _threadHandle = new ManualResetEvent(false);
        private Thread _threadObj = null;

        private AutoResetEvent _moreWorkTodoHandle = new AutoResetEvent(true);

        private DoPieceOfWorkHandler _doPieceOfWorkHandler = null;

        private object _startedLock = new object();
        private bool _started = false;

        #region constructors
        // -----------------------------------------------------------------------------
        public WorkerThread(DoPieceOfWorkHandler doPieceOfWorkHandler)
        {
            CommonInit(doPieceOfWorkHandler, "", false);
        }

        // -----------------------------------------------------------------------------
        public WorkerThread(DoPieceOfWorkHandler doPieceOfWorkHandler, string tag)
        {
            CommonInit(doPieceOfWorkHandler, tag, false);
        }

        // -----------------------------------------------------------------------------
        public WorkerThread(DoPieceOfWorkHandler doPieceOfWorkHandler, string tag, bool autoStart)
        {
            CommonInit(doPieceOfWorkHandler, tag, autoStart);
        }

        // -----------------------------------------------------------------------------
        private void CommonInit(DoPieceOfWorkHandler doPieceOfWorkHandler, string tag, bool autoStart)
        {
            _doPieceOfWorkHandler = doPieceOfWorkHandler;

            _threadObj = new Thread(Run);
            _threadObj.Name = BuildTag(tag);

            if (autoStart)
            {
                Start();
            }
        }
        #endregion

        #region public properties
        // -----------------------------------------------------------------------------
        public int ManagedThreadId
        {
            get { return _threadObj.ManagedThreadId; }
        }

        // -----------------------------------------------------------------------------
        public Thread Thread
        {
            get { return _threadObj; }
        }

        // -----------------------------------------------------------------------------
        public WaitHandle Handle
        {
            get { return _threadHandle; }
        }

        // -----------------------------------------------------------------------------
        public string Name
        {
            get { return _threadObj.Name; }
        }

        // -----------------------------------------------------------------------------
        public bool IsAlive
        {
            get
            {
                return _threadObj.IsAlive;
            }
        }
        #endregion

        #region public methods
        // -----------------------------------------------------------------------------
        public void Start()
        {
            lock (_startedLock)
            {
                if (_started == false)
                {
                    Debug.Assert(_threadObj != null);

                    _threadObj.Start();

                    _started = true;
                }
            }
        }

        // -----------------------------------------------------------------------------
        public void Stop()
        {
            lock (_startedLock)
            {
                if (_started == true)
                {
                    Debug.Assert(_threadObj != null);

                    if (IsAlive == false)
                    {
                        return;
                    }

                    EndLoop = true;

                    _moreWorkTodoHandle.Set();

                    _started = false;
                }
            }
        }

        // -----------------------------------------------------------------------------
        public void WakeUp()
        {
            // Activate thread
            _moreWorkTodoHandle.Set();

            // Allow other threads to execute, setting to 1, prevents starvation of lower priority threads
            Thread.Sleep(1);
        }

        // -----------------------------------------------------------------------------
        public void GotoSleepASAP()
        {
            // Deactivate thread
            _moreWorkTodoHandle.Reset();

            // Allow other threads to execute, setting to 1, prevents starvation of lower priority threads
            Thread.Sleep(1);
        }

        // -----------------------------------------------------------------------------
        public void Kill()
        {
            Debug.Assert(_threadObj != null);

            if (IsAlive == false)
            {
                return;
            }

            Stop();

            Join();

            _threadHandle.Close();

            _moreWorkTodoHandle.Close();
        }

        // -----------------------------------------------------------------------------
        public bool Join()
        {
            return Join(Timeout.Infinite);
        }

        // -----------------------------------------------------------------------------
        public bool Join(int millisecondsTimeout)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            return Join(timeout);
        }

        // -----------------------------------------------------------------------------
        public bool Join(TimeSpan timeout)
        {
            Debug.Assert(_threadObj != null);

            if (IsAlive == false)
            {
                return true;
            }

            Debug.Assert(Thread.CurrentThread.ManagedThreadId != _threadObj.ManagedThreadId);

            return _threadObj.Join(timeout);
        }

        // -----------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return _threadObj.GetHashCode();
        }

        // -----------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            return _threadObj.Equals(obj);
        }

        // -----------------------------------------------------------------------------
        public bool IsStopRequested()
        {
            return EndLoop;
        }
        #endregion

        #region private and protected methods
        // -----------------------------------------------------------------------------
        private void Run()
        {
            int waitBeforeNextPieceOfWorkTimeout = Timeout.Infinite;

            try
            {
                while (EndLoop == false)
                {
                    try
                    {
                        bool b = _moreWorkTodoHandle.WaitOne(waitBeforeNextPieceOfWorkTimeout);

                        if (EndLoop == false)
                        {
                            waitBeforeNextPieceOfWorkTimeout = DoOnePieceOfWork();
                        }

                    }
                    catch (Exception ex)
                    {
                        // Happens on service breakdown, piece of work failures should be caught in do-piece-of-works delegate (and reported from there)
                        // Exceptions here is interpreted as unhandled illegal state, and MUST result in application termination

                        string msg = "Unhandled exception thrown inside GKApp2010.Threads.WorkerThread.Run(). Proces will be terminated! ";

                        GKAApplicationException appException = new GKAApplicationException(msg, ex);
                        appException.TerminateApplication = true;

                        throw appException;
                    }
                }
            }
            finally
            {
                _threadHandle.Set();
            }
        }

        // -----------------------------------------------------------------------------
        private int DoOnePieceOfWork()
        {
            // Do something ...
            if (_doPieceOfWorkHandler != null)
            {
                // ... through delegate
                return _doPieceOfWorkHandler();
            }
            else
            {
                // ... or by calling inherited method
                return ExecutePieceOfWork();
            }
        }

        // -----------------------------------------------------------------------------
        protected virtual int ExecutePieceOfWork()
        {
            return Timeout.Infinite;
        }

        // -----------------------------------------------------------------------------
        protected override void Cleanup()
        {
            try
            {
                Kill();
            }
            finally
            {
                base.Cleanup();
            }
        }

        // -----------------------------------------------------------------------------
        private string BuildTag(string tag)
        {
            string s = "";

            if (tag == null) { tag = ""; }
            tag = tag.Trim();

            s += this.GetType().ToString() + "_";
            s += (tag == "") ? "" : tag + "_";
            s += _instanceGuid.ToString("N");

            return s;
        }

        // -----------------------------------------------------------------------------
        private bool EndLoop
        {
            set
            {
                lock (_endLoopLock)
                {
                    _endLoop = value;
                }
            }
            get
            {
                lock (_endLoopLock)
                {
                    return _endLoop;
                }
            }
        }
        #endregion
    }
}
