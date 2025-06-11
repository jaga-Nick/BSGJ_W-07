using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using InGame.Model;
using InGame.Presenter;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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

            if (ActionMap.Player.Attack.WasPressedThisFrame())
            {
                Model.Explosion();
            }
            if (ActionMap.Player.Have.WasPressedThisFrame())
            {
                if (checker.FindClosestEnemyOfTypeOne(Model.PlayerObject.transform.position, 1f)!=null) 
                {
                    GameObject electro=checker.FindClosestEnemyOfTypeOneGameObject(Model.PlayerObject.transform.position, 10f);

                    //複数のコードを繋げないようにする
                    var obje=Model.CodeSimulaters.Where(code => code.StartObject==electro).FirstOrDefault();
                    Debug.Log(obje?.name);
                    if (electro && obje == null) {
                        //ジェネレート(始点:家電と終点:プレイヤーキャラクター）
                        var code = Model.generateCodeSystem.GenerateCode(electro, Model.PlayerObject);
                        Model.SetCurrentHaveCode(code);
                    }
                }
            }
            if (ActionMap.Player.Have.WasReleasedThisFrame())
            {
                if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 1f) != null)
                {
                    Debug.Log("プラグを刺す");
                    Model.ConnectSocketCode();

                }
                //範囲内にコードがない場合(それで保持している時。)
                else if (Model.CurrentHaveCode != null)
                {
                    Model.PutCode();
                }

            }
            if (ActionMap.Player.Jump.WasPressedThisFrame()) 
            {
                if (Model.Socket==null)
                {
                    Debug.Log("コンセント");
                    //コンセントを生成する。
                    Model.GenerateSocket(Presenter.GetSocketPrefab());
                    Debug.Log(Model.Socket.name);
                }
                else if (checker.CharacterCheck<SocketPresenter>(Model.PlayerObject.transform.position, 0.5f) != null )
                {
                    Debug.Log("ソケットを回収するキー");
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
