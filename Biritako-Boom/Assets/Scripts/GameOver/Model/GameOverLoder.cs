using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameOver.Model
{
    public class GameOverLoder : MonoBehaviour
    {
        /// <summary>
        /// Resultシーンに遷移する
        /// </summary>
        public async UniTask OnResultLoder()
        {
            // await SceneManager.Instance().LoadMainScene(name);
        }
    }
}
