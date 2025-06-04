using UnityEngine;
using InGame.Model;
using InGame.View;
using System.Security.Cryptography.X509Certificates;
using InGame.NonMVP;

namespace InGame.Presenter
{
    /// <summary>
    /// Playerのpresenter
    /// </summary>
    public class PlayerPresenter:MonoBehaviour
    {
        [Header("キャラクターPrefabデータ")]
        [SerializeField]
        private GameObject CharacterPrefab;
        [SerializeField]
        private string CharacterAddress="PlayerCharacter";

        //----------------------------------------------

        //Player統括
        private PlayerModel Model;
        private GameHUDView View;
        
        //システムモデル
        private TimerModel timerModel=new TimerModel();
        private ScoreModel scoreModel;

        private PlayerController playerController;



        private void Awake()
        {
            Model = new PlayerModel();

            View = gameObject.GetComponent<GameHUDView>();
            //スコアイベントの購読(Singletonの呼び出し）
            scoreModel = ScoreModel.Instance();
            scoreModel.ScoreChanged += ScoreChanged;

            playerController = new PlayerController(Model);
        }

        private void Update()
        {
            View?.UpdatePlayerView(Model.GetCodeGaugePercent());
            View?.UpdateTimerView(timerModel.GetTimePersent());

            //Input系列の制御
            playerController.Update();
        }

        private void FixedUpdate()
        {
            playerController.FixedUpdate();
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
