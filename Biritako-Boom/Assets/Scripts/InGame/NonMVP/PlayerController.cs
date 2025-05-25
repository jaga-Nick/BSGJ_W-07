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
        private PlayerModel Model;
        private InputSystem_Actions ActionMap;

        public void Awake()
        {
            ActionMap = InputSystemActionsManager.Instance().GetInputSystem_Actions(); 
        }

        public void Update()
        {
            //移動処理
            Model.MoveInput(ActionMap);

            if (ActionMap.Player.Attack.WasPressedThisFrame())
            {
                Debug.Log("爆破する。");
            }
            if (ActionMap.Player.Have.WasPressedThisFrame())
            {
                Debug.Log("コンセントを持つ");
            }
            if (ActionMap.Player.Have.WasReleasedThisFrame())
            {
                Debug.Log("コンセントを置く");
            }
        }
        public void FixedUpdate()
        {
            Model.MovePlayer();
        }
    }   
}
