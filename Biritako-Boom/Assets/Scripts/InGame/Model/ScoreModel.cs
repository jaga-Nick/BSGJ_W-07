using System;
using Common;
using Setting;
//
namespace InGame.Model
{
    //シーン間のデータ以降が行われる為。DontDestroy化
    public class ScoreModel : SingletonMonoBehaviourBase<ScoreModel>
    {
        public int score { get; private set; }

        public event Action scoreChanged;

        //変更イベント
        private void UpdateScoreInfo()
        {
            scoreChanged?.Invoke();
            AudioManager.Instance().LoadSoundEffect("Scored");
        }
        //加算
        public void IncrementScore(int Num) { 
            score += Num;
            UpdateScoreInfo();
        }
        //減少
        public void DecrementScore(int Num)
        {
            score -= Num;
            UpdateScoreInfo();
        }
        //初期化
        public void RestoreScore() 
        {
            score = 0;
            UpdateScoreInfo ();
        }
    }
}
