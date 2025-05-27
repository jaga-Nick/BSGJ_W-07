using TMPro;
using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// Scoreの表示
    /// </summary>
    public class ScoreView : MonoBehaviour
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
