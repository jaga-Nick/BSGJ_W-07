using Cysharp.Threading.Tasks;
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
        [Header("UFOが一度に動く移動範囲")]
        [SerializeField] private float radius = 10f;
        [Header("停止時間")]
        [SerializeField] private float stopTime = 2.0f;
        [Header("UFO全員の可動範囲")]
        [SerializeField] private Rect ufoSpawnRate = new Rect(-32f, -32f, 64f, 64f);
        
        /// <summary>
        /// UFOのステータス
        /// </summary>
        [Header("UFOのステータス")]
        [Header("UFOの最大HP")]
        [SerializeField] private int maxUfoHp = 100;
        [Header("UFOにダメージを与えた時のスコア")]
        [SerializeField] private int ufoScore = 100;
        [Header("UFOが倒された時のスコア")]
        [SerializeField] private int ufoDeadScore = 100;
        
        /// <summary>
        /// modelとview
        /// </summary>
        private UfoModel _model;
        private UfoView _view;

        private CancellationTokenSource _autoMoveCancel;
        private CancellationTokenSource _moveCancel;
        
        private void Start()
        {
            // パラメタのセット
            _model = gameObject.GetComponent<UfoModel>();
            _model.Rb = gameObject.GetComponent<Rigidbody2D>();
            _model.Speed = moveSpeed;
            _model.Position = transform.position;
            _model.MaxUfoHp = maxUfoHp;
            _model.UfoScore = ufoScore;
            _model.UfoDeadScore = ufoDeadScore;
            _view = gameObject.GetComponent<UfoView>();
            
            // UFOの自動運転スタート
            AutoMoveUfoRoutine().Forget();
        }
        
        /// <summary>
        /// UFOの自動運転
        /// </summary>
        /// <returns></returns>
        private async UniTask AutoMoveUfoRoutine()
        {
            _autoMoveCancel?.Cancel();
            _autoMoveCancel?.Dispose();
            _autoMoveCancel = new CancellationTokenSource();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(_autoMoveCancel.Token, destroyCancellationToken);
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
                    
                    // マップ外に出そうになったら止まる
                    target.x = Mathf.Clamp(target.x, ufoSpawnRate.xMin, ufoSpawnRate.xMax);
                    target.y = Mathf.Clamp(target.y, ufoSpawnRate.yMin, ufoSpawnRate.yMax);

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
                // キャンセルされたときの処理
            }
            finally
            {
                _autoMoveCancel = null;
            }
        }

        /// <summary>
        /// UFOの移動コルーチン
        /// </summary>
        /// <returns></returns>
        private async UniTask MoveUfoRoutine(Vector2 target)
        {
            _moveCancel?.Cancel();
            _moveCancel?.Dispose();
            _moveCancel = new CancellationTokenSource();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(_moveCancel.Token, destroyCancellationToken);
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
                // キャンセルされた時の処理
            }
            finally
            {
                _moveCancel = null;
            }
        }

        // TODO: ダメージを受けた時はPresenterを介す
        /// <summary>
        /// Ufoがダメージを受けた時の処理
        /// ModelとViewに反映させる
        /// </summary>
        public void StopAllTasks()
        {
            // キャンセル処理
            _autoMoveCancel?.Cancel();
            _autoMoveCancel?.Dispose();
            _autoMoveCancel = null;
            
            _moveCancel?.Cancel();
            _moveCancel?.Dispose();
            _moveCancel = null;

            // 移動アニメーションの開始
            // _view.PlayMoveAnimation(false);
            // 死亡アニメーションの開始
            // _view.PlayDeadAnimation();
        }
    }
}
