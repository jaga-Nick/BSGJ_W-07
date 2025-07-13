using System;
using System.Collections;
using System.Threading;
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
        [Header("家電全員の可動範囲")]
        [SerializeField] private Rect electronicsSpawnRate = new Rect(-32f, -32f, 64f, 64f);
        
        /// <summary>
        /// modelとview
        /// </summary>
        private ElectronicsModel _model;
        private ElectronicsView _view;
        private UfoModel _ufoModel;

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
        /// 家電の自動運転
        /// </summary>
        /// <returns></returns>
        private async UniTask AutoMoveElectronicsRoutine()
        {
            var cts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, this.GetCancellationTokenOnDestroy());
            var linkedToken = linkedCts.Token;
            try
            {
                while (true)
                {
                    // キャンセル要求があったら、例外を投げて処理を中断
                    linkedToken.ThrowIfCancellationRequested();

                    // 目的座標をランダムに決める
                    var randomCircle = Random.insideUnitCircle * radius;
                    var target = new Vector3(
                        transform.position.x + randomCircle.x,
                        transform.position.y + randomCircle.y,
                        transform.position.z
                    );
                    
                    // マップ外に出そうになったら止まる
                    target.x = Mathf.Clamp(target.x, electronicsSpawnRate.xMin, electronicsSpawnRate.xMax);
                    target.y = Mathf.Clamp(target.y, electronicsSpawnRate.yMin, electronicsSpawnRate.yMax);

                    // 移動アニメーションの開始
                    _view.PlayMoveAnimation(true);
                    // 移動コルーチン
                    await MoveElectronicsRoutine(target);
                    // 一定時間待機
                    await UniTask.Delay(TimeSpan.FromSeconds(stopTime), cancellationToken: linkedToken);
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた時の処理
            }
        }

        /// <summary>
        /// 家電の移動コルーチン
        /// </summary>
        /// <returns></returns>
        private async UniTask MoveElectronicsRoutine(Vector2 target)
        {
            var cts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, this.GetCancellationTokenOnDestroy());
            var linkedToken = linkedCts.Token;
            try
            {
                // 家電の移動処理
                while (Vector2.Distance(transform.position, target) > 0.1f)
                {
                    // キャンセル要求があったら、例外を投げて処理を中断
                    linkedToken.ThrowIfCancellationRequested();
                    
                    transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _model.Speed);
                    _model.Position = transform.position;
                    await UniTask.Yield(linkedToken);
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた時の処理
            }
        }
    }
}