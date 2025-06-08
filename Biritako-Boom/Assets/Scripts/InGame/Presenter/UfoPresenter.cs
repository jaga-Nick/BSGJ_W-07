using System.Collections;
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
        [SerializeField] private float moveSpeed = 1f;
        [Header("前進する秒数")]
        [SerializeField] private float moveDuration = 0.5f;
        
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
        private UfoView _view;
        
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
                Rb = gameObject.GetComponent<Rigidbody2D>(),
                MaxUfoHp = maxUfoHp,
                CurrentUfoHp = maxUfoHp,
                UfoScore = ufoScore,
            };
            
            StartCoroutine(MoveCharacterRoutine());
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

        private IEnumerator MoveCharacterRoutine()
        {
            // Modelにランダムな移動方向を問い合わせる
            // modelにセットする
            _model.Speed = moveSpeed;
            // 移動開始時にアニメーションを再生
            var direction = Vector2.up;
            while (true)
            {
                var elapsedTime = 0f;
                while (elapsedTime < moveDuration)
                {
                    _model.Rb.linearVelocity = direction * _model.Speed;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                direction *= -1;
            }
        }

        private void OnDestroy()
        {
            _model?.DestroyUfo(gameObject);
        }
    }
}
