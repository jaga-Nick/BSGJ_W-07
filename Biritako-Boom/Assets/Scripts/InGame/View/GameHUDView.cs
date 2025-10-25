using InGame.Model;
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
        private PlayerModel _model;

        [Header("ゲージとPlayerCharacterAnimation")]
        [SerializeField]
        private PlayerView playerView;
        [Header("ScoreとTimer")]
        [SerializeField]
        private ScoreView scoreView;
        [SerializeField]
        private TimerView timerView;

        public void Start()
        {
            playerView.SetPlayerModel(_model);
            playerView.Init();
        }

        public void SetModel(PlayerModel model)
        {
            this._model = model;
        }

        public PlayerView GetplayerView()
        {
            return playerView;
        }

        public void AnimationUpdate() => playerView?.AnimationUpdate();
        public void UpdatePlayerView(float gauge)=>playerView?.DisplayCodeGauge(gauge);
        public void UpdateScoreView(int score)=>scoreView?.DisplayScore(score);
        public void UpdateTimerView(float time) => timerView?.DisplayTimer(time);
    }
}