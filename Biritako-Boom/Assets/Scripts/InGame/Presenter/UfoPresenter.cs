using UnityEngine;
using InGame.Model;
using InGame.View;

namespace InGame.Presenter
{
    public class UfoPresenter : MonoBehaviour
    {
        /// <summary>
        /// 移動パラメータ
        /// </summary>
        [Header("移動速度")]
        [SerializeField] private float moveSpeed = 3f;
        [Header("前進する秒数")]
        [SerializeField] private float moveDuration = 3f;
        
        /// <summary>
        /// UFOのステータス
        /// </summary>
        [Header("UFOの最大HP")]
        [SerializeField] private int maxUfoHp = 100;
        [Header("UFOのスコア")]
        [SerializeField] private int ufoScore = 100;

        /// <summary>
        /// modelとview
        /// </summary>
        private UfoModel _model;
        private ElectronicsView _view;
        
        /// <summary>
        /// Camera
        /// </summary>
        private Camera _camera;
        
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _model = new UfoModel
            {
                MaxUfoHp = maxUfoHp,
                CurrentUfoHp = maxUfoHp,
            };
        }
        
        /// <summary>
        /// UFOのスポーンする座標を決める
        /// </summary>
        public Vector3 DetermineSpawnPoints()
        {
            // ランダムな座標を生成
            var randomPositionX = RandomRun();
            var randomPositionY = RandomRun();

            // 画面外の座標を取得
            var position = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionX, randomPositionY, _camera.nearClipPlane));
            return position;
        }
        
        /// <summary>
        /// ランダムな値を取得
        /// </summary>
        /// <returns></returns>
        private static float RandomRun()
        {
            float value;
            do { value = Random.Range(-1.5f, 2.0f); } while (value is >= 0.0f and <= 1.0f);
            return value;
        }

        private void OnDestroy()
        {
            _model?.DestroyUfo(gameObject);
        }
    }
}
