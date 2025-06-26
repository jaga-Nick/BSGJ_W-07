using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// エイリアンのデータと振る舞いのロジックを持つコンポーネント。
    /// </summary>
    public class AlienModel : MonoBehaviour, IEnemyModel
    {
        // --- 状態定義 ---
        /// <summary>
        /// エイリアンの行動状態
        /// </summary>
        private enum AlienState
        {
            Waiting, // 待機中
            Moving   // 移動中
        }
        

        // 移動できる限界距離
        public float LimitMoveDistance { get; private set; } = 20f;
        // 爆発力
        public float ExplosionPower { get; private set; } = 5f;
        // 物理的な移動を担うRigidbody2Dへの参照 (private setで外部からの直接設定を防ぐ)
        public Rigidbody2D Rb { get; private set; }
        // 経過時間を記録するタイマー
        public float CurrentTime { get; set; }
        // 次の行動までの間隔時間
        public float IntervalTime { get; set; }
        // 現在の移動方向
        public Vector3 Angle { get; set; }

        // --- IEnemyModel以外の変数とイベント ---

        // HPはprivate変数として保持
        int IEnemyModel.CurrentHp { get; set; }

        
        // 現在の状態を保持する変数
        private AlienState _currentState;
        // 状態を切り替えるためのタイマー
        private float _stateTimer;
        // 状態を切り替える間隔（秒）
        private const float STATE_CHANGE_INTERVAL = 3.0f;

        private bool _isRightFlip = true;
        

        /// <summary>
        /// ダメージを受けた時に発行されるイベント
        /// </summary>
        public event Action OnDamaged;
        
        /// <summary>
        /// プールに戻るべき時に発行されるイベント
        /// </summary>
        public event Action OnReturnedToPool;

        #region アクセサメソッド

        /// <summary>
        /// このModelが使用するRigidbody2Dを設定します（外部から注入）。
        /// </summary>
        public void SetRigidbody(Rigidbody2D rigidbody)
        {
            
            this.Rb = rigidbody;
        }

        /// <summary>
        /// 現在のHPを取得します。
        /// </summary>
        public int GetHp()
        {
            // privateなHPの値を返す
            return ((IEnemyModel)this).CurrentHp;
        }
        
        /// <summary>
        /// エイリアンの状態を初期化またはリセットする。
        /// </summary>
        public void SetInitialState(int initialHp)
        {
            // privateなHPの値を設定
            ((IEnemyModel)this).CurrentHp = initialHp;
            // タイマーをリセット
            CurrentTime = 0f;
            // 最初の行動までの時間をランダムに設定
            IntervalTime = UnityEngine.Random.Range(0f, 1.5f);
        }

        public bool GetFlip()
        {
            return _isRightFlip;
        }
        
        
        #endregion

        /// <summary>
        /// ダメージを受ける処理。
        /// </summary>
        public void OnDamage(int damage)
        {
            // HPを減算
            ((IEnemyModel)this).CurrentHp -= damage;
            
            // HPがまだ残っている場合
            if (((IEnemyModel)this).CurrentHp > 0)
            {
                // ダメージを受けたことを外部に通知（イベント発行）
                OnDamaged?.Invoke();
            }
            else // HPが0以下になった場合
            {
                // プールに戻るべきことを外部に通知（イベント発行）
                OnDead().Forget();
            }
        }
        public async UniTask OnDead()
        {
            OnReturnedToPool?.Invoke();
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void Move()
        {
            if (Rb == null) return;

            // 状態タイマーを経過時間分だけ進める
            _stateTimer += Time.deltaTime;
            
            // タイマーが設定した間隔（3秒）を超えたら
            if (_stateTimer >= STATE_CHANGE_INTERVAL)
            {
                // タイマーをリセット
                _stateTimer = 0f;

                // 現在の状態に応じて、次の状態に切り替える
                if (_currentState == AlienState.Waiting)
                {
                    // 現在「待機中」なら「移動中」へ
                    _currentState = AlienState.Moving;

                    // 新しいランダムな移動方向を計算
                    int num = UnityEngine.Random.Range(0, 360);
                    JudgeFlip(num);
                    float rad = Mathf.Deg2Rad * num;
                    Angle = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

                    // 計算した方向に移動を開始
                    Rb.linearVelocity = Angle;
                }
                else // 現在「移動中」なら
                {
                    // 「待機中」へ
                    _currentState = AlienState.Waiting;

                    // 移動を停止
                    Rb.linearVelocity = Vector2.zero;
                }
            }
        }

        #region 非公開メソッド

        private void JudgeFlip(int rad)
        {
            if (rad  <= 90 || 270 < rad)
            {
                _isRightFlip = true;
            }
            else
            {
                _isRightFlip = false;
            }
        }

        #endregion
    }
}