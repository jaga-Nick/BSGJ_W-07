using System;
using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// エイリアンのデータと振る舞いのロジックを持つコンポーネント。
    /// 自身ではコンポーネント取得を行わず、外部から設定されるのを待つ。
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
        
        
        // --- IEnemyModelの実装 ---

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
        private int _hp;
        
        
        // 現在の状態を保持する変数
        private AlienState _currentState;
        // 状態を切り替えるためのタイマー
        private float _stateTimer;
        // 状態を切り替える間隔（秒）
        private const float STATE_CHANGE_INTERVAL = 3.0f;
        

        /// <summary>
        /// ダメージを受けた時に発行されるイベント
        /// </summary>
        public event Action OnDamaged;
        
        /// <summary>
        /// プールに戻るべき時に発行されるイベント
        /// </summary>
        public event Action OnReturnedToPool;

        // --- アクセサメソッド ---

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
            return _hp;
        }
        
        /// <summary>
        /// エイリアンの状態を初期化またはリセットする。
        /// </summary>
        public void SetInitialState(int initialHp)
        {
            // privateなHPの値を設定
            _hp = initialHp;
            // タイマーをリセット
            CurrentTime = 0f;
            // 最初の行動までの時間をランダムに設定
            IntervalTime = UnityEngine.Random.Range(0f, 1.5f);
        }

        /// <summary>
        /// ダメージを受ける処理。
        /// </summary>
        public void TakeDamage(int damage)
        {
            // HPを減算
            _hp -= damage;
            
            // HPがまだ残っている場合
            if (_hp > 0)
            {
                // ダメージを受けたことを外部に通知（イベント発行）
                OnDamaged?.Invoke();
            }
            else // HPが0以下になった場合
            {
                // プールに戻るべきことを外部に通知（イベント発行）
                OnReturnedToPool?.Invoke();
            }
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void Move()
        {
            // Rigidbodyが設定されていなければ何もしない（安全対策）
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
    }
}