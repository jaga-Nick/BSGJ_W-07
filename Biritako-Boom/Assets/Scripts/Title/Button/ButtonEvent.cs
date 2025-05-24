using Common;
using Title.Loader;
using UnityEngine;

namespace Title.Button
{
    public class ButtonEvent : MonoBehaviour
    {
        private readonly ISceneInfo _settingSceneLoader = new SettingSceneLoader();
        private readonly ISceneInfo _inGameSceneLoader = new InGameSceneLoader();
        private ISceneInfo _introSceneLoader = new IntroSceneLoader();

        /// <summary>
        /// Settingボタンを押してSettingシーンに遷移する
        /// </summary>
        public async void OnClickSettingButton()
        {
            await SceneManager.Instance().LoadMainScene(_settingSceneLoader);
        }

        /// <summary>
        /// Playボタンを押してInGameシーンに遷移する。一旦Introシーンはさむ。
        /// </summary>
        public async void OnClickPlayButton()
        {
            await SceneManager.Instance().LoadMainScene(_inGameSceneLoader);
        }

        /// <summary>
        /// Creditボタンを押してCreditPanelを開く。
        /// </summary>
        public async void OnClickCreditButton()
        {
            // await SceneManager.Instance().LoadMainScene(_settingSceneLoader);
        }

        /// <summary>
        /// Exitボタンを押してゲームを終了する。
        /// </summary>
        public void OnClickExitButton()
        {
            // Application.Quit();
            Debug.Log("Exit");
        }
    }
}
