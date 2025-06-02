using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Result.Model
{
    public class ResultButton : MonoBehaviour
    {
        
        
        /// <summary>
        /// ReTryボタンを押したらInGameシーンに遷移する
        /// </summary>
        public async UniTask OnClickReTryButton()
        {
            // await SceneManager.Instance().LoadMainScene(name);
        }
        
        /// <summary>
        /// Titleボタンを押したらTitleシーンに遷移する
        /// </summary>
        public async UniTask OnClickTitleButton()
        {
            // await SceneManager.Instance().LoadMainScene(name);
        }
        
        /// <summary>
        /// Exitボタンを押したらゲームを終了する
        /// </summary>
        public async UniTask OnClickExitButton()
        {
            #if UNITY_EDITOR                                        // Unityエディタ上で実行中の場合
                UnityEditor.EditorApplication.isPlaying = false;    // エディタの再生を停止
            #else                                                   // ビルドされた実行ファイルの場合
                Application.Quit();                                 // アプリケーションを終了
            #endif
        }
    }
}
