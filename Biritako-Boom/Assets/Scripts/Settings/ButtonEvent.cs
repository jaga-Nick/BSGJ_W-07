using Common;
using UnityEngine;
using Title.Loader;
using UnityEngine.UI;

namespace Settings.Button
{
    public class ButtonEvent : MonoBehaviour
    {
        /// <summary>
        /// Loader
        /// </summary>
        private readonly ISceneInfo _titleSceneLoader = new TitleSceneLoader();
        
        /// <summary>
        /// Panel
        /// </summary>
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
        }

        /// <summary>
        /// Settingボタンを押してSettingシーンに遷移する。
        /// </summary>
        public async void OnCloseSettingButton()
        {
            await _sceneFadeManager.End();
            await SceneManager.Instance().LoadMainScene(_titleSceneLoader);
            await _sceneFadeManager.Init();
        }
    }

}