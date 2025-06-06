using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common
{
    /// <summary>
    /// Addressableを使う時にusingさせる
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
                Handle = default; //無効値
            }
        }
    }
}