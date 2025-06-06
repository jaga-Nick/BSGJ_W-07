using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Result.Model
{
    public class ResultScoreModel : MonoBehaviour
    {
        private List<int> _scores = new List<int>();
    
        public void ScoreGen()
        {
            GenerateDummyScore();
        }
    
        public async UniTask<int> GetScore()
        {
            await UniTask.Delay(500); // 0.5秒待機
            if (_scores.Any())
            {
                return _scores.OrderByDescending(score => score).First(); // 例として一番高いスコアを返す
            }
            return 0; // スコアがない場合は0を返す
        }
    
        public void AddScore(int score)
        {
            _scores.Add(score);
        }
    
        private void GenerateDummyScore()
        {
            _scores.Clear();
            _scores.Add(UnityEngine.Random.Range(100, 100000));
        }
    }
}

