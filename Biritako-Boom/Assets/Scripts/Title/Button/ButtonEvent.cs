using Common;
using Title.Loader;
using Title.View;
using UnityEngine;
using UnityEngine.UI;

namespace Title.Button
{
    public class ButtonEvent : MonoBehaviour
    {
        /// <summary>
        /// Loader
        /// </summary>
        private readonly ISceneInfo _settingSceneLoader = new SettingSceneLoader();
        private readonly ISceneInfo _introSceneLoader = new IntroSceneLoader();

        /// <summary>
        /// Panel
        /// </summary>
        [SerializeField] private GameObject creditPanel;
        [SerializeField] private Image fadePanel;
        
        /// <summary>
        /// Manager
        /// </summary>
        private FadeManager _fadeManager;

        private void Start()
        {
            _fadeManager = new FadeManager();
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
            await SceneManager.Instance().LoadMainScene(_introSceneLoader);
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
