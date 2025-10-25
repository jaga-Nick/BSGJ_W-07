using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.View
{
    /// <summary>
    /// Scoreの表示
    /// </summary>
    [Serializable]
    public class ScoreView
    {
        [FormerlySerializedAs("ScoreText")] [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// スコアのテキストを表示する。
        /// </summary>
        public void DisplayScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
