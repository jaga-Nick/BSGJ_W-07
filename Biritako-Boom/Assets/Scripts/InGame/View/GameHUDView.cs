using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.View
{
    /// <summary>
    ///  InGame.View統括
    /// </summary>

    public class GameHUDView : MonoBehaviour
    {
        private PlayerView playerView;
        private ScoreView scoreView;
        private TimerView timerView;

        //初期化。
        private void Awake()
        {
            playerView=gameObject.GetComponent<PlayerView>();
            scoreView = gameObject.GetComponent<ScoreView>();
            timerView = gameObject.GetComponent<TimerView>();
        }


        public void UpdatePlayerView(int gauge)=>playerView?.DisplayCodeGauge(gauge);
        public void UpdateScoreView(int score)=>scoreView?.DisplayScore(score);
        public void UpdateTimerView(float time) => timerView?.DisplayTimer(time);
    }
}