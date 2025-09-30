using System;
using TMPro;
using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// Scoreの表示
    /// </summary>
    [Serializable]
    public class ScoreView
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// スコアのテキストを表示する。
        /// </summary>
        public void DisplayScore(int Score)
        {
            scoreText.text = Score.ToString();
        }
    }
}
