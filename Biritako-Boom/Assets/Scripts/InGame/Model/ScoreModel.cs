using System;
using Common;
using UnityEngine;

//
namespace InGame.Model
{
    //シーン間のデータ以降が行われる為。DontDestroy化
    public class ScoreModel : SingletonMonoBehaviourBase<ScoreModel>
    {
        public int Score { get; private set; }

        public event Action ScoreChanged;

        //変更イベント
        private void UpdateScoreInfo()
        {
            ScoreChanged?.Invoke();
        }
        //加算
        public void IncrementScore(int Num) { 
            Score += Num;
            UpdateScoreInfo();
        }
        //減少
        public void DecrementScore(int Num)
        {
            Score -= Num;
            UpdateScoreInfo();
        }
        //初期化
        public void RestoreScore() 
        {
            Score = 0;
            UpdateScoreInfo ();
        }
    }
}
