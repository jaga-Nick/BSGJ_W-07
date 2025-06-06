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
    
        /// <summary>
        /// リスト内の最大値のスコアを取得
        /// </summary>
        /// <returns></returns>
        public async UniTask<int> GetScore()
        {
            await UniTask.Delay(500); // 0.5秒待機
            if (_scores.Any())
            {
                return _scores.OrderByDescending(score => score).First(); // 例として一番高いスコアを返す
            }
            return 0; // スコアがない場合は0を返す
        }
    
        /// <summary>
        /// リストにスコアを追加
        /// </summary>
        /// <param name="score"></param>
        public void AddScore(int score)
        {
            _scores.Add(score);
        }
    
        
        /// <summary>
        /// ダミーのスコア生成(テスト用)
        /// </summary>
        private void GenerateDummyScore()
        {
            _scores.Clear();
            _scores.Add(UnityEngine.Random.Range(100, 100000));
        }
    }
}

