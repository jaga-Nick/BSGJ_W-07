using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using InGame.Model;
using InGame.Presenter;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Input
    /// </summary>
    public class PlayerController
    {
        //
        public PlayerController(PlayerModel playerModel,PlayerPresenter playerPresenter)
        {
            Model = playerModel;
            this.Presenter = playerPresenter;

            InputSystemActionsManager manage=InputSystemActionsManager.Instance();
            ActionMap = manage.GetInputSystem_Actions();
            
        }
        private PlayerPresenter Presenter;
        private PlayerModel Model;
        private InputSystem_Actions ActionMap;
        private ComponentChecker checker=new ComponentChecker();

        public void Init()
        {
            InputSystemActionsManager manager = new InputSystemActionsManager();
            ActionMap=manager.GetInputSystem_Actions();
            manager.PlayerEnable();
        }

        public void Update()
        {
            //移動処理
            Model.MoveInput(ActionMap);

            //爆発
            if (ActionMap.Player.Attack.WasPressedThisFrame())
            {
                Model.Explosion();
            }

            //コードを保持する
            if (ActionMap.Player.Have.WasPressedThisFrame())
            {
                //コードを生成-保持する為の処理
                Model.OnHave();
            }

            //ーーー
            if (ActionMap.Player.Have.WasReleasedThisFrame())
            {
                if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 1f) != null)
                {
                    //Debug.Log("プラグを刺す");
                    Model.ConnectSocketCode();

                }
                //範囲内にコードがない場合(それで保持している時。)
                else if (Model.CurrentHaveCodeSimulater != null)
                {
                    //Debug.Log("プラグを置く");
                    Model.PutCode();
                }

            }

            //コンセント生成/回収
            if (ActionMap.Player.Jump.WasPressedThisFrame()) 
            {
                if (Model.Socket==null)
                {
                    //Debug.Log("コンセント生成");
                    //コンセントを生成する。
                    Model.GenerateSocket(Presenter.GetSocketPrefab());
                }
                else if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 0.5f) != null )
                {
                    //Debug.Log("コンセント回収");
                    Model.DeleteSocket();
                }
            }
        }

        
        public void FixedUpdate()
        {
            Model.MovePlayer();
        }
    }   
}
