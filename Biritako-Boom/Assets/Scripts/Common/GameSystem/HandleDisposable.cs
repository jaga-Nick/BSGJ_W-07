using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.GameSystem
{
    /// <summary>
    /// Addressableを使う時にusingさせる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    struct HandleDisposable<T> : IDisposable
    {
        private AsyncOperationHandle<T> _handle;

        // ctor
        public HandleDisposable(AsyncOperationHandle<T> handle) => _handle = handle;

        // IDisposable interface
        public void Dispose()
        {
            if (_handle.IsValid())
            {
                _handle.Release();
                _handle = default; //無効値
            }
        }
    }
}