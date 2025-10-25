using UnityEngine;
using InGame.Model;
using InGame.View;
using InGame.NonMVP;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace InGame.Presenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        [FormerlySerializedAs("CharacterPrefab")]
        [Header("キャラクターPrefabデータ")]
        [SerializeField] private GameObject characterPrefab;
        [FormerlySerializedAs("CharacterAddress")] [SerializeField] private string characterAddress="PlayerCharacter";
        public GameObject CharacterPrefab { get; private set; }

        [FormerlySerializedAs("SocketPrefab")]
        [Header("ソケット(コンセント）データ")]
        [SerializeField] private GameObject socketPrefab;
        [FormerlySerializedAs("SocketTipPrefab")] [SerializeField] private GameObject socketTipPrefab;
        [FormerlySerializedAs("SocketAddress")] [SerializeField] private string socketAddress = "PlayerCharacter";
        
        /// <summary>
        /// model
        /// </summary>
        public PlayerModel Model { get; private set; }
        private readonly TimerModel _timerModel = new TimerModel();
        private ScoreModel _scoreModel;
        
        /// <summary>
        /// view
        /// </summary>
        private GameHUDView _view;
        public PlayerView AnimationView { get; private set; }
        
        /// <summary>
        /// controller
        /// </summary>
        private PlayerController _playerController;

        private void Awake()
        {
            CharacterPrefab = CharacterPrefab;

            Model = new PlayerModel();
            Model.Initialize(this);

            //-------------------------------------------
            //デバッグ用生成。（Initializeを用意したほうがいいと思う。）
            Model.SetInstancePosition(new Vector3(0, 0, 0));
            Model.GeneratePlayerCharacter();
            //------------------------------------

            //コード生成に必要なクラスを取得。
            Model?.SetGenerateCodeSystem(gameObject.GetComponent<GenerateCodeSystem>());

            _view = gameObject.GetComponent<GameHUDView>();
            _view.SetModel(Model);
            AnimationView=_view.GetplayerView();

            //スコアイベントの購読(Singletonの呼び出し）
            _scoreModel = ScoreModel.Instance;
            _scoreModel.ScoreChanged += ScoreChanged;

            _playerController = new PlayerController(Model,this);
            _playerController.Init();
        }
        
        private void Update()
        {
            _view?.AnimationUpdate();
            _view?.UpdatePlayerView(Model.CalculatePercentOfCodeGaugePercent());
            _view?.UpdateTimerView(_timerModel.GetTimePersent());

            //Input系列の制御
            _playerController?.Update();
        }
        
        /// <summary>
        /// PlayerControllerのFixedUpdate
        /// </summary>
        private void FixedUpdate() { _playerController?.FixedUpdate(); }
        
        /// <summary>
        /// スコアの変更を監視し、HUDに反映する
        /// </summary>
        private void ScoreChanged() { _view.UpdateScoreView(_scoreModel.Score); } 
        
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
            Model.socketTips.Clear();
        }
        
        /// <summary>
        /// スコアモデルのインスタンスを破壊
        /// </summary>
        private void OnDestroy() { _scoreModel.ScoreChanged -= ScoreChanged; }
    }
}
