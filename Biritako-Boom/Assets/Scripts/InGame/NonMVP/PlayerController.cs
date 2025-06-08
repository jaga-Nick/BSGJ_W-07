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



        public void Update()
        {
            //移動処理
            Model.MoveInput(ActionMap);

            if (ActionMap.Player.Attack.WasPressedThisFrame())
            {
                Model.Explosion();
            }
            if (ActionMap.Player.Have.WasPressedThisFrame())
            {
                Debug.Log("プラグを持つ(家電からプラグを取得する）");
                if (checker.FindClosestEnemyOfTypeOne(Model.PlayerObject.transform.position, 5f)!=null) {
                    GameObject electro=checker.FindClosestEnemyOfTypeOneGameObject(Model.PlayerObject.transform.position, 5f);

                    //ジェネレート(プレイヤーキャラクターと家電）
                    var code = Model.generateCodeSystem.GenerateCode(Model.PlayerObject, electro);
                    Model.SetCurrentHaveCode(code);
                }
            }
            if (ActionMap.Player.Have.WasReleasedThisFrame())
            {
                if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 10f) == null && Model.Socket != null)
                {
                    Debug.Log("プラグを刺す");
                    Model.ConnectCode();

                }
                //範囲内にコードがない場合(それで保持している時。)
                else if (Model.CurrentHaveCode != null)
                {
                    Model.PutCode();
                }

            }
            if (ActionMap.Player.Jump.WasPressedThisFrame()) 
            {
                if (Model.Socket)
                {
                    //コンセントを生成する。
                    Model.GenerateSocket(Presenter.GetSocketPrefab());
                }
                //Socketが周囲10マスくらいに存在している場合。回収する。
                else if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 10f) == null && Model.Socket != null)
                {
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
