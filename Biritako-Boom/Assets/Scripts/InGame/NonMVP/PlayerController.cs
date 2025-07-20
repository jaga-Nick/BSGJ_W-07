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
        public PlayerController(PlayerModel playerModel,PlayerPresenter playerPresenter)
        {
            _model = playerModel;
            _presenter = playerPresenter;
            _enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
            var manage = InputSystemActionsManager.Instance();
            _actionMap = manage.GetInputSystem_Actions();
        }
        
        /// <summary>
        /// presenterとmodelとInputSystem_Actionsとcheckerを保持する。
        /// </summary>
        private readonly PlayerPresenter _presenter;
        private readonly PlayerModel _model;
        private InputSystem_Actions _actionMap;
        private readonly ComponentChecker _checker = new ComponentChecker();
        private readonly EnemySpawner _enemySpawner;

        /// <summary>
        /// Playerがコードを保持しているかどうか
        /// </summary>
        private bool _isHaveCode = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            var manager = InputSystemActionsManager.Instance();
            _actionMap = manager.GetInputSystem_Actions();
            manager.PlayerEnable();
        }

        public void Update()
        {
            // 移動処理
            _model.MoveInput(_actionMap);

            // ジャンプ処理（Enterキー）
            if (_model.codeSimulators.Count > 0 && _actionMap.Player.Attack.WasPressedThisFrame())
            {
                // 爆発動作の開始
                _model.ExplosionToSimultaneous();
                // プラグの先を消す
                _presenter.DestroySocketTip();
                // 爆発させた分だけ家電の数を減らす
                _enemySpawner.CurrentElectronics -= _model.codeSimulators.Count;
            }

            // コードを保持する
            if (_actionMap.Player.Have.WasPressedThisFrame())
            {
                _presenter.AnimationView.SetHaveConcent(true);
                // コードを生成-保持する為の処理
                _model.OnHave();
                // コードを保持しているかどうか
                if (_model.CurrentHaveCodeSimulator != null) { _isHaveCode = true; }
            }
            
            // コードを保持している時に離した場合
            if (_actionMap.Player.Have.WasReleasedThisFrame())
            {
                // 範囲内にコンセントがある場合
                _presenter.AnimationView.SetHaveConcent(false);
                
                // 保持しているときかつ範囲内にコンセントがある場合
                var socketPresenter = _checker.CharacterCheck<SocketPresenter>(_model.PlayerObject.transform.position, 1f);
                if (socketPresenter != null)
                {
                    _presenter.AnimationView.SetHaveConcent(true);
                    
                    // プラグの先のPrefabをコンセントの先に表示
                    if (_isHaveCode)
                    {
                        var socketTip = _presenter.GetSocketTipPrefab();
                        var socketTipTransform = socketPresenter.socketTipTransform;
                        
                        if (socketTip != null && socketTipTransform != null)
                        {
                            var socketTipInstance = GameObject.Instantiate(
                                socketTip,
                                socketTipTransform.position,
                                socketTipTransform.rotation,
                                socketPresenter.transform
                            );
                            _model.SocketTips.Add(socketTipInstance);
                        }
                        
                        Debug.Log(_model.SocketTips);
                        _isHaveCode = false;
                    }
                    // プラグをコンセントにさす
                    _model.ConnectSocketToCode();
                }
                
                // 保持しているかつ範囲内にコードがない場合
                else if (_model.CurrentHaveCodeSimulator != null)
                {
                    _presenter.AnimationView.SetHaveConcent(false);
                    // コードを地面に置く
                    _model.PutOnCode();
                }
            }

            // コンセント生成および回収
            if (_actionMap.Player.Jump.WasPressedThisFrame()) 
            {
                // すでに接続されているなら何もしない
                if (_model.IsConnectedToSocket()) { return; }
                
                if (_model.Socket ==null)
                {
                    // コンセントがない場合は生成
                    _model.GenerateSocket(_presenter.GetSocketPrefab());
                }
                else 
                {
                    // すでにコンセントがある場合は回収して生成
                    _model.RetrieveSocket();
                    _model?.GenerateSocket(_presenter.GetSocketPrefab());
                }
            }

            // ポーズ画面の表示
            if (!_actionMap.Player.Pose.WasPressedThisFrame()) return;
            TimeManager.Instance().SetTimeScale(0);
            SceneManager.Instance().LoadSubScene(new PauseSceneLoader()).Forget();
        }

        
        public void FixedUpdate()
        {
            _model.MovePlayer();
        }
    }   
}
