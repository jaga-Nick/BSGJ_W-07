using Common;
using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using InGame.Presenter;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using Setting;


namespace InGame.Model
{
    /// <summary>
    /// Playerのmodel管理
    /// </summary>
    [Serializable]
    public class PlayerModel
    {
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="playerPresenter"></param>
        public void Initialize(PlayerPresenter playerPresenter)
        {
            this._presenter = playerPresenter;
        }
        
        /// <summary>
        /// presenterとchecker
        /// </summary>
        private PlayerPresenter _presenter;
        private ComponentChecker _checker = new ComponentChecker();

        /// <summary>
        /// PlayerのGameObjectとRigidbody2D
        /// </summary>
        public GameObject PlayerObject { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        
        /// <summary>
        /// Playerの生成地点
        /// 今後ランダム配置でもいいかも
        /// </summary>
        private Vector3 _instancePosition = new Vector3(0,0,0);

        /// <summary>
        /// Playerのスピード
        /// </summary>
        public float Speed { get; private set; } = 10.0f;
        
        /// <summary>
        /// Playerの移動ベクトル
        /// </summary>
        private Vector3 _moveVector;

        /// <summary>
        /// コードゲージ
        /// </summary>
        private const float MaxCodeGauge = 30.0f;
        private float _currentCodeGauge　= 30.0f;
        private float _regionCodeGauge =　0.007f;
        
        /// <summary>
        /// 探索範囲
        /// </summary>
        private float _searchScale =1f;

        /// <summary>
        /// コンセント（Socketとして命名）
        /// </summary>
        public GameObject Socket { get; set;} = null;
        
        /// <summary>
        /// コードのシミュレーター
        /// </summary>
        public GenerateCodeSystem GenerateCodeSystem { get; private set; }
        
        /// <summary>
        /// 現在持っているコード
        /// </summary>
        public CodeSimulater CurrentHaveCodeSimulator { get; private set; }
        public List<CodeSimulater> codeSimulators = new List<CodeSimulater>();

        /// <summary>
        /// 爆発するか否かのブーリアン
        /// </summary>
        public bool doExplosion = false;
        
        /// <summary>
        /// Socketにコードが刺さっているかの否かの状態
        /// </summary>
        private CancellationTokenSource _codeHaveCancellation;
        
        #region セッター関数
        
        /// <summary>
        /// Playerのスピードをセット
        /// </summary>
        /// <param name="newSpeed"></param>
        public void SetSpeed(float newSpeed) { Speed = newSpeed; }

        /// <summary>
        /// Playerの生成座標をセット
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetInstancePosition(Vector3 newPosition) { _instancePosition = newPosition; }

        /// <summary>
        /// Playerのコードシミュレーターをセット
        /// </summary>
        /// <param name="code"></param>
        public void SetCurrentHaveCode(CodeSimulater code) { CurrentHaveCodeSimulator = code; }

        #endregion
        
        
        /// <summary>
        /// Playerの移動ベクトルをInputSystem_Actionsから取得し、Speedを掛ける。
        /// </summary>
        /// <param name="actions"></param>
        public void MoveInput(InputSystem_Actions actions)
        {
            _moveVector = actions.Player.Move.ReadValue<Vector2>() * Speed;
        }
        
        /// <summary>
        /// Playerの移動
        /// </summary>
        public void MovePlayer()
        {
            if (Rb != null)
            {
                Rb.linearVelocity = _moveVector;
            }
        }

        /// <summary>
        /// コードの生成時、generatorを入れる。
        /// </summary>
        /// <param name="generateCodeSystem"></param>
        public void SetGenerateCodeSystem(GenerateCodeSystem generateCodeSystem) { GenerateCodeSystem = generateCodeSystem; }
        
        /// <summary>
        /// キャラクター生成
        /// </summary>
        public void GeneratePlayerCharacter()
        {
            if (PlayerObject != null) return;
            PlayerObject = UnityEngine.Object.Instantiate( _presenter.characterPrefab , _instancePosition, Quaternion.identity);
            Rb = PlayerObject?.GetComponent<Rigidbody2D>();
            HealCodeGauge().Forget();
        }
        
        /// <summary>
        /// Addressable使用自機生成
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> AddressGeneratePlayerCharacter(string addressChara, CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(addressChara);

            using (new HandleDisposable<GameObject>(handle))
            {
                var prefab = await handle;
                var instance = UnityEngine.Object.Instantiate(prefab,_instancePosition,Quaternion.identity);
                return instance;
            }
        }

        #region ソケット関連（コンセント）

        /// <summary>
        /// Socketを生成する
        /// </summary>
        /// <param name="socketPrefab"></param>
        /// <returns></returns>
        public void GenerateSocket(GameObject socketPrefab)
        {
            // Socketオブジェクトの生成
            var instance = UnityEngine.Object.Instantiate(socketPrefab, PlayerObject.transform.position, Quaternion.identity);
            // modelのSocketに設定
            Socket = instance;
            // SocketのSEを再生
            AudioManager.Instance().LoadSoundEffect("SocketPutOn");
        }

        /// <summary>
        /// Socketを回収する
        /// </summary>
        public void RetrieveSocket()
        {
            // Socketがnullまたはコードシミュレーターが存在する場合は何もしない
            if (Socket == null || codeSimulators.Count != 0) return;
            // Socketの音を再生
            AudioManager.Instance().LoadSoundEffect("PlayerableCharacterPickUpSocket");
            // Socketを破棄
            UnityEngine.Object.Destroy(Socket);
        }
        
        #endregion

        
        /// <summary>
        /// Socketのコードゲージを減らす。
        /// </summary>
        /// <param name="num"></param>
        public async UniTask DecreaseCodeGauge(float num)
        { 
            _currentCodeGauge -= num;
            _currentCodeGauge = Mathf.Max (_currentCodeGauge, 0);
            // コードゲージが0になった時、強制的に爆発させる
            if (_currentCodeGauge == 0) { await ExplosionToSimultaneous(); }
        }
        
        /// <summary>
        /// Socketのコードゲージを増やす。
        /// </summary>
        /// <param name="num"></param>
        public void IncrementCodeGauge(float num)
        {
            _currentCodeGauge += num;
            _currentCodeGauge = Mathf.Min(_currentCodeGauge, MaxCodeGauge);
        }


        /// <summary>
        /// Socketのコードゲージを回復する。
        /// </summary>
        public async UniTask HealCodeGauge()
        {
            try{
                var token = PlayerObject.GetCancellationTokenOnDestroy();
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (codeSimulators.Count > 0)
                    {
                        Debug.Log(_regionCodeGauge * (1 + codeSimulators.Count-1));
                        DecreaseCodeGauge(_regionCodeGauge * (1.1f+codeSimulators.Count-1)).Forget();
                    }
                    else if (CurrentHaveCodeSimulator == null) {
                        IncrementCodeGauge(_regionCodeGauge);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセル処理
            }
        }

        /// <summary>
        /// Socketのコードゲージの割合を計算する
        /// </summary>
        /// <returns></returns>
        public float CalculatePercentOfCodeGaugePercent()
        {
            return _currentCodeGauge / MaxCodeGauge;
        }


        /// <summary>
        /// Socketにコードを接続する
        /// </summary>
        public void ConnectSocketToCode()
        {
            var socket = _checker.CharacterCheckGameObject<SocketPresenter>(PlayerObject.transform.position, _searchScale);

            // Socketがnullだった場合は何もしない
            if (CurrentHaveCodeSimulator != null)
            {
                CurrentHaveCodeSimulator?.InjectionSocketCode(socket);
                // CodeSimulatorsに今持っているコードを入れてハブを無くす
                codeSimulators.Add(CurrentHaveCodeSimulator);

                // Socketにコードがどれだけ刺さっているかで爆発力が変わる
                switch (codeSimulators.Count)
                {
                    case <= 2: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionSmall"); break;
                    case <= 4: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionMiddle"); break;
                    case >= 5: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionLarge"); break;
                }
            }
            // Socketのコードゲージをリセット
            CurrentHaveCodeSimulator = null;
        }


        /// <summary>
        /// Playerがコードを持っている時の処理
        /// </summary>
        /// <returns></returns>
        public async UniTask HavingCode()
        {
            // CancellationToken
            _codeHaveCancellation?.Cancel();
            _codeHaveCancellation?.Dispose();
            _codeHaveCancellation= new CancellationTokenSource();

            try
            {
                while (true) {
                    _codeHaveCancellation.Token.ThrowIfCancellationRequested();
                    // コードゲージが0になった時、自動的にプラグを落とす。
                    if(_currentCodeGauge <= 0) { PutOnCode(); break; }
                    // コードを持っている時、コードゲージを減らす。
                    if (CurrentHaveCodeSimulator) { await DecreaseCodeGauge(CurrentHaveCodeSimulator.DecideCost()); }
                    // 毎秒待機で軽くする。
                    await UniTask.Yield(PlayerLoopTiming.Update,_codeHaveCancellation.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("コードを持つ処理キャンセル");
            }
        }

        /// <summary>
        /// Playerがコードを持っている時の処理
        /// メソッド名を変えたほうがいいかも
        /// </summary>
        public void OnHave()
        {
            // 家電が周囲に存在しているか、放置しているプラグ（コード）の場所を検索する。
            var electronics = _checker.FindInterfaceContainList<IEnemyModel>(PlayerObject.transform.position, _searchScale);
            // 検索結果
            ComponentChecker.Contain<IEnemyModel> minDisElectro =null;
            
            if (electronics.Count > 0)
            {
                // 最短距離検索
                foreach (var elect in electronics)
                {
                    // ソート
                    if (elect.Component.GetEnemyType() != 1) continue;
                    // 最短の家電の距離に更新する
                    if (elect.Distance < Mathf.Infinity) { minDisElectro = elect; }
                }
            }
            
            // 放置しているプラグ（コード）の場所を検索
            var codeEndPoint= _checker.FindCheckPackage<CodeEndPointAttach>(PlayerObject.transform.position, _searchScale);
            // どちらもNullだった時何もしない
            if ( minDisElectro == null && codeEndPoint ==null ) { return; }
            // 探索で家電しか検索できていない
            if ( minDisElectro != null && codeEndPoint == null ) { GenerateCode(); }
            // 放置しているプラグ（コード）のみ
            else if ( minDisElectro == null ) { PickUpCode(); }
            
            // 条件式でどちらが近いか比較し実行
            else
            {
                // 家電がプラグ終点より近い時
                if (minDisElectro.Distance <= codeEndPoint.Distance) { GenerateCode(); return; }
                // そうでないとき
                PickUpCode();
            }
        }

        /// <summary>
        /// コードを生成する
        /// </summary>
        public void GenerateCode()
        {
            // 家電が周囲に存在している場合
            if (_checker.FindClosestEnemyOfTypeOne(PlayerObject.transform.position, _searchScale) == null) return;
            var electronics = _checker.FindClosestEnemyOfTypeOneGameObject(PlayerObject.transform.position, _searchScale);

            // 複数のコードを繋げないようにする
            var codeObject = codeSimulators.FirstOrDefault(code => code.StartObject == electronics);

            // 近くに家電が存在し、家電に既にコードが繋がれていない場合
            if (!electronics || codeObject != null) return;
            {
                var code = GenerateCodeSystem.GenerateCode(electronics, PlayerObject);
                // 生成したコードをセットする
                SetCurrentHaveCode(code);
                // 完了待機はしない（寧ろ待つとバグが発生する）
                HavingCode().Forget();
                // コードを生成した時のSEを再生
                AudioManager.Instance().LoadSoundEffect("PlayableCharacterPlugCatch");
            }
        }
        
        /// <summary>
        /// すでにコードがコンセントに接続されているかどうか判定
        /// </summary>
        public bool IsConnectedToSocket()
        {
            // ソケットが存在しており、接続されたコードがある（＝接続されている）
            return Socket != null && codeSimulators.Count > 0;
        }

        /// <summary>
        /// 落ちたコードを拾う処理
        /// </summary>
        public void PickUpCode()
        {
            // プラグの先端を検索
            var endpoint = _checker.CharacterCheck<CodeEndPointAttach>(PlayerObject.transform.position, _searchScale);
            // 何も見つからなかったら何もしない
            if (endpoint == null || CurrentHaveCodeSimulator != null) return;
            // プラグを拾った時のSEを再生
            AudioManager.Instance().LoadSoundEffect("PlayableCharacterPlugCatch");
            // 生成したコードをセットする
            SetCurrentHaveCode(endpoint.CodeSimulater);
            // コードを拾った時の処理を実行
            endpoint.CodeSimulater.TakeCodeEvent(PlayerObject);
            HavingCode().Forget();
        }
        
        /// <summary>
        /// 持ってるコードを置く時
        /// </summary>
        public void PutOnCode()
        {
            // コードを置くときのSEを再生
            AudioManager.Instance().LoadSoundEffect("PlugUnpluged");

            //持っている処理をWhileを強制終了させる。
            _codeHaveCancellation?.Cancel();
            _codeHaveCancellation?.Dispose();
            _codeHaveCancellation = null;
            
            // コードを持ってない時のアニメーション
            _presenter.animationView.SetHaveConcent(false);

            CurrentHaveCodeSimulator?.PutCodeEvent(this);
            CurrentHaveCodeSimulator = null;
        }
        
        /// <summary>
        /// 一斉に爆破する
        /// </summary>
        public async UniTask ExplosionToSimultaneous()
        {
            // コードが1つ以上生成されており、保持していない時
            if (codeSimulators.Count > 0 && CurrentHaveCodeSimulator ==null && doExplosion ==false)
            {
                // 爆発するか否かのブーリアンをtrueにする
                doExplosion = true;
                // カットインのSEを再生
                AudioManager.Instance().LoadSoundEffect("CutInBomb");
                // カットイン挿入
                var cutIn= GenerateExplosionManager.Instance().GenerateCutIn();
                // 一旦時を止める
                var timeManager = TimeManager.Instance();
                timeManager.SetTimeScale(0);
                await cutIn.GetComponent<CutInAttach>().ActCutIn();
                // 再度時を動かす
                timeManager.SetTimeScale(1);
                
                // 爆発のサイズを決定する
                var explosionSize = codeSimulators.Count switch
                {
                    1 => 0,
                    <= 4 => 1,
                    >= 5 => 2
                };

                // 爆発サイズをもとに爆発させる
                foreach (var i in codeSimulators)
                {
                    i.Explosion(explosionSize).Forget();
                }
                
                //リセット
                codeSimulators = new List<CodeSimulater>();
                _currentCodeGauge = MaxCodeGauge;
                doExplosion = false;
            }
        }
    }
}