﻿using Common;
using Title.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Title.Button
{
    /// <summary>
    /// ボタンを押した時の処理
    /// </summary>
    public class ButtonEvent : MonoBehaviour
    {
        /// <summary>
        /// Loader
        /// </summary>
        private readonly ISceneInfo _settingSceneLoader = new SettingSceneLoader();
        // private readonly ISceneInfo _introSceneLoader = new IntroSceneLoader();
        private readonly ISceneInfo _inGameSceneLoader = new InGameSceneLoader();
        

        /// <summary>
        /// Panel
        /// </summary>
        [SerializeField] private GameObject creditPanel;
        [SerializeField] private Image fadePanel;
        
        /// <summary>
        /// Manager
        /// </summary>
        private FadeManager _sceneFadeManager;
        private FadeManager _panelFadeManager;

        private void Start()
        {
            _sceneFadeManager = new FadeManager(fadePanel);
            _panelFadeManager = new FadeManager(fadePanel);
            // 最初はクレジットパネルは隠す
            creditPanel.SetActive(false);
        }

        /// <summary>
        /// Settingボタンを押してSettingシーンに遷移する。
        /// </summary>
        public async void OnClickSettingButton()
        {
            await _sceneFadeManager.End();
            await SceneManager.Instance().LoadMainScene(_settingSceneLoader);
            await _sceneFadeManager.Init();
        }

        /// <summary>
        /// Playボタンを押してInGameシーンに遷移する。
        /// </summary>
        public async void OnClickPlayButton()
        {
            await _sceneFadeManager.End();
            // await SceneManager.Instance().LoadMainScene(_introSceneLoader);
            await SceneManager.Instance().LoadMainScene(_inGameSceneLoader);
            await _panelFadeManager.Init();
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
