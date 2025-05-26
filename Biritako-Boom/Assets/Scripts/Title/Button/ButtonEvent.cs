using Common;
using Title.Loader;
using Title.View;
using UnityEngine;
using UnityEngine.UI;

namespace Title.Button
{
    public class ButtonEvent : MonoBehaviour
    {
        private readonly ISceneInfo _settingSceneLoader = new SettingSceneLoader();
        private readonly ISceneInfo _inGameSceneLoader = new InGameSceneLoader();
        private ISceneInfo _introSceneLoader = new IntroSceneLoader();

        [SerializeField] private GameObject creditPanel;

        private void Start()
        {
            OnClickCloseCreditButton();
        }

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
        public void OnClickOpenCreditButton()
        {
            creditPanel.SetActive(true);
        }

        /// <summary>
        /// Closeボタンを押してCreditPanelを閉じる。
        /// </summary>
        public void OnClickCloseCreditButton()
        {
            creditPanel.SetActive(false);
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
