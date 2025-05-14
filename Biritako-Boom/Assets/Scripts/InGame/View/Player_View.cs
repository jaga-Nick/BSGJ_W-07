using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Common;
using System;
using System.Threading;
using UnityEngine.InputSystem;
namespace InGame
{
    /// <summary>
    /// 
    /// </summary>
    public class Player_View : MonoBehaviour
    {
        [Header("Demo-今は一応Addressableを使用しないやり方で。")]
        [SerializeField]
        private GameObject PlayerCharacter;
        private Rigidbody2D rb;
        private InputSystem_Actions _inputActions;
        private GameObject Character;

        private void Awake()
        {

            _inputActions = InputSystemActionsManager.Instance().GetInputSystem_Actions();
        }

        private void Update()
        {
            PlayerMove(3.0f,rb,_inputActions);
        }

        /// <summary>
        /// 自機生成
        /// </summary>
        public GameObject PlayerCharacterGenerate(Vector3 GeneratePosition)
        {
            GameObject player=null;
            if (PlayerCharacter != null)
            {
                player = Instantiate(PlayerCharacter, GeneratePosition, Quaternion.identity);
            }
            rb = player?.GetComponent<Rigidbody2D>();
            return player;
        }

        /// <summary>
        /// Addressable使用自機生成
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> PlayerCharacterAddressGenerate(string Chara_Address,CancellationToken cancellationToken)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(Chara_Address);

            using(new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                GameObject instance = Instantiate(prefab);
                rb = instance.GetComponent<Rigidbody2D>();
                return instance;
            }
        }

        /// <summary>
        /// 適当にプレイヤー動かす。
        /// </summary>
        /// <param name="Speed"></param>
        /// <param name="rb"></param>
        /// <param name="actions"></param>
        public void PlayerMove(float Speed,Rigidbody2D rb,InputSystem_Actions actions)
        {
            rb.linearVelocity = actions.Player.Move.ReadValue<Vector3>()*Speed; 
        }
    }
}