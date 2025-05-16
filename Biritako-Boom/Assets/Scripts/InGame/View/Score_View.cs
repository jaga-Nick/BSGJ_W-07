using TMPro;
using UnityEngine;

namespace Score
{
    public class Score_View : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// ÉXÉRÉAÇUIÇ…îΩâfÇµÇ‹Ç∑
        /// </summary>
        public void DisplayScore(int score)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
