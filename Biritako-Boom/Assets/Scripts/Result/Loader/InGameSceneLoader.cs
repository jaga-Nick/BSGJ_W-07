using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Result.Loader
{
    public class InGameSceneLoader : MonoBehaviour
    {
        public string SceneName => "InGame";

        public bool IsDefault => throw new System.NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public async UniTask End()
        {
            Debug.Log("テスト_end");
        }

        /// <summary>
        /// 
        /// </summary>
        public async UniTask Init()
        {
            Debug.Log("テスト_Init");
        }

        public void InputStart()
        {
        }

        public void InputStop()
        {
        } 
    }
}

