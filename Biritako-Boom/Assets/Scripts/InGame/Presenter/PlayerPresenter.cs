using UnityEngine;
using InGame.Model;
using InGame.View;
using InGame.NonMVP;
using Cysharp.Threading.Tasks;

namespace InGame.Presenter
{
    public class PlayerPresenter:MonoBehaviour
    {
        [Header("キャラクターPrefabデータ")]
        [SerializeField]
        private GameObject CharacterPrefab;

        public GameObject characterPrefab { get; private set; }

        [SerializeField]
        private string CharacterAddress="PlayerCharacter";

        [Header("ソケット(コンセント）データ")]
        [SerializeField]
        private GameObject SocketPrefab;
        [SerializeField]
        private string SocketAddress = "PlayerCharacter";
        //----------------------------------------------

        //Player統括
        public PlayerModel Model { get; private set; }
        private GameHUDView View;

        public PlayerView animationView { get; private set; }

        
        //システムモデル
        private TimerModel timerModel=new TimerModel();
        private ScoreModel scoreModel;

        private PlayerController playerController;

        private void Awake()
        {
            characterPrefab = CharacterPrefab;

            Model = new PlayerModel();
            Model.Initialize(this);

            //-------------------------------------------
            //デバッグ用生成。（Initializeを用意したほうがいいと思う。）
            Model.SetInstancePosition(new Vector3(0, 0, 0));
            Model.GeneratePlayerCharacter();
            //------------------------------------

            //コード生成に必要なクラスを取得。
            Model?.SetGenerateCodeSystem(gameObject.GetComponent<GenerateCodeSystem>());

            View = gameObject.GetComponent<GameHUDView>();
            View.SetModel(Model);
            animationView=View.GetplayerView();

            //スコアイベントの購読(Singletonの呼び出し）
            scoreModel = ScoreModel.Instance();
            scoreModel.ScoreChanged += ScoreChanged;

            playerController = new PlayerController(Model,this);
            playerController.Init();
        }
        private void Update()
        {
            View?.AnimationUpdate();
            View?.UpdatePlayerView(Model.CalculatePercentOfCodeGaugePercent());
            View?.UpdateTimerView(timerModel.GetTimePersent());

            //Input系列の制御
            playerController?.Update();
        }
        private void FixedUpdate()
        {
            playerController?.FixedUpdate();
        }

        //購読解除対策
        private void ScoreChanged()
        {
            View.UpdateScoreView(scoreModel.Score);
        } 
        public GameObject GetSocketPrefab()
        {
            return SocketPrefab;
        }
        private void OnDestroy()
        {
            //購読解除
            scoreModel.ScoreChanged -= ScoreChanged;
        }


    }
}
