using Common;
using Cysharp.Threading.Tasks;
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
        private GameObject _playerObject;
        private Rigidbody2D _rb;
        //-----------------------
        /// <summary>
        /// 生成地点
        /// </summary>
        private Vector3 _instancePosition = new Vector3(0,0,0);

        //---ステータス部分-----------
        public float Speed { get; private set; } = 3.0f;

        /// <summary>
        /// 自機生成
        /// </summary>
        public GameObject PlayerCharacterGenerate(Vector3 generatePosition)
        {
            GameObject player = null;
            if (_playerObject)
            {
                player = Object.Instantiate(_playerObject, generatePosition, Quaternion.identity);
            }
            _rb = player?.GetComponent<Rigidbody2D>();
            return player;
        }

        /// <summary>
        /// Addressable使用自機生成
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> PlayerCharacterAddressGenerate(string charaAddress, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(charaAddress);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                GameObject instance = Object.Instantiate(prefab,_instancePosition,Quaternion.identity);
                _rb = instance.GetComponent<Rigidbody2D>();
                return instance;
            }
        }
        
        /// <summary>
        /// 速度変化
        /// </summary>
        /// <param name="newSpeed"></param>
        public void ChangeSpeed(float newSpeed)
        {
            Speed = newSpeed;
        }

        /// <summary>
        /// 生成地点変更
        /// </summary>
        /// <param name="newPosition"></param>
        public void InstancePositionChange(Vector3 newPosition)
        {
            _instancePosition = newPosition;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="actions"></param>
        public void PlayerMove(InputSystem_Actions actions)
        {
            _rb.linearVelocity = actions.Player.Move.ReadValue<Vector3>() * Speed;
        }
    }
}