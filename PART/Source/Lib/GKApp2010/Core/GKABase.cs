//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;

namespace GKApp2010.Core
{
    // ================================================================================
    public class GKABase : IDisposable
    {
        private object _disposedLock = new object();
        private bool _disposed = false;

        #region constructors
        // -----------------------------------------------------------------------------
        public GKABase()
        {
        }

        // -----------------------------------------------------------------------------
        ~GKABase()
        {
            Cleanup();
        }
        #endregion

        #region public and protected properties
        // -----------------------------------------------------------------------------
        public bool Disposed
        {
            get
            {
                lock (_disposedLock)
                {
                    return _disposed;
                }
            }
        }
        #endregion

        #region public methods
        // -----------------------------------------------------------------------------
        public void Dispose()
        {
            lock (_disposedLock)
            {
                if (_disposed == false)
                {
                    Cleanup();
                    _disposed = true;

                    // No reason to call finalize
                    GC.SuppressFinalize(this);
                }
            }
        }
        #endregion

        #region private and protected methods
        // -----------------------------------------------------------------------------
        protected virtual void Cleanup()
        {
        }
        #endregion
    }
}
