using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Result.Loader
{
    public class TitleSceneLoader : ISceneInfo
    {
        public string SceneName => "Title";

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

