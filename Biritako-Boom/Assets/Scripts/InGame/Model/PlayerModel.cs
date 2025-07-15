using Common;
using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using InGame.Presenter;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
            this.presenter = playerPresenter;
        }
        
        private PlayerPresenter presenter;
        ComponentChecker checker = new ComponentChecker();

        //--データとして保持する為に。
        public GameObject PlayerObject { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        //-----------------------
        /// <summary>
        /// 生成地点
        /// </summary>
        private Vector3 InstancePosition = new Vector3(0,0,0);

        //---ステータス部分-----------
        public float Speed { get; private set; } = 10.0f;

        //コードゲージ
        private const float MaxCodeGauge = 30.0f;
        private float CurrentCodeGauge= 30.0f;

        private float RegenCodeGauge =0.003f;
        
        //探索範囲
        private float SearchScale =1f;


        private Vector3 MoveVector;

        //ソケット（コンセント）
        public GameObject Socket { get; private set;} = null;
        
        //コードのシミュレートを格納
        public GenerateCodeSystem generateCodeSystem { get; private set; }
        
        //現在持っているコード
        public CodeSimulater CurrentHaveCodeSimulator { get; private set; }
        //コードをシミュレートしている。
        public List<CodeSimulater> codeSimulators = new List<CodeSimulater>();
        //-------------------------------

        /// <summary>
        /// コードを生成する時、入れる。
        /// </summary>
        /// <param name="generater"></param>
        public void SetGenerateCodeSystem(GenerateCodeSystem _generateCodeSystem)
        {
            generateCodeSystem = _generateCodeSystem;
        }

        #region キャラクター生成
        /// <summary>
        /// キャラクター生成。
        /// </summary>
        public void GeneratePlayerCharacter()
        {
            if (PlayerObject == null)
            {
                PlayerObject = UnityEngine.Object.Instantiate( presenter.characterPrefab , InstancePosition, Quaternion.identity);
                Rb = PlayerObject?.GetComponent<Rigidbody2D>();

                HealGauge().Forget();
            }     
        }
        /// <summary>
        /// Addressable使用自機生成
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> AddressGeneratePlayerCharacter(string AddressChara, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(AddressChara);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                GameObject instance = UnityEngine.Object.Instantiate(prefab,InstancePosition,Quaternion.identity);
                return instance;
            }
        }
        #endregion

        #region ソケット関連（コンセント）
        /// <summary>
        /// ソケット通常設定
        /// </summary>
        /// <param name="Socket"></param>
        /// <returns></returns>
        public void GenerateSocket(GameObject SocketPrefab)
        {
            GameObject instance = null;
            //ソケットが生成されていない時。
            instance = UnityEngine.Object.Instantiate(SocketPrefab, PlayerObject.transform.position, Quaternion.identity);
            
            Socket = instance;

            AudioManager.Instance().LoadSoundEffect("SocketPutOn");
        }

        /// <summary>
        /// ソケットの情報入れてるわけじゃないのでエラーおきます。（まだ）
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public async UniTask AddressGenerateSocket(string Address)
        {
            AudioManager.Instance().LoadSoundEffect("SocketPutOn");
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(Address);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                UnityEngine.Object.Instantiate(prefab, InstancePosition, Quaternion.identity);
            }
        }

        /// <summary>
        /// ソケットを回収する
        /// </summary>
        public void DeleteSocket() {
            if (Socket!=null && codeSimulators.Count == 0)
            {
                AudioManager.Instance().LoadSoundEffect("PlayerableCharacterPickUpSocket");
                UnityEngine.Object.Destroy(Socket);
            }
         }
        #endregion

        //-----------------
        #region セット
        public void SetSpeed(float newSpeed)
        {
            Speed = newSpeed;
        }

        public void SetInstancePosition(Vector3 newPosition)
        {
            InstancePosition = newPosition;
        }

        public void SetCurrentHaveCode(CodeSimulater code)
        {
            CurrentHaveCodeSimulator = code;
        }

        #endregion
        //---------------------------
        #region 移動関数
        public void MoveInput(InputSystem_Actions Actions)
        {
            MoveVector = Actions.Player.Move.ReadValue<Vector2>() * Speed;
        }
        public void MovePlayer()
        {
            if (Rb != null)
            {
                Rb.linearVelocity = MoveVector;
            }
        }
        #endregion

        
        public void DecreaseCodeGauge(float Num)
        { CurrentCodeGauge -= Num;}
        public void IncrementCodeGauge(float Num)
        {
            CurrentCodeGauge += Num;
            CurrentCodeGauge = Mathf.Min(CurrentCodeGauge, MaxCodeGauge);
        }


        public async UniTask HealGauge()
        {
            try{
                CancellationToken token=PlayerObject.GetCancellationTokenOnDestroy();
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (CurrentHaveCodeSimulator == null)
                    {
                        IncrementCodeGauge(RegenCodeGauge);
                    }
                    //await UniTask.WaitForSeconds(1, cancellationToken :token);
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        //ゲージの割合
        public float GetCodeGaugePercent()
        {
            return CurrentCodeGauge / MaxCodeGauge;
        }


        /// <summary>
        /// コード接続(Socketに）
        /// </summary>
        public void ConnectSocketCode()
        {
            GameObject socket = checker.CharacterCheckGameObject<SocketPresenter>(PlayerObject.transform.position, SearchScale);

            //Null処理
            if (CurrentHaveCodeSimulator != null)
            {
                CurrentHaveCodeSimulator?.InjectionSocketCode(socket);
                //CodeSimulatorsに今持っているコードを入れてハブを無くす。
                CodeSimulaters.Add(CurrentHaveCodeSimulater);

                if (CodeSimulaters.Count <= 2)
                {
                    AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionSmall");
                }
                else if (CodeSimulaters.Count <= 4)
                {
                    AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionMiddle");
                }
                else if(CodeSimulaters.Count >= 5)
                {
                    AudioManager.Instance().LoadSoundEffect("PlugPluged_ExplosionLarge");
                }
            }
            CurrentHaveCodeSimulator = null;
        }

        //ここで特例的にコード『接続中』の処理を態と書いていく
        private CancellationTokenSource codeHaveCancellation;
        


        /// <summary>
        /// コードを持っている時。
        /// </summary>
        /// <returns></returns>
        public async UniTask HavingCode()
        {
            codeHaveCancellation?.Cancel();
            codeHaveCancellation?.Dispose();
            codeHaveCancellation= new CancellationTokenSource();

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(codeHaveCancellation.Token, PlayerObject.GetCancellationTokenOnDestroy());

            try
            {
                while (true) {
                    codeHaveCancellation.Token.ThrowIfCancellationRequested();

                    

                    //0になった時。自動的にプラグを落とす。
                    if(CurrentCodeGauge <= 0)
                    {
                        PutCode();
                        break;
                    }

                    if (CurrentHaveCodeSimulator)
                    {
                        DecreaseCodeGauge(CurrentHaveCodeSimulator.DecideCost());
                    }
                    //毎秒待機で軽くする。
                    await UniTask.Yield(PlayerLoopTiming.Update,codeHaveCancellation.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("コードを持つ処理キャンセル");
            }
            finally
            {
            }
        }

        /// <summary>
        /// 持つ処理の大枠。
        /// </summary>
        public void OnHave()
        {
            List<ComponentChecker.Contain<IEnemyModel>> Elects=checker.FindInterfaceContainList<IEnemyModel>(PlayerObject.transform.position, SearchScale);

            //検索結果
            ComponentChecker.Contain<IEnemyModel> MinDisElectro =null;
            float closestDist = Mathf.Infinity;

            if (Elects.Count > 0)
            {
                //最短距離検索
                foreach (var elect in Elects)
                {
                    //ソート
                    if (elect.Component.GetEnemyType() != 1) continue;

                    if (elect.Distance < closestDist)
                    {
                        MinDisElectro = elect;
                    }
                }
            }
            //放置しているプラグ（コード）の場所を検索
            ComponentChecker.Contain<CodeEndPointAttach> codeEndPoint= checker.FindCheckPackage<CodeEndPointAttach>(PlayerObject.transform.position, SearchScale);

            //どちらもNullだった時何もしない
            if(MinDisElectro == null && codeEndPoint ==null)
            {
                return;
            }
            //探索で家電しか検索できていない
            else if( MinDisElectro != null && codeEndPoint == null)
            {
                TakeGenerateCode();
                return;
            }
            //放置しているプラグ（コード）のみ
            else if (MinDisElectro == null && codeEndPoint != null)
            {
                TakeCode();
                return;
            }
            //条件式でどちらが近いか比較し実行
            else 
            {
                //家電がプラグ終点より近い時。
                if (MinDisElectro.Distance <= codeEndPoint.Distance)
                {
                    TakeGenerateCode();
                    return;
                }
                //
                else
                {
                    TakeCode();
                    return;
                }
            }
        }

        /// <summary>
        /// コードを生成する処理。
        /// </summary>
        public void TakeGenerateCode()
        {
            //家電が周囲に存在している場合。
            if (checker.FindClosestEnemyOfTypeOne(PlayerObject.transform.position, SearchScale) != null)
            {
                GameObject electro = checker.FindClosestEnemyOfTypeOneGameObject(PlayerObject.transform.position, SearchScale);

                //複数のコードを繋げないようにする
                var obje = codeSimulators.Where(code => code.StartObject == electro).FirstOrDefault();

                //近くに家電が存在し、家電に既にコードが繋がれていない場合。
                if (electro && obje == null)
                {
                    var code = generateCodeSystem.GenerateCode(electro, PlayerObject);
                    SetCurrentHaveCode(code);

                    //完了待機はしない（寧ろ待つとバグが発生する）
                    HavingCode().Forget();

                    AudioManager.Instance().LoadSoundEffect("PlayableCharacterPlugCatch");
                }

            }
        }

        /// <summary>
        /// コードを『拾う処理』
        /// </summary>
        public void TakeCode()
        {
            CodeEndPointAttach endpoint=checker.CharacterCheck<CodeEndPointAttach>(PlayerObject.transform.position, SearchScale);
            
            if (endpoint != null && CurrentHaveCodeSimulator == null)
            {
                AudioManager.Instance().LoadSoundEffect("PlayableCharacterPlugCatch");

                SetCurrentHaveCode(endpoint.CodeSimulater);
                endpoint.CodeSimulater.TakeCodeEvent(PlayerObject);
                HavingCode().Forget();
            }
        }
        
        //---------------------TakeCommand終了---------------------------------------------
        /// <summary>
        /// 一斉に爆破する
        /// </summary>
        public async void Explosion()
        {
            //コードが一つ以上生成されており、保持していない時。
            if (codeSimulators.Count > 0 && CurrentHaveCodeSimulator ==null)
            {
                AudioManager.Instance().LoadSoundEffect("CutInBomb");
                //カットイン挿入
                GameObject CutIn=GenerateExplosionManager.Instance().GenerateCutIn();
                //止める。
                TimeManager timeManager=TimeManager.Instance();
                timeManager.SetTimeScale(0);
                await CutIn.GetComponent<CutInAttach>().ActCutIn();
                //動かす
                timeManager.SetTimeScale(1);

                foreach (var i in codeSimulators)
                {
                    i.Explosion();
                }
                //リセット。
                codeSimulators = new List<CodeSimulater>();

                CurrentCodeGauge = MaxCodeGauge;
            }
        }


        //---------------動作関数--------------------

        /// <summary>
        /// コードを置く時。
        /// </summary>
        public void PutCode()
        {
            AudioManager.Instance().LoadSoundEffect("PlugUnpluged");

            //持っている処理をWhileを強制終了させる。
            codeHaveCancellation?.Cancel();
            codeHaveCancellation?.Dispose();
            codeHaveCancellation = null;

            presenter.animationView.SetHaveConcent(false);

            CurrentHaveCodeSimulator.PutCodeEvent(this);
            CurrentHaveCodeSimulator = null;
        }
    }
}