using UnityEngine;


namespace Score
{
    public class Score_Model : MonoBehaviour
    {
        private int _score;

        public void ResetScore()
        {
            _score = 0;
        }

        public void AddScore(int score)
        {
            _score += score;
        }

        public int GetScore()
        {
            return _score;
        }
    }
}