using Common;
using Cysharp.Threading.Tasks;
using InGame.Model;
using InGame.Presenter;
using Pose;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Input
    /// </summary>
    public class PlayerController
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerModel"></param>
        /// <param name="playerPresenter"></param>
        public PlayerController(PlayerModel _playerModel,PlayerPresenter _playerPresenter)
        {
            model = _playerModel;
            presenter = _playerPresenter;
            enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
            var manage = InputSystemActionsManager.Instance();
            actionMap = manage.GetInputSystem_Actions();
        }
        
        /// <summary>
        /// presenterとmodelとInputSystem_Actionsとcheckerを保持する。
        /// </summary>
        private readonly PlayerPresenter presenter;
        private readonly PlayerModel model;
        private InputSystem_Actions actionMap;
        private readonly ComponentChecker checker = new ComponentChecker();
        private readonly EnemySpawner enemySpawner;

        /// <summary>
        /// Playerがコードを保持しているかどうか
        /// </summary>
        private bool isHaveCode = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            var manager = InputSystemActionsManager.Instance();
            actionMap = manager.GetInputSystem_Actions();
            manager.PlayerEnable();
        }

        public void Update()
        {
            // 移動処理
            model.MoveInput(actionMap);

            // ジャンプ処理（Enterキー）
            if (model.codeSimulators.Count > 0 && actionMap.Player.Attack.WasPressedThisFrame())
            {
                // 爆発動作の開始
                model.ExplosionToSimultaneous();
                // プラグの先を消す
                presenter.DestroySocketTip();
                // 爆発させた分だけ家電の数を減らす
                enemySpawner.CurrentElectronics -= model.codeSimulators.Count;
            }

            // コードを保持する
            if (actionMap.Player.Have.WasPressedThisFrame())
            {
                presenter.animationView.SetHaveConcent(true);
                // コードを生成-保持する為の処理
                model.OnHave();
                // コードを保持しているかどうか
                if (model.currentHaveCodeSimulator != null) { isHaveCode = true; }
            }
            
            // コードを保持している時に離した場合
            if (actionMap.Player.Have.WasReleasedThisFrame())
            {
                // 範囲内にコンセントがある場合
                presenter.animationView.SetHaveConcent(false);
                
                // 保持しているときかつ範囲内にコンセントがある場合
                var socketPresenter = checker.CharacterCheck<SocketPresenter>(model.playerObject.transform.position, 1f);
                if (socketPresenter != null)
                {
                    presenter.animationView.SetHaveConcent(true);
                    
                    // プラグの先のPrefabをコンセントの先に表示
                    if (isHaveCode)
                    {
                        var socketTip = presenter.GetSocketTipPrefab();
                        var socketTipTransform = socketPresenter.socketTipTransform;
                        
                        if (socketTip != null && socketTipTransform != null)
                        {
                            var socketTipInstance = GameObject.Instantiate(
                                socketTip,
                                socketTipTransform.position,
                                socketTipTransform.rotation,
                                socketPresenter.transform
                            );
                            model.socketTips.Add(socketTipInstance);
                        }
                        
                        Debug.Log(model.socketTips);
                        isHaveCode = false;
                    }
                    // プラグをコンセントにさす
                    model.ConnectSocketToCode();
                }
                
                // 保持しているかつ範囲内にコードがない場合
                else if (model.currentHaveCodeSimulator != null)
                {
                    presenter.animationView.SetHaveConcent(false);
                    // コードを地面に置く
                    model.PutOnCode();
                }
            }

            // コンセント生成および回収
            if (actionMap.Player.Jump.WasPressedThisFrame()) 
            {
                // すでに接続されているなら何もしない
                if (model.IsConnectedToSocket()) { return; }
                
                if (model.socket ==null)
                {
                    // コンセントがない場合は生成
                    model.GenerateSocket(presenter.GetSocketPrefab());
                }
                else 
                {
                    // すでにコンセントがある場合は回収して生成
                    model.RetrieveSocket();
                    model?.GenerateSocket(presenter.GetSocketPrefab());
                }
            }

            // ポーズ画面の表示
            if (!actionMap.Player.Pose.WasPressedThisFrame()) return;
            TimeManager.Instance(true).SetTimeScale(0);
            SceneManager.Instance().LoadSubScene(new PauseSceneLoader()).Forget();
        }

        
        public void FixedUpdate()
        {
            model.MovePlayer();
        }
    }   
}
