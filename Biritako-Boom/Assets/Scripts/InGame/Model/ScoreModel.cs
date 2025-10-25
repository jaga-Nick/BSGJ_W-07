using System;
using Common.AudioSystem;
using Common.GameSystem;


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
            AudioManager.Instance.PlaySe(AUDIO.SE_SCORED);
        }
        //加算
        public void IncrementScore(int num) { 
            Score += num;
            UpdateScoreInfo();
        }
        //減少
        public void DecrementScore(int num)
        {
            Score -= num;
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
