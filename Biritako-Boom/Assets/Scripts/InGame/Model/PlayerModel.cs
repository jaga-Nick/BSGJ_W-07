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

namespace InGame.Model
{
    /// <summary>
    /// Playerのmodel管理
    /// </summary>
    [Serializable]
    public class PlayerModel
    {
        public void Initialize(PlayerPresenter playerPresenter)
        {
            this.presenter = playerPresenter;
        }
        
        private PlayerPresenter presenter; 

        //--データとして保持する為に。
        public GameObject PlayerObject { get; private set; }
        private Rigidbody2D Rb;
        //-----------------------
        /// <summary>
        /// 生成地点
        /// </summary>
        private Vector3 InstancePosition = new Vector3(0,0,0);

        //---ステータス部分-----------
        public float Speed { get; private set; } = 3.0f;

        private float MaxCodeGauge = 10.0f;
        public float CodeGauge { get; private set; } = 10.0f;

        private Vector3 MoveVector;

        //ソケット（コンセント）
        public GameObject Socket { get; private set;} = null;
        
        //格納。
        public List<GenerateCodeSystem> code = new List<GenerateCodeSystem>();
        //コードのシミュレートを格納
        public GenerateCodeSystem generateCodeSystem { get; private set; }
        
        public CodeSimulater CurrentHaveCode { get; private set; }
        //コードをシミュレートしている。
        public List<CodeSimulater> CodeSimulaters = new List<CodeSimulater>();
        //-------------------------------

        //オブジェクトをスタック
        private LinkedList<GameObject> Objects = new LinkedList<GameObject>();

        private GameObject SocketObject = null;


        /// <summary>
        /// コードを生成する時、入れる。
        /// </summary>
        /// <param name="generater"></param>
        public void GetGenerateCodeSystem(GenerateCodeSystem _generateCodeSystem)
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


        /// <summary>
        /// ソケット通常設定
        /// </summary>
        /// <param name="Socket"></param>
        /// <returns></returns>
        public void GenerateSocket(GameObject Socket)
        {
            GameObject instance = null;
            //ソケットが生成されていない時。
            instance = UnityEngine.Object.Instantiate(Socket, PlayerObject.transform.position, Quaternion.identity);
            
        }

        /// <summary>
        /// ソケットの情報入れてるわけじゃないのでエラーおきます。（まだ）
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public async UniTask AddressGenerateSocket(string Address)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(Address);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                UnityEngine.Object.Instantiate(prefab, InstancePosition, Quaternion.identity);
            }
        }

        /// <summary>
        /// ソケットをデリート
        /// </summary>
        public void DeleteSocket() {
            if (Socket!=null)
            {
                UnityEngine.Object.Destroy(SocketObject);
            }
         }


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
            CurrentHaveCode = code;
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

        
        /// <summary>
        /// ゲージ現象
        /// </summary>
        /// <param name="Num"></param>
        public void DecreaseCodeGauge(float Num)
        {
            CodeGauge -= Num;
        }

        //
        public float GetCodeGaugePercent()
        {
            return CodeGauge / MaxCodeGauge;
        }

        /// <summary>
        /// コード接続
        /// </summary>
        public void ConnectCode()
        {
            ComponentChecker checker = new ComponentChecker();
            GameObject socket = checker.CharacterCheckGameObject<SocketPresenter>(PlayerObject.transform.position, 10f);
            CurrentHaveCode.InjectionSocketCode(socket);

            //CodeSimulatorsに今持っているコードを入れてハブを無くす。
            CodeSimulaters.Add(CurrentHaveCode);
            CurrentHaveCode = null;
        }
        
        /// <summary>
        /// コードを置く時。
        /// </summary>
        public void PutCode()
        {
            CurrentHaveCode.PutCodeEvent();
            //CodeSimulaters.Add(CurrentHaveCode);
            CurrentHaveCode = null;
        }

        

        /// <summary>
        /// 一斉に爆破する
        /// </summary>
        public void Explosion()
        {
            //コードが一つ以上生成されており、保持していない時。
            if (CodeSimulaters.Count > 0 && CurrentHaveCode==null)
            {
                foreach (var i in CodeSimulaters)
                {
                    i.Explosion();
                }
                //リセット。
                CodeSimulaters = new List<CodeSimulater>();
            }
        }
        
    }
}