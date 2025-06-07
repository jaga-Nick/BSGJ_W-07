using Common;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InGame.Model
{
    /// <summary>
    /// 母艦UFOのモデル管理。
    /// </summary>
    public class MotherShipModel : IEnemyModel
    {
        #region 定数
        // この距離までターゲットに近づいたら、目的地に到着したとみなすための閾値
        private const float DESTINATION_THRESHOLD = 0.5f;

        #endregion

        #region 状態定義

        /// <summary>
        /// 母艦の現在の行動状態を定義します
        /// </summary>
        private enum State
        {
            Idle,           // 待機中
            Patrolling,     // 雑魚UFOを巡回中
            MovingToCenter, // マップ中央へ移動中
            Stopped         // 停止
        }

        #endregion

        #region プロパティ
        
        /// <summary>
        /// インターフェイスプロパティ
        /// </summary>
        // 移動許容距離
        public float LimitMoveDistance { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        // 爆発力
        public float ExplosionPower { get; private set; }
        
        public float CurrentTime { get; set; }
        public float IntervalTime { get; set; } = 0.2f;
        public Vector3 Angle { get; set; }

        /// <summary>
        /// 母艦固有プロパティ
        /// </summary>
        private GameObject motherShipObject;
        
        // 母艦のHP
        private int _hp;
        
        // 破壊されたときに得られるスコア
        private int _score { get; } = 1000000;
        
        // 母艦の移動速度
        private float _speed;

        private bool isEnd = false;
        
        private bool isRandomPatrol { get; set; } = true;

        #endregion
        
        #region プライベート変数
        
        // 制御対象のTransformコンポーネント。自身の位置を知るために使用します。
        private Transform _transform;
        
        // 母艦の現在の状態
        private State _currentState = State.Idle;
        
        // 巡回対象となる雑魚UFOのTransformリスト
        private List<GameObject> _ufoTargets = new List<GameObject>();
        
        // 現在の巡回ターゲットのインデックス
        private int _currentTargetIndex = 0;

        #endregion

        #region 

        /// <summary>
        /// MotherShipUfoModelのコンストラクタ
        /// </summary>
        public MotherShipModel(Rigidbody2D rb)
        {
            // --- ステータスを初期化 ---
            IntervalTime = 0.2f;
            
            _speed = 5.0f;
            _hp = 5000;
            ExplosionPower = 100;
            _score = 1000000;
            isEnd = false;
        }

        #endregion
        
        #region アクセスメソッド
        
        public void SetRb(Rigidbody2D rb)
        {
            Rb = rb;
            _transform = rb.transform;
        }
        
        public void SetRandomPatrol(bool mode){ isRandomPatrol  = mode; }
        
        public void SetSpeed(float speed) { _speed = speed; }
        
        
        
        
        
        
        /// <summary>
        /// 撃破状態から呼び出され、isEndフラグを立てます。
        /// </summary>
        public void DefeatNotification()
        {
            Debug.Log("第三部　完!!");
            
            isEnd = true;
        }
        
        #endregion
        
        #region 生成メソッド
        
        /// <summary>
        /// Addressablesを使用して母艦UFOを非同期で生成
        /// </summary>
        public async UniTask<GameObject> GenerateMotherShip(string address, Vector3 position, CancellationToken cancellationToken)
        {
            // Addressables経由でプレハブを非同期ロード
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);

            using (new HandleDisposable<GameObject>(handle))
            {
                GameObject prefab = await handle;
                // ロードしたプレハブからGameObjectをインスタンス化
                GameObject instance = UnityEngine.Object.Instantiate(prefab,position,Quaternion.identity);
                // 生成したインスタンスからコンポーネントを取得し、自身のフィールドに保持
                Rb = instance.GetComponent<Rigidbody2D>();
                return instance;
            }
        }

        #endregion
        
        
        
        #region 公開メソッド

        /// <summary>
        /// ダメージを与える
        /// </summary>
        public async UniTask TakeDamage(int damage)
        {
            _hp -= damage;
            if (_hp <= 0)
            {
                DefeatNotification();
            }
        }

        /// <summary>
        /// シーン内に存在する全てのモブUFOを探し出し、巡回ターゲットのリストとして設定します。
        /// </summary>
        public void FindTargets()
        {
            test[] ufo = Object.FindObjectsOfType<test>();
            
            _ufoTargets.Clear(); 
    
            // 新しいリストを作成して、見つけたUFOのGameObjectを格納していきます
            foreach (var presenter in ufo)
            {
                // PresenterがアタッチされているGameObjectをリストに追加
                _ufoTargets.Add(presenter.gameObject);
            }
        }

        /// <summary>
        /// 巡回を開始します
        /// </summary>
        public void StartPatrol()
        {
            _currentTargetIndex = 0; // 最初のターゲットから開始

            // 雑魚UFOが1体以上いれば巡回を開始し、いなければ中央への移動を開始します
            _currentState = _ufoTargets.Count > 0 ? State.Patrolling : State.MovingToCenter;
        }
        
        
        /// <summary>
        /// PresenterのUpdateから毎フレーム呼び出される更新処理です。
        /// </summary>
        public void Move()
        {
            
            // 状態に応じて処理を分岐させます
            switch (_currentState)
            {
                case State.Patrolling:
                    Patrolling();
                    break;
                case State.MovingToCenter:
                    MoveCenter();
                    break;
            }
            
            /*
            
            CurrentTime += Time.deltaTime;

            if (IntervalTime >= CurrentTime)
            {
                // 状態に応じて処理を分岐させます
                switch (_currentState)
                {
                    case State.Patrolling:
                        Patrolling();
                        break;
                    case State.MovingToCenter:
                        MoveCenter();
                        break;
                }
            }
            */
        }
        
        /// <summary>
        /// 母艦を破壊する、
        /// </summary>
        public void DestroyUfo(GameObject MothershipInstance)
        {
            if (MothershipInstance) return;
            Addressables.ReleaseInstance(MothershipInstance); 
            MothershipInstance = null;
        }

        #endregion

        #region 内部処理メソッド

        /// <summary>
        /// 巡回中の処理
        /// </summary>
        private void Patrolling()
        {
            // リスト内の破壊されたUFO（nullになったもの）を安全に削除
            _ufoTargets.RemoveAll(target => target == null || !target.activeInHierarchy);

            // 全ての雑魚UFOが破壊された場合、中央への移動状態に遷移します
            if (_ufoTargets.Count == 0)
            {
                _currentState = State.MovingToCenter; 
                return;
            }
            
            // ターゲットインデックスがリストの範囲を超えるのを防ぐ
            if (_currentTargetIndex >= _ufoTargets.Count)
            {
                // ランダム巡回フラグがtrueの場合、リストをシャッフルする
                if (isRandomPatrol)
                {
                    RandomTargets();
                }
                
                _currentTargetIndex = 0; // 最初のターゲットに戻る（ループ）
            }

            // 現在のターゲットを取得
            Transform currentTarget = _ufoTargets[_currentTargetIndex].transform;
            
            MoveTowards(currentTarget.position);

            // ターゲットに十分に近づいたら、次のターゲットへ移行します
            if (Vector2.Distance(_transform.position, currentTarget.position) < DESTINATION_THRESHOLD)
            {
                Rb.linearVelocity = Vector2.zero;
                _currentTargetIndex++;
            }
            else
            {
                MoveTowards(currentTarget.position);
            }
                
        }

        /// <summary>
        /// マップ中央へ移動中の処理。
        /// </summary>
        private void MoveCenter()
        {
            // 中央へ移動
            MoveTowards(Vector2.zero);

            // 中央に十分に近づいたら完全に停止し、状態をStoppedにします
            if (Vector2.Distance(_transform.position, Vector2.zero) < DESTINATION_THRESHOLD)
            {
                if(Rb != null) Rb.linearVelocity = Vector2.zero;
                _currentState = State.Stopped;
            }
        }
        
        /// <summary>
        /// ターゲットポジションに向かって動かす
        /// </summary>
        private void MoveTowards(Vector2 targetPosition)
        {
            if (Rb == null || _transform == null) return;
            
            Vector2 direction = (targetPosition - (Vector2)_transform.position).normalized;

            Rb.linearVelocity = direction * _speed;
        }
        
        
        /// <summary>
        /// UFOリストをシャッフル
        /// </summary>
        private void RandomTargets()
        {
            int n = _ufoTargets.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (_ufoTargets[k], _ufoTargets[n]) = (_ufoTargets[n], _ufoTargets[k]);
            }
        }
        #endregion
    }
}