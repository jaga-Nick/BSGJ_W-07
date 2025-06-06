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
        [SerializeField] private TextMeshProUGUI ScoreText;

        /// <summary>
        /// スコアのテキストを表示する。
        /// </summary>
        public void DisplayScore(int Score)
        {
            ScoreText.text = "Score: " + Score.ToString();
        }
    }
}
