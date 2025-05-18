using Common;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InGame
{
    /// <summary>
    /// Player
    /// </summary>
    public class Player_Model
    {
        //--データとして保持する為に。
        private GameObject PlayerObject;
        private Rigidbody2D rb;
        //-----------------------
        /// <summary>
        /// 生成地点
        /// </summary>
        private Vector3 InstancePosition=new Vector3(0,0,0);

        //---ステータス部分-----------
        public float Speed { get; private set; } = 3.0f;

        /// <summary>
        /// 自機生成
        /// </summary>
        public GameObject PlayerCharacterGenerate(Vector3 GeneratePosition)
        {
            GameObject player = null;
            if (PlayerObject != null)
            {
                player = Object.Instantiate(PlayerObject, GeneratePosition, Quaternion.identity);
            }
            rb = player?.GetComponent<Rigidbody2D>();
            return player;
        }

        /// <summary>
        /// Addressable使用自機生成
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> PlayerCharacterAddressGenerate(string Chara_Address, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(Chara_Address);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                GameObject instance = Object.Instantiate(prefab,InstancePosition,Quaternion.identity);
                rb = instance.GetComponent<Rigidbody2D>();
                return instance;
            }
        }
        public void ChangeSpeed(float NewSpeed)
        {
            Speed = NewSpeed;
        }

        /// <summary>
        /// 生成地点変更
        /// </summary>
        /// <param name="NewPosition"></param>
        public void InstancePositionChange(Vector3 NewPosition)
        {
            InstancePosition = NewPosition;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="Speed"></param>
        /// <param name="rb"></param>
        /// <param name="actions"></param>
        public void PlayerMove(InputSystem_Actions actions)
        {
            rb.linearVelocity = actions.Player.Move.ReadValue<Vector3>() * Speed;
        }


    }
}