using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Common;
using System;
using System.Threading;
using UnityEngine.InputSystem;
namespace InGame
{
    /// <summary>
    /// 
    /// </summary>
    public class Player_View : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private GameObject Character;

        private void Awake()
        {
            _inputActions = InputSystemActionsManager.Instance().GetInputSystem_Actions();
        }
    }
}