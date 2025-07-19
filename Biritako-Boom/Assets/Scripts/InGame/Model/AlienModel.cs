using UnityEngine;
using InGame.Model;
using InGame.View;
using InGame.NonMVP;
using Cysharp.Threading.Tasks;
using InGame.Presenter;
using System;
using Random = UnityEngine.Random;

namespace InGame.Model
{
    /// <summary>
    /// エイリアンのデータと振る舞いのロジックを持つコンポーネント。
    /// IEnemyModelインターフェースを実装し、ステートマシンによるAIで動作する。
    /// </summary>
    public class AlienModel : MonoBehaviour, IEnemyModel
    {
        /// <summary>
        /// エイリアンの行動状態を定義する
        /// </summary>
        private enum AlienState
        {
            Waiting,   // 待機状態。一定時間後に移動へ移行
            Moving,    // 通常移動状態。ランダムな方向へ移動
            Searching, // プレイヤー探索状態
            Chasing,   // プレイヤー追跡状態
            Attacking  // 攻撃状態。プレイヤーの妨害を試みる
        }
        
        // --- AIパラメータ（privateなフィールド） ---
        private float _moveSpeed;   // 通常時の移動速度
        private float _chaseSpeed;  // プレイヤー追跡時の移動速度
        private float _searchRadius;// プレイヤーを探す範囲
        private float _attackRadius;// 攻撃を開始できる範囲

        // --- 状態管理 ---
        private AlienState _currentState = AlienState.Waiting; // エイリアンの現在の行動状態
        private float _stateTimer = 0f;                        // 各状態の経過時間を計るタイマー
        private Transform _playerTransform;                    // 発見したプレイヤーのTransform
        private readonly ComponentChecker _checker = new ComponentChecker(); // 周囲のオブジェクトを探索するユーティリティ

        // --- IEnemyModel/既存のプロパティ ---
        public float LimitMoveDistance { get; private set; } = 20f;
        public float ExplosionPower { get; private set; } = 5f;
        public Rigidbody2D Rb { get; private set; }
        public Vector3 Angle { get; set; }
        int IEnemyModel.CurrentHp { get; set; }
        private const int _deadScore = 20;
        private bool _isRightFlip = true; // trueなら右向き、falseなら左向き
        
        // UFO挙動用のプロパティ（このクラスでは未使用）
        public float CurrentTime { get; set; } 
        public float IntervalTime { get; set; }

        /// <summary>
        /// ダメージを受けた時に発行されるイベント
        /// </summary>
        public event Action OnDamaged;
        /// <summary>
        /// HPが0になり、プールに返却されるべき時に発行されるイベント
        /// </summary>
        public event Action OnReturnedToPool;

        #region アクセサメソッドと初期化
        
        /// <summary>
        /// このモデルが使用するRigidbody2Dコンポーネントを設定する
        /// </summary>
        /// <param name="rigidbody">アタッチされているRigidbody2D</param>
        public void SetRigidbody(Rigidbody2D rigidbody) => this.Rb = rigidbody;

        /// <summary>
        /// 現在のHPを取得する
        /// </summary>
        public int GetHp() => ((IEnemyModel)this).CurrentHp;

        /// <summary>
        /// キャラクターが右を向いているかどうかの状態を取得する
        /// </summary>
        public bool GetFlip() => _isRightFlip;
        
        /// <summary>
        /// エイリアンがプールから取り出される際に呼ばれ、状態を初期化する
        /// </summary>
        /// <param name="initialHp">初期HP</param>
        public void SetInitialState(int initialHp)
        {
            // HPを設定
            ((IEnemyModel)this).CurrentHp = initialHp;
            
            // AIパラメータを固定値で初期化
            this._moveSpeed = 2f;
            this._chaseSpeed = 4f;
            this._searchRadius = 8f;
            this._attackRadius = 2.5f;

            // 初期状態を「待機」から開始
            ChangeState(AlienState.Waiting);
        }
        
        /// <summary>
        /// 敵の種類を返す (IEnemyModelインターフェースの実装)
        /// </summary>
        /// <returns>0: 未定義, 1: 家電, 2: エイリアンなど</returns>
        public virtual int GetEnemyType()
        {
            // このモデルはエイリアンなので0以外の固有値を返す（例: 2）
            return 2;
        }

        #endregion

        #region ダメージ/死亡処理

        /// <summary>
        /// ダメージを受け、HPを減らす処理
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void OnDamage(int damage)
        {
            ((IEnemyModel)this).CurrentHp -= damage;
            // HPがまだ残っている場合
            if (((IEnemyModel)this).CurrentHp > 0)
            {
                OnDamaged?.Invoke();
            }
            // HPが0以下になったら死亡処理へ
            else
            {

                Debug.Log("死亡した");
                // プールに戻るべきことを外部に通知（イベント発行）
                OnDead().Forget();
            }
        }
        
        /// <summary>
        /// 死亡時の処理。スコアを加算し、プール返却イベントを発行する
        /// </summary>
        public UniTask OnDead()
        {
            // シングルトンのスコアモデルにアクセスしてスコアを加算
            ScoreModel.Instance().IncrementScore(_deadScore);
            // Presenterにプールへ戻るよう通知
            OnReturnedToPool?.Invoke();
            return UniTask.CompletedTask;
        }
        #endregion

        /// <summary>
        /// AIのメインロジック。現在の状態に応じて処理を分岐させるステートマシン。
        /// AlienManagerから毎フレーム呼び出される。
        /// </summary>
        public virtual void Move()
        {
            // Rigidbodyがなければ処理を中断
            if (Rb == null) return;

            // 状態ごとのタイマーを進める
            _stateTimer += Time.deltaTime;

            // 現在の状態に応じて行動を切り替える
            switch (_currentState)
            {
                // 「待機」状態の処理
                case AlienState.Waiting:
                    // 3秒経過したら「移動」状態へ移行
                    if (_stateTimer >= 3.0f) ChangeState(AlienState.Moving);
                    break;

                // 「移動」状態の処理
                case AlienState.Moving:
                    // 3秒経過したら「探索」状態へ移行
                    if (_stateTimer >= 3.0f) ChangeState(AlienState.Searching);
                    break;

                // 「探索」状態の処理
                case AlienState.Searching:
                    // この状態は一瞬で終わるため、タイマー判定は不要
                    SearchForPlayer();
                    break;

                // 「追跡」状態の処理
                case AlienState.Chasing:
                    // 5秒経過するか、追跡対象を見失ったら「攻撃」フェーズへ
                    if (_stateTimer >= 5.0f || _playerTransform == null)
                    {
                        ChangeState(AlienState.Attacking);
                        break; // 即座に次の状態へ移るためbreak
                    }
                    
                    // プレイヤーとの距離を計算
                    float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

                    // 攻撃範囲の外にいる場合のみ、プレイヤーに向かって移動する
                    if (distanceToPlayer > _attackRadius)
                    {
                        // プレイヤーへの方向ベクトルを計算して移動
                        Vector3 chaseDirection = (_playerTransform.position - transform.position).normalized;
                        Rb.linearVelocity = chaseDirection * _chaseSpeed;
                        JudgeFlip(chaseDirection.x);
                    }
                    // 攻撃範囲内に入ったら、物理衝突を避けるために移動を停止する
                    else
                    {
                        Rb.linearVelocity = Vector2.zero;
                        // 停止してもプレイヤーの方向は向き続ける
                        float horizontalDiff = _playerTransform.position.x - transform.position.x;
                        JudgeFlip(horizontalDiff);
                    }
                    break;
                
                // 「攻撃」状態の処理
                case AlienState.Attacking:
                    // この状態も一瞬で終わる
                    AttackPlayer();
                    break;
            }
        }

        /// <summary>
        /// AIの状態を安全に切り替え、タイマーをリセットするヘルパーメソッド
        /// </summary>
        /// <param name="newState">次に移行する新しい状態</param>
        private void ChangeState(AlienState newState)
        {
            _currentState = newState;
            _stateTimer = 0f; // 状態を切り替えたらタイマーを0にリセット

            // 新しい状態に応じた初期設定を行う
            switch (newState)
            {
                case AlienState.Waiting:
                    Rb.linearVelocity = Vector2.zero; // 待機状態では移動を止める
                    break;
                case AlienState.Moving:
                    // ランダムな角度を計算し、その方向へ移動を開始
                    float randomAngle = Random.Range(0, 360);
                    float rad = Mathf.Deg2Rad * randomAngle;
                    Angle = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
                    Rb.linearVelocity = Angle * _moveSpeed;
                    JudgeFlip(Angle.x); // 移動方向に応じて向きを変える
                    break;
                case AlienState.Searching:
                case AlienState.Attacking:
                    // 探索や攻撃の直前には、一旦停止する
                    Rb.linearVelocity = Vector2.zero;
                    break;
            }
        }

        /// <summary>
        /// 索敵範囲内のプレイヤーを探索する
        /// </summary>
        private void SearchForPlayer()
        {
            //索敵範囲内にPlayerAttachを持つオブジェクトがいるかチェック
            var player = _checker.CharacterCheck<PlayerAttach>(transform.position, _searchRadius);
            
            // プレイヤーを発見した場合
            if (player != null)
            {
                _playerTransform = player.transform; // 追跡対象として保存
                ChangeState(AlienState.Chasing);    // 「追跡」状態へ
            }
            // プレイヤーを発見できなかった場合
            else
            {
                _playerTransform = null;            // 追跡対象をリセット
                ChangeState(AlienState.Waiting);    // 「待機」状態へ戻る
            }
        }
        
        /// <summary>
        /// プレイヤーへの攻撃（妨害）を試みる
        /// </summary>
        private void AttackPlayer()
        {
            // 追跡対象がいて、かつ攻撃範囲内にいるか最終確認
            if (_playerTransform != null && Vector2.Distance(transform.position, _playerTransform.position) <= _attackRadius)
            {
                // シーン内のPlayerPresenterを探してPlayerModelを取得
                PlayerPresenter playerPresenter = FindObjectOfType<PlayerPresenter>();
                if (playerPresenter != null)
                {
                    var playerModel = playerPresenter.Model;
                    // プレイヤーがコードを持っている（妨害できる）場合のみPutCodeを実行
                    if (playerModel.CurrentHaveCodeSimulator != null)
                    {
                        playerModel.PutOnCode();
                        var effect = GetComponent<EnemyEffect>();
                        effect.GenerateEffect();
                    }
                }
            }
            // 攻撃の成否にかかわらず、「待機」状態へ移行してクールダウンに入る
            ChangeState(AlienState.Waiting);
        }

        /// <summary>
        /// X軸の移動方向に応じて、キャラクターの左右の向きを決定する
        /// </summary>
        /// <param name="directionX">X軸の移動方向ベクトルまたは位置の差分</param>
        private void JudgeFlip(float directionX)
        {
            // フリップのしきい値。これより小さい動きでは向きを変えず、振動を防ぐ
            const float flipThreshold = 0.1f;

            // しきい値より大きく右に動いていれば右向き
            if (directionX > flipThreshold) _isRightFlip = true;
            // しきい値より大きく左に動いていれば左向き
            else if (directionX < -flipThreshold) _isRightFlip = false;
            // しきい値の範囲内では、現在の向きを維持する
        }
    }
}