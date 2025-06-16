using System;
using System.Collections;
using Cysharp.Threading.Tasks;
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
        [Header("移動パラメータ")]
        [Header("移動速度")]
        [SerializeField] private float moveSpeed = 3.0f;
        [Header("前進する秒数")]
        [SerializeField] private float moveDuration = 2.0f;
        [Header("移動範囲")]
        [SerializeField] private float radius = 10f;
        [Header("停止時間")]
        [SerializeField] private float stopTime = 1.0f;
        
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
            // modelにセット
            _model = gameObject.GetComponent<ElectronicsModel>();
            _model.Initialize(gameObject.GetComponent<Rigidbody2D>(),transform.position);
            _model.Speed = moveSpeed;
            
            // viewにセット
            _view = gameObject.GetComponent<ElectronicsView>();

            AutoMoveElectronicsRoutine().Forget();
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
            return spawnPosition;
        }
        
        
        /// <summary>
        /// 家電の自動運転
        /// </summary>
        /// <returns></returns>
        private async UniTask AutoMoveElectronicsRoutine()
        {
            while (true)
            {
                // 目的座標をランダムに決める
                var randomCircle = Random.insideUnitCircle * radius;
                var target = new Vector3(
                    transform.position.x + randomCircle.x, 
                    transform.position.y + randomCircle.y, 
                    transform.position.z
                );
            
                // 移動アニメーションの開始
                _view.PlayMoveAnimation(true);
                // 移動コルーチン
                await MoveElectronicsRoutine(target);
                // 一定時間待機
                await UniTask.WaitForSeconds(stopTime);
            }
        }

        /// <summary>
        /// 家電の移動コルーチン
        /// </summary>
        /// <returns></returns>
        private async UniTask MoveElectronicsRoutine(Vector2 target)
        {
            // 家電の移動処理
            while (Vector2.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _model.Speed);
                _model.Position = transform.position;
                await UniTask.Yield();
            }
        }
    }
}