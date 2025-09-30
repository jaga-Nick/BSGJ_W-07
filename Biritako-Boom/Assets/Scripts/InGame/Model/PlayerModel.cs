using Common;
using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using InGame.Presenter;
using Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace InGame.Model
{
    /// <summary>
    /// Playerのmodel管理
    /// </summary>
    [Serializable]
    public class PlayerModel
    {
        public static event Action onPlayerSpawned;
        
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="playerPresenter"></param>
        public void Initialize(PlayerPresenter playerPresenter)
        {
            this.presenter = playerPresenter;
        }
        
        /// <summary>
        /// presenterとchecker
        /// </summary>
        private PlayerPresenter presenter;
        private ComponentChecker checker = new ComponentChecker();

        /// <summary>
        /// PlayerのGameObjectとRigidbody2D
        /// </summary>
        public GameObject playerObject { get; private set; }
        public Rigidbody2D rb { get; private set; }
        
        /// <summary>
        /// Playerの生成地点
        /// 今後ランダム配置でもいいかも
        /// </summary>
        private Vector3 instancePosition = new Vector3(0,0,0);

        /// <summary>
        /// Playerのスピード
        /// </summary>
        public float speed { get; private set; } = 10.0f;
        
        /// <summary>
        /// Playerの移動ベクトル
        /// </summary>
        private Vector3 moveVector;

        /// <summary>
        /// コードゲージ
        /// </summary>
        private const float maxCodeGauge = 30.0f;
        private float currentCodeGauge　= 30.0f;
        private float regionCodeGauge =　0.007f;
        
        /// <summary>
        /// 探索範囲
        /// </summary>
        private float searchScale =1f;

        /// <summary>
        /// コンセント（Socketとして命名）
        /// </summary>
        public GameObject socket { get; set;} = null;
        
        /// <summary>
        /// コンセントに刺さったプラグの先端
        /// </summary>
        public List<GameObject> socketTips = new List<GameObject>();
        
        /// <summary>
        /// コードのシミュレーター
        /// </summary>
        public GenerateCodeSystem generateCodeSystem { get; private set; }
        
        /// <summary>
        /// 現在持っているコード
        /// </summary>
        public CodeSimulater currentHaveCodeSimulator { get; private set; }
        public List<CodeSimulater> codeSimulators = new List<CodeSimulater>();

        /// <summary>
        /// 爆発するか否かのブーリアン
        /// </summary>
        public bool doExplosion = false;
        
        /// <summary>
        /// Socketにコードが刺さっているかの否かの状態
        /// </summary>
        private CancellationTokenSource codeHaveCancellation;
        
        #region セッター関数
        
        /// <summary>
        /// Playerのスピードをセット
        /// </summary>
        /// <param name="newSpeed"></param>
        public void SetSpeed(float newSpeed) { speed = newSpeed; }

        /// <summary>
        /// Playerの生成座標をセット
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetInstancePosition(Vector3 newPosition) { instancePosition = newPosition; }

        /// <summary>
        /// Playerのコードシミュレーターをセット
        /// </summary>
        /// <param name="code"></param>
        public void SetCurrentHaveCode(CodeSimulater code) { currentHaveCodeSimulator = code; }

        #endregion
        
        
        /// <summary>
        /// Playerの移動ベクトルをInputSystem_Actionsから取得し、Speedを掛ける。
        /// </summary>
        /// <param name="actions"></param>
        public void MoveInput(InputSystem_Actions actions)
        {
            moveVector = actions.Player.Move.ReadValue<Vector2>() * speed;
        }
        
        /// <summary>
        /// Playerの移動
        /// </summary>
        public void MovePlayer()
        {
            if (rb != null)
            {
                rb.linearVelocity = moveVector;
            }
        }

        /// <summary>
        /// コードの生成時、generatorを入れる。
        /// </summary>
        /// <param name="generateCodeSystem"></param>
        public void SetGenerateCodeSystem(GenerateCodeSystem generateCodeSystem) { this.generateCodeSystem = generateCodeSystem; }
        
        /// <summary>
        /// キャラクター生成
        /// </summary>
        public void GeneratePlayerCharacter()
        {
            if (playerObject != null) return;
            playerObject = UnityEngine.Object.Instantiate( presenter.characterPrefab , instancePosition, Quaternion.identity);
            onPlayerSpawned?.Invoke();
            rb = playerObject?.GetComponent<Rigidbody2D>();
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
                var instance = UnityEngine.Object.Instantiate(prefab,instancePosition,Quaternion.identity);
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
            var instance = UnityEngine.Object.Instantiate(socketPrefab, playerObject.transform.position, Quaternion.identity);
            // modelのSocketに設定
            socket = instance;
            // SocketのSEを再生
            AudioManager.Instance().LoadSoundEffect("SocketPutOn");
        }

        /// <summary>
        /// Socketを回収する
        /// </summary>
        public void RetrieveSocket()
        {
            // Socketがnullまたはコードシミュレーターが存在する場合は何もしない
            if (socket == null || codeSimulators.Count != 0) return;
            // Socketの音を再生
            AudioManager.Instance().LoadSoundEffect("PlayerableCharacterPickUpSocket");
            // Socketを破棄
            UnityEngine.Object.Destroy(socket);
        }
        
        #endregion

        
        /// <summary>
        /// Socketのコードゲージを減らす。
        /// </summary>
        /// <param name="num"></param>
        public async UniTask DecreaseCodeGauge(float num)
        { 
            currentCodeGauge -= num;
            currentCodeGauge = Mathf.Max (currentCodeGauge, 0);
            // コードゲージが0になった時、強制的に爆発させる
            if (currentCodeGauge == 0) { await ExplosionToSimultaneous(); }
        }
        
        /// <summary>
        /// Socketのコードゲージを増やす。
        /// </summary>
        /// <param name="num"></param>
        public void IncrementCodeGauge(float num)
        {
            currentCodeGauge += num;
            currentCodeGauge = Mathf.Min(currentCodeGauge, maxCodeGauge);
        }


        /// <summary>
        /// Socketのコードゲージを回復する。
        /// </summary>
        public async UniTask HealCodeGauge()
        {
            try{
                var token = playerObject.GetCancellationTokenOnDestroy();
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (codeSimulators.Count > 0)
                    {
                        Debug.Log(regionCodeGauge * (1 + codeSimulators.Count-1));
                        DecreaseCodeGauge(regionCodeGauge * (1.1f+codeSimulators.Count-1)).Forget();
                    }
                    else if (currentHaveCodeSimulator == null) {
                        IncrementCodeGauge(regionCodeGauge);
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
            return currentCodeGauge / maxCodeGauge;
        }


        /// <summary>
        /// Socketにコードを接続する
        /// </summary>
        public void ConnectSocketToCode()
        {
            var socket = checker.CharacterCheckGameObject<SocketPresenter>(playerObject.transform.position, searchScale);

            // Socketがnullだった場合は何もしない
            if (currentHaveCodeSimulator != null)
            {
                currentHaveCodeSimulator?.InjectionSocketCode(socket);
                // CodeSimulatorsに今持っているコードを入れてハブを無くす
                codeSimulators.Add(currentHaveCodeSimulator);

                // Socketにコードがどれだけ刺さっているかで爆発力が変わる
                switch (codeSimulators.Count)
                {
                    case <= 2: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionSmall"); break;
                    case <= 4: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionMiddle"); break;
                    case >= 5: AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionLarge"); break;
                }
            }
            // Socketのコードゲージをリセット
            currentHaveCodeSimulator = null;
        }


        /// <summary>
        /// Playerがコードを持っている時の処理
        /// </summary>
        /// <returns></returns>
        public async UniTask HavingCode()
        {
            // CancellationToken
            codeHaveCancellation?.Cancel();
            codeHaveCancellation?.Dispose();
            codeHaveCancellation= new CancellationTokenSource();

            try
            {
                while (true) {
                    codeHaveCancellation.Token.ThrowIfCancellationRequested();
                    // コードゲージが0になった時、自動的にプラグを落とす。
                    if(currentCodeGauge <= 0) { PutOnCode(); break; }
                    // コードを持っている時、コードゲージを減らす。
                    if (currentHaveCodeSimulator) { await DecreaseCodeGauge(currentHaveCodeSimulator.DecideCost()); }
                    // 毎秒待機で軽くする。
                    await UniTask.Yield(PlayerLoopTiming.Update,codeHaveCancellation.Token);
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
            var electronics = checker.FindInterfaceContainList<IEnemyModel>(playerObject.transform.position, searchScale);
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
            var codeEndPoint= checker.FindCheckPackage<CodeEndPointAttach>(playerObject.transform.position, searchScale);
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
            if (checker.FindClosestEnemyOfTypeOne(playerObject.transform.position, searchScale) == null) return;
            var electronics = checker.FindClosestEnemyOfTypeOneGameObject(playerObject.transform.position, searchScale);

            // 複数のコードを繋げないようにする
            var codeObject = codeSimulators.FirstOrDefault(code => code.startObject == electronics);

            // 近くに家電が存在し、家電に既にコードが繋がれていない場合
            if (!electronics || codeObject != null) return;
            {
                var code = generateCodeSystem.GenerateCode(electronics, playerObject);
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
            return socket != null && codeSimulators.Count > 0;
        }

        /// <summary>
        /// 落ちたコードを拾う処理
        /// </summary>
        public void PickUpCode()
        {
            // プラグの先端を検索
            var endpoint = checker.CharacterCheck<CodeEndPointAttach>(playerObject.transform.position, searchScale);
            // 何も見つからなかったら何もしない
            if (endpoint == null || currentHaveCodeSimulator != null) return;
            // プラグを拾った時のSEを再生
            AudioManager.Instance().LoadSoundEffect("PlayableCharacterPlugCatch");
            // 生成したコードをセットする
            SetCurrentHaveCode(endpoint.CodeSimulater);
            // コードを拾った時の処理を実行
            endpoint.CodeSimulater.TakeCodeEvent(playerObject);
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
            codeHaveCancellation?.Cancel();
            codeHaveCancellation?.Dispose();
            codeHaveCancellation = null;
            
            // コードを持ってない時のアニメーション
            presenter.animationView.SetHaveConcent(false);

            currentHaveCodeSimulator?.PutCodeEvent(this);
            currentHaveCodeSimulator = null;
        }
        
        /// <summary>
        /// 一斉に爆破する
        /// </summary>
        public async UniTask ExplosionToSimultaneous()
        {
            // コードが1つ以上生成されており、保持していない時
            if (codeSimulators.Count > 0 && currentHaveCodeSimulator ==null && doExplosion ==false)
            {
                // 爆発するか否かのブーリアンをtrueにする
                doExplosion = true;
                // カットインのSEを再生
                AudioManager.Instance().LoadSoundEffect("CutInBomb");
                // カットイン挿入
                var cutIn= GenerateExplosionManager.Instance(true).GenerateCutIn();
                // 一旦時を止める
                var timeManager = TimeManager.Instance(true);
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
                currentCodeGauge = maxCodeGauge;
                doExplosion = false;
            }
        }
    }
}