using UnityEngine;
using InGame.Model;
using InGame.View;
using InGame.NonMVP;
using Cysharp.Threading.Tasks;

namespace InGame.Presenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        [Header("キャラクターPrefabデータ")]
        [SerializeField] private GameObject CharacterPrefab;
        [SerializeField] private string characterAddress="PlayerCharacter";
        public GameObject characterPrefab { get; private set; }

        [Header("ソケット(コンセント）データ")]
        [SerializeField] private GameObject socketPrefab;
        [SerializeField] private GameObject socketTipPrefab;
        [SerializeField] private string socketAddress = "PlayerCharacter";
        
        /// <summary>
        /// model
        /// </summary>
        public PlayerModel model { get; private set; }
        private readonly TimerModel timerModel = new TimerModel();
        private ScoreModel scoreModel;
        
        /// <summary>
        /// view
        /// </summary>
        private GameHUDView view;
        public PlayerView animationView { get; private set; }
        
        /// <summary>
        /// controller
        /// </summary>
        private PlayerController playerController;

        private void Awake()
        {
            characterPrefab = CharacterPrefab;

            model = new PlayerModel();
            model.Initialize(this);

            //-------------------------------------------
            //デバッグ用生成。（Initializeを用意したほうがいいと思う。）
            model.SetInstancePosition(new Vector3(0, 0, 0));
            model.GeneratePlayerCharacter();
            //------------------------------------

            //コード生成に必要なクラスを取得。
            model?.SetGenerateCodeSystem(gameObject.GetComponent<GenerateCodeSystem>());

            view = gameObject.GetComponent<GameHUDView>();
            view.SetModel(model);
            animationView=view.GetplayerView();

            //スコアイベントの購読(Singletonの呼び出し）
            scoreModel = ScoreModel.Instance();
            scoreModel.scoreChanged += ScoreChanged;

            playerController = new PlayerController(model,this);
            playerController.Init();
        }
        
        private void Update()
        {
            view?.AnimationUpdate();
            view?.UpdatePlayerView(model.CalculatePercentOfCodeGaugePercent());
            view?.UpdateTimerView(timerModel.GetTimePersent());

            //Input系列の制御
            playerController?.Update();
        }
        
        /// <summary>
        /// PlayerControllerのFixedUpdate
        /// </summary>
        private void FixedUpdate() { playerController?.FixedUpdate(); }
        
        /// <summary>
        /// スコアの変更を監視し、HUDに反映する
        /// </summary>
        private void ScoreChanged() { view.UpdateScoreView(scoreModel.score); } 
        
        /// <summary>
        /// コンセントのPrefabを取得する
        /// </summary>
        /// <returns></returns>
        public GameObject GetSocketPrefab() { return socketPrefab; }
        
        /// <summary>
        /// コンセントに刺さったプラグの先端のPrefabを取得する
        /// </summary>
        /// <returns></returns>
        public GameObject GetSocketTipPrefab() { return socketTipPrefab; }
        
        public void DestroySocketTip()
        {
            foreach (Transform child in GetSocketPrefab().transform)
            {
                Destroy(child.gameObject);
            }
            model.socketTips.Clear();
        }
        
        /// <summary>
        /// スコアモデルのインスタンスを破壊
        /// </summary>
        private void OnDestroy() { scoreModel.scoreChanged -= ScoreChanged; }
    }
}
