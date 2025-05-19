using Common;
using InGame.Model;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Inputを制御
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _model;
        private InputSystem_Actions _actionMap;

        public void Awake()
        {
            _actionMap = InputSystemActionsManager.Instance().GetInputSystem_Actions(); 
        }

        public void Update()
        {
            //Model.PlayerMove(ActionMap);
        }
    }   
}
