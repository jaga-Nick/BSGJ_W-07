using UnityEngine;
using InGame.Model;
using InGame.View;
using System.Security.Cryptography.X509Certificates;

namespace InGame.Presenter
{
    /// <summary>
    /// Playerのpresenter
    /// </summary>
    public class PlayerPresenter:MonoBehaviour
    {
        private PlayerModel Model = new PlayerModel();
        private GameHUDView View;


        private TimerModel timerModel=new TimerModel();
        private ScoreModel scoreModel;
        private void Awake()
        {
            //スコアイベントの購読
            scoreModel = ScoreModel.Instance();
            scoreModel.ScoreChanged += ScoreChanged;
            

        }

        private void Update()
        {
            View.UpdatePlayerView(Model.GetCodeGaugePercent());
            View.UpdateTimerView(timerModel.GetTimePersent());
        }

        //購読解除対策
        private void ScoreChanged()
        {
            View.UpdateScoreView(scoreModel.Score);
        } 

        private void OnDestroy()
        {
            //購読解除
            scoreModel.ScoreChanged -= ScoreChanged;
        }
    }
}
