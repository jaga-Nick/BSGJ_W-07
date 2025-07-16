using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace InGame.Loader
{
    public class GameOverSceneLoader : ISceneInfo
    {
        public string SceneName => "GameOver";

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