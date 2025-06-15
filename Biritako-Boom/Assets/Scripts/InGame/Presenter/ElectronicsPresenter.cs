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

            // StartCoroutine(MoveCharacterRoutine());
        }
        
        /// <summary>
        /// 家電をスポーンする座標を決める
        /// </summary>
        /// <param name="ufoPosition"></param>
        /// <param name="spawnRadius"></param>
        /// <param name="exclusionRadius"></param>
        /// <returns></returns>
        public Vector3 DetermineSpawnPoints(Vector3 ufoPosition, float spawnRadius, float exclusionRadius)
        {
            // UFOの座標半径いくらかを取得してポジションを決める
            Vector3 spawnOffset;
            do
            {
                var randomCircle = Random.insideUnitCircle * spawnRadius;
                spawnOffset = new Vector3(randomCircle.x, randomCircle.y, 0);
            } 
            while (spawnOffset.magnitude < exclusionRadius);
            
            // 最終的なスポーン位置
            var spawnPosition = ufoPosition + spawnOffset;
            
            // 画面外の座標に変換
            return  _camera.ViewportToWorldPoint(
                new Vector3(spawnPosition.x, spawnPosition.y, _camera.nearClipPlane)
                );;
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