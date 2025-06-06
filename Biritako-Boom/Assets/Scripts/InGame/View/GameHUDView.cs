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
        [Header("ゲージとPlayerCharacterAnimation")]
        [SerializeField]
        private PlayerView playerView = new PlayerView();
        [Header("ScoreとTimer")]
        [SerializeField]
        private ScoreView scoreView = new ScoreView();
        [SerializeField]
        private TimerView timerView=new TimerView();

        public void UpdatePlayerView(float gauge)=>playerView?.DisplayCodeGauge(gauge);
        public void UpdateScoreView(int score)=>scoreView?.DisplayScore(score);
        public void UpdateTimerView(float time) => timerView?.DisplayTimer(time);
    }
}