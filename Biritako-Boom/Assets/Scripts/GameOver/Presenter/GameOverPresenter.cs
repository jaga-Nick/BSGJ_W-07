using Cysharp.Threading.Tasks;
using GameOver.Model;
using UnityEngine;

namespace GameOver.Presenter
{
    public class GameOverPresenter : MonoBehaviour
    {
        GameOverLoder gameOverLoder = new GameOverLoder();
        async　void Start()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(5));// 5秒待機
            gameOverLoder.OnResultLoder();
        }
    }
}

