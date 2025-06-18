using UnityEngine;
using UnityEngine.UI;

namespace InGame.NonMVP
{
    /// <summary>
    /// ゲームの流れを管理するクラス。
    /// UI周りもこちらで行う。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// ゲームタイムのパラメタ
        /// </summary>
        [Header("ゲームタイムのパラメータ")]
        [Header("ゲームの全体時間（秒）")]
        [SerializeField] private float inGameSecond;
        
        /// <summary>
        /// uGUI
        /// </summary>
        [Header("タイマーのUI")]
        [SerializeField] protected Image timerUI;
        [Header("開始の色")]
        [ColorUsage(false, false), SerializeField] private Color startColor;
        [Header("終端の色")]
        [ColorUsage(false, false), SerializeField] private Color endColor;
        // [SerializeField] private Button pauseButton;
        
        private void Start()
        {
            // ゲーム開始時はゲームタイムを0にする。
            TimeManager.Instance().ResetInGameTime(inGameSecond);
        }

        /// <summary>
        /// UniTaskにする
        /// </summary>
        private void Update()
        {
            var time = TimeManager.Instance().GetInGameTime();
            UpdateTimerUI(time);
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
            timerUI.color = Color.Lerp(endColor, startColor, t);
        }
    }
}