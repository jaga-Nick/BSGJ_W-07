using Common;
using Cysharp.Threading.Tasks;
using InGame.Model;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Input
    /// </summary>
    public class PlayerController
    {
        //
        public PlayerController(PlayerModel playerModel)
        {
            Model = playerModel;

            InputSystemActionsManager manage=InputSystemActionsManager.Instance();
            ActionMap = manage.GetInputSystem_Actions();
            
        }

        private PlayerModel Model;
        private InputSystem_Actions ActionMap;

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
                Debug.Log("プラグを持つ");


            }
            if (ActionMap.Player.Have.WasReleasedThisFrame())
            {
                Debug.Log("プラグを刺す");
            }
            if (ActionMap.Player.Jump.WasPressedThisFrame()) 
            {
                Model.AddressGenerateSocket("").Forget();

                Debug.Log("コンセントを配置する");
            }
        }
        public void FixedUpdate()
        {
            Model.MovePlayer();
        }
    }   
}
