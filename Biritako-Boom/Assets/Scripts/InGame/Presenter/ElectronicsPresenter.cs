using System;
using System.Collections;
using InGame.Model;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Presenter
{
    public class ElectronicsPresenter : MonoBehaviour
    {
        /// <summary>
        /// 移動パラメータ
        /// </summary>
        [Header("移動速度")]
        [SerializeField] private float moveSpeed = 3f;
        [Header("前進する秒数")]
        [SerializeField] private float moveDuration = 3f;
        
        /// <summary>
        /// modelとview
        /// </summary>
        private ElectronicsModel _model;
        private ElectronicsView _view;
        private UfoModel _ufoModel;
        
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
            _model = gameObject.GetComponent<ElectronicsModel>();
            _model.Initialize(gameObject.GetComponent<Rigidbody2D>(),transform.position);
            
            _view = gameObject.GetComponent<ElectronicsView>();

            StartCoroutine(MoveCharacterRoutine());
        }

        /// <summary>
        /// Enemyをスポーンする座標を決める
        /// </summary>
        public Vector3 DetermineSpawnPoints()
        {
            // UFOのmodelを取得して座標を確認
            // _ufoModel = new UfoModel();
            
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
            do { value = Random.Range(-0.5f, 1.5f); } while (value is >= 0.0f and <= 1.0f);
            return value;
        }
        
        /// <summary>
        /// キャラクターをランダムな方向に向かせて前進させるコルーチン
        /// </summary>
        private IEnumerator MoveCharacterRoutine()
        {
            // Modelにランダムな移動方向を問い合わせる
            var moveDirection = _model.GetRandomDirection();
            
            // modelにセットする
            _model.Speed = moveSpeed;
            _model.Direction = moveDirection;
            
            // 移動開始時にアニメーションを再生
            _view.PlayMoveAnimation(true);

            while (true)
            {
                var elapsedTime = 0f;

                while (elapsedTime < moveDuration)
                {
                    _model.Rb.linearVelocity = _model.Direction * _model.Speed;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                _model.Direction *= -1;
            }
        }
    }
}