using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common
{
    /// <summary>
    /// Addressable‚ðŽg‚¤Žž‚Éusing‚³‚¹‚é
    /// </summary>
    /// <typeparam name="T"></typeparam>
    struct HandleDisposable<T> : IDisposable
    {
        private AsyncOperationHandle<T> Handle;

        // ctor
        public HandleDisposable(AsyncOperationHandle<T> handle) => Handle = handle;

        // IDisposable interface
        public void Dispose()
        {
            if (Handle.IsValid())
            {
                Handle.Release();
                Handle = default; //–³Œø’l
            }
        }
    }
}