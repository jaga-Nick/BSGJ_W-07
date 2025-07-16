using UnityEngine;
using UnityEngine.UI;
using Common;
using InGame.Model;

namespace InGame.NonMVP
{
    /// <summary>
    /// ゲームの流れを管理するクラス。
    /// UI周りもこちらで行う。
    /// </summary>
    public class InGameManager : DestroyAvailable_SingletonMonoBehaviourBase<InGameManager>
    {
        /// <summary>
        /// ゲームタイムのパラメタ
        /// </summary>
        [Header("ゲームタイムのパラメータ")]
        [Header("ゲームの全体時間（秒）")]
        [SerializeField] private float inGameSecond;
        
        private float _time;
        
        /// <summary>
        /// uGUI
        /// </summary>
        [Header("タイマーのUI")]
        [SerializeField] private Image timerUI;
        [Header("開始の色")]
        [ColorUsage(false, false), SerializeField] private Color startColor;
        [Header("終端の色")]
        [ColorUsage(false, false), SerializeField] private Color endColor;
        // [SerializeField] private Button pauseButton;
        
        InGameSceneEvent _sceneEvent =  new InGameSceneEvent();
        
        private void Start()
        {
            // ゲーム開始時はゲームタイムを0にする。
            TimeManager.Instance().ResetInGameTime(inGameSecond);
        }
        
        private void OnEnable()
        {
            MotherShipModel.OnGameClear += HandleGameClear;
        }

        private void OnDisable()
        {
            MotherShipModel.OnGameClear -= HandleGameClear;
        }

        /// <summary>
        /// UniTaskにする
        /// </summary>
        private void Update()
        {
            _time = TimeManager.Instance().GetInGameTime();
            
            UpdateTimerUI(_time);
            
            if (0f >= _time) // 時間切れ
            {
                _sceneEvent.OnGameOverLoder();
            }
        }
        
        /// <summary>
        /// イベントによって呼び出される処理
        /// </summary>
        private void HandleGameClear(bool clearFlag)
        {
            if (0f < _time && clearFlag)
            {
                this.enabled = false; // 自分自身のUpdateを止め、OnDisableを呼び出してイベントを解除する
                _sceneEvent.OnGameClearLoder();
            }
        }

        /// <summary>
        /// タイマーのUIの更新
        /// </summary>
        /// <param name="time"></param>
        private void UpdateTimerUI(float time)
        {
            timerUI.fillAmount = time / inGameSecond;
            // 時間経過とともに色を変えていく
            var t = Mathf.Clamp01(time / inGameSecond);
            var color = Color.Lerp(endColor, startColor, t);
            color.a = 1.0f;
            timerUI.color = color;
        }
    }
}