using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Ending.Loader;


namespace Ending.Model
{
    public class EndingEvent : MonoBehaviour
    {
        private readonly ISceneInfo _resultSceneLoader = new ResultSceneLoader();
        
        /// <summary>
        /// Resultシーンに遷移する
        /// </summary>
        public async UniTask OnResultLoder()
        {
            await SceneManager.Instance().LoadMainScene(_resultSceneLoader);
            Debug.Log("通りました");
        }
    }
}

