using Common;
using Cysharp.Threading.Tasks;
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
    public class PlayerModel
    {
        //--データとして保持する為に。
        private GameObject PlayerObject;
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

        //オブジェクトをスタック
        private LinkedList<GameObject> Objects = new LinkedList<GameObject>();


        public GameObject GeneratePlayerCharacter(Vector3 generatePosition)
        {
            GameObject player = null;
            if (PlayerObject)
            {
                player = Object.Instantiate(PlayerObject, generatePosition, Quaternion.identity);
            }
            Rb = player?.GetComponent<Rigidbody2D>();
            return player;
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
                GameObject instance = Object.Instantiate(prefab,InstancePosition,Quaternion.identity);
                Rb = instance.GetComponent<Rigidbody2D>();
                return instance;
            }
        }
        
        public void SetSpeed(float newSpeed)
        {
            Speed = newSpeed;
        }

        public void SetInstancePosition(Vector3 newPosition)
        {
            InstancePosition = newPosition;
        }

        public void MoveInput(InputSystem_Actions Actions)
        {
            MoveVector = Actions.Player.Move.ReadValue<Vector3>() * Speed;
        }
        public void MovePlayer()
        {
            Rb.linearVelocity = MoveVector;
        }

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
            
        }
    }
}