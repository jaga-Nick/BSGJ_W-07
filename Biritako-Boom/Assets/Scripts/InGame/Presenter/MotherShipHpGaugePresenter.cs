using UnityEngine;
using InGame.View;
using InGame.Model;
using InGame.NonMVP;
using Cysharp.Threading.Tasks; // UniTaskを使用するために追加
using System.Threading;     // CancellationTokenSourceを使用するために追加

namespace InGame.Presenter
{
    /// <summary>
    /// 母艦のHPゲージのModelとViewを仲介するPresenter。
    /// ダメージゲージの遅延表示にUniTaskを使用。
    /// </summary>
    public class MotherShipHpGaugePresenter : MonoBehaviour
    {
        private MotherShipHpGaugeView _view;
        private MotherShipModel _motherShipModel;

        // --- 内部で利用する変数 ---
        private int _maxHp;
        private float _lerpFillAmount;   // 現在ゲージに表示されている滑らかな量（ダメージゲージ用）
        private float _targetFillAmount;  // ゲージが目指すべき実際のHP割合（HPゲージ用）
        private bool _isLerpAllowed = true; // ダメージゲージのLerpを許可するかどうかのフラグ

        // --- UniTask関連 ---
        private CancellationTokenSource _cts; // 遅延タスクをキャンセルするためのトークンソース

        [Header("ゲージの変動設定")]
        [Tooltip("ゲージが滑らかに変化する際の速さ。数値が大きいほど速く追従します。")]
        [SerializeField] private float gaugeLerpSpeed = 3f;
        
        [Header("ダメージゲージ設定")]
        [Tooltip("ダメージゲージモードで、ゲージが減り始めるまでの遅延時間（秒）")]
        [SerializeField] private float damageGaugeDelay = 1f;

        private bool _useDamageGaugeMode; // Viewのモードを保持

        #region イベント購読

        private void OnEnable()
        {
            EnemySpawner.OnGenerateMotherShip += FindAndSetupModel; // 母艦生成のイベント購読
            MotherShipModel.OnBossHit += HandleBossHit; // 母艦のダメージイベント購読
        }

        private void OnDisable()
        {
            EnemySpawner.OnGenerateMotherShip -= FindAndSetupModel;
            MotherShipModel.OnBossHit -= HandleBossHit;

            // オブジェクトが無効になる際にCancellationTokenをキャンセルして破棄
            _cts?.Cancel();
            _cts?.Dispose();
        }

        #endregion

        private void Awake()
        {
            _view = GetComponent<MotherShipHpGaugeView>();
            // CancellationTokenSourceを初期化
            _cts = new CancellationTokenSource();
        }

        private void Start()
        {
            if (_view == null)
            {
                Debug.LogError("同じGameObjectにMotherShipHpGaugeViewが見つかりません！", this);
                enabled = false;
                return;
            }
            // Viewの初期設定を呼び出す
            _view.Initialize();


            _useDamageGaugeMode = _view.GetDamageGauge();
        }

        private void LateUpdate()
        {
            // Modelがまだ見つかっていない場合は処理しない
            if (_motherShipModel == null) return;
            
            // Lerpが許可されている場合のみ、ゲージを滑らかに動かす
            if (_isLerpAllowed && !Mathf.Approximately(_lerpFillAmount, _targetFillAmount))
            {
                // Lerpを使って、現在の表示量を目標量に近づける
                _lerpFillAmount = Mathf.Lerp(_lerpFillAmount, _targetFillAmount, Time.deltaTime * gaugeLerpSpeed);
            }
            
            // Viewに、実際のHP割合と、滑らかに変化するゲージの割合を両方渡す
            _view.UpdateView(_targetFillAmount, _lerpFillAmount);
        }
        
        /// <summary>
        /// 母艦が生成された時に呼び出されるメソッド。
        /// </summary>
        private void FindAndSetupModel()
        {
            var motherShip = FindObjectOfType<MotherShipPresenter>();
            if (motherShip != null)
            {
                _motherShipModel = motherShip.GetModel();
                _maxHp = _motherShipModel.GetMaxHp();
                int currentHp = _motherShipModel.GetCurrentHp();
                
                float initialFill = (_maxHp > 0) ? (float)currentHp / _maxHp : 0;
                _lerpFillAmount = initialFill;
                _targetFillAmount = initialFill;

                _view.UpdateView(_targetFillAmount, _lerpFillAmount);
            }
        }
        
        /// <summary>
        /// 母艦のHPが変動した時に呼び出されるイベントハンドラ。
        /// </summary>
        private void HandleBossHit(int currentHp)
        {
            if (_maxHp <= 0) return;

            // 目標値（実際のHP割合）を更新する
            _targetFillAmount = (float)currentHp / _maxHp;

            // ダメージゲージモードの場合、遅延処理を開始する
            if (_useDamageGaugeMode)
            {
                // 既に実行中の遅延タスクがあればキャンセルして、新しいタスクを開始する
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                StartDamageGaugeDelayAsync(_cts.Token).Forget();
            }
        }

        /// <summary>
        /// UniTaskを使い、指定時間後にダメージゲージのLerpを開始する非同期メソッド。
        /// </summary>
        private async UniTaskVoid StartDamageGaugeDelayAsync(CancellationToken token)
        {
            // ダメージを受けた直後はLerpを停止
            _isLerpAllowed = false;

            try
            {
                // 指定された秒数だけ待機する
                await UniTask.Delay(System.TimeSpan.FromSeconds(damageGaugeDelay), cancellationToken: token);
                
                // 遅延時間が経過したら、Lerpを許可する
                _isLerpAllowed = true;
            }
            catch (System.OperationCanceledException)
            {
                // タスクがキャンセルされた場合は何もしない（新しいダメージが入った場合など）
            }
        }
    }
}
