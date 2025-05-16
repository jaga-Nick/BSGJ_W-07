using TMPro;
using UnityEngine;

namespace InGame.View
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// スコアのテキストを表示する。
        /// </summary>
        public void DisplayScore(int score)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
