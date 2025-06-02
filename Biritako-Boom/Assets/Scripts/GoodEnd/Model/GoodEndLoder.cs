using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace GoodEnd.Model
{
    public class GoodEndLoder : MonoBehaviour
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

