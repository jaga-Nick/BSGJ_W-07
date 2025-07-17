using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using InGame.Loader;


namespace InGame.Model
{
    public class InGameSceneEvent : MonoBehaviour
    {
        private readonly ISceneInfo _gameclearSceneLoader = new GameClearSceneLoader();
        private readonly ISceneInfo _gameoverSceneLoader = new GameOverSceneLoader();
        
        /// <summary>
        /// Resultシーンに遷移する
        /// </summary>
        public async UniTask OnGameClearLoder()
        {
            await SceneManager.Instance().LoadMainScene(_gameclearSceneLoader);
            Debug.Log("通りました");
        }
        
        
        /// <summary>
        /// Resultシーンに遷移する
        /// </summary>
        public async UniTask OnGameOverLoder()
        {
            await SceneManager.Instance().LoadMainScene(_gameoverSceneLoader);
            Debug.Log("通りました");
        }
    }
}
