using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using InGame.Model;
using InGame.View;
using System.Threading;
using System;
using Random = UnityEngine.Random;

namespace InGame.Presenter
{
    public class UfoPresenter : MonoBehaviour
    {
        /// <summary>
        /// 移動パラメータ
        /// </summary>
        [Header("移動パラメータ")]
        [Header("移動速度")]
        [SerializeField] private float moveSpeed = 2.0f;
        [Header("移動範囲")]
        [SerializeField] private float radius = 10f;
        [Header("停止時間")]
        [SerializeField] private float stopTime = 2.0f;
        
        /// <summary>
        /// 浮遊パラメータ
        /// </summary>
        [Header("浮遊パラメータ")]
        [Header("浮遊の時間")]
        [SerializeField] private float frequencyTime = 2.0f;
        [Header("浮遊の振幅")]
        [SerializeField] private float frequencyAmplitude = 0.01f;
        [Header("浮遊の速さ")]
        [SerializeField] private float frequencySpeed = 0.5f;
        
        /// <summary>
        /// UFOのステータス
        /// </summary>
        [Header("UFOのステータス")]
        [Header("UFOの最大HP")]
        [SerializeField] private int maxUfoHp = 100;
        [Header("UFOのスコア")]
        [SerializeField] private int ufoScore = 100;
        
        /// <summary>
        /// modelとview
        /// </summary>
        private UfoModel _model;
        private UfoView _view;

        private CancellationTokenSource AutoMoveCancel;
        private CancellationTokenSource MoveCancel;
        
        private void Start()
        {
            _model = gameObject.GetComponent<UfoModel>();
            _model.Rb = gameObject.GetComponent<Rigidbody2D>();
            _model.Speed = moveSpeed;
            _model.Position = transform.position;
            _model.MaxUfoHp = maxUfoHp;
            //CurrentUfoHp = maxUfoHp,
            _model.UfoScore = ufoScore;
            _view = gameObject.GetComponent<UfoView>();
            
            AutoMoveUfoRoutine().Forget();
        }
        
        /// <summary>
        /// UFOの自動運転
        /// </summary>
        /// <returns></returns>
        private async UniTask AutoMoveUfoRoutine()
        {
            AutoMoveCancel?.Cancel();
            AutoMoveCancel?.Dispose();
            AutoMoveCancel = new CancellationTokenSource();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(AutoMoveCancel.Token, destroyCancellationToken);
            var linkedToken = linked.Token;
            try
            {
                while (true)
                {
                    linkedToken.ThrowIfCancellationRequested();

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
                    await MoveUfoRoutine(target);
                    // 時間待機
                    await UniTask.Delay(TimeSpan.FromSeconds(stopTime), cancellationToken: linkedToken);
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                AutoMoveCancel = null;
            }
        }

        /// <summary>
        /// UFOの移動コルーチン
        /// </summary>
        /// <returns></returns>
        private async UniTask MoveUfoRoutine(Vector2 target)
        {
            MoveCancel?.Cancel();
            MoveCancel?.Dispose();
            MoveCancel = new CancellationTokenSource();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(MoveCancel.Token, destroyCancellationToken);
            var linkedToken = linked.Token;
            try
            {
                // UFOの移動処理
                while (Vector2.Distance(transform.position, target) > 0.1f)
                {
                    linkedToken.ThrowIfCancellationRequested();

                    transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _model.Speed);
                    _model.Position = transform.position;
                    await UniTask.Yield(linkedToken);
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                MoveCancel = null;
            }
        }

       
    }
}
