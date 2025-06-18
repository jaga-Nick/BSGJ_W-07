using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.NonMVP
{
    /// <summary>
    /// ゲームの流れを管理するクラス。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// ゲームタイムのパラメタ
        /// </summary>
        [Header("ゲーム時間（分）")]
        [SerializeField] private float inGameMinutes;
        
        
        /// <summary>
        /// UI
        /// </summary>
        [SerializeField] private TextMeshProUGUI timerText;
        // [SerializeField] private Button pauseButton;
        
        private void Start()
        {
            // ゲーム開始時はゲームタイムを0にする。
            TimeManager.Instance().ResetInGameTime(inGameMinutes);
        }

        /// <summary>
        /// UniTaskにする
        /// </summary>
        private void Update()
        {
            var time = TimeManager.Instance().GetInGameTime();
            timerText.text = time.ToString("F2");
        }
    }
}