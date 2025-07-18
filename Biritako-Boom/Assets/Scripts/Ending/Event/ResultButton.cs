using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Ending.Loader;
using Title.Loader;

namespace Result.Model
{
    public class ResultButton : MonoBehaviour
    {
        private readonly ISceneInfo _inGameSceneLoader = new InGameSceneLoader();
        private readonly ISceneInfo _titleSceneLoader = new TitleSceneLoader();
        private readonly ISceneInfo _resultSceneLoader = new ResultSceneLoader();
        
        /// <summary>
        /// ReTryボタンを押したらInGameシーンに遷移する
        /// </summary>
        public async void OnClickRePlayButton()
        {
            await SceneManager.Instance().LoadMainScene(_inGameSceneLoader);
        }
        
        /// <summary>
        /// Titleボタンを押したらTitleシーンに遷移する
        /// </summary>
        public async void OnClickTitleButton()
        {
            await SceneManager.Instance().LoadMainScene(_titleSceneLoader);
        }
        
        /// <summary>
        /// Exitボタンを押したらゲームを終了する
        /// </summary>
        public void OnClickExitButton()
        {
            #if UNITY_EDITOR                                        // Unityエディタ上で実行中の場合
                UnityEditor.EditorApplication.isPlaying = false;    // エディタの再生を停止
            #else                                                   // ビルドされた実行ファイルの場合
                Application.Quit();                                 // アプリケーションを終了
            #endif
        }
    }
}
