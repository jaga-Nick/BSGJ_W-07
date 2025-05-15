using TMPro;
using UnityEngine;

namespace Score
{
    public class Score_View : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// �X�R�A��UI�ɔ��f���܂�
        /// </summary>
        public void DisplayScore(int score)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
