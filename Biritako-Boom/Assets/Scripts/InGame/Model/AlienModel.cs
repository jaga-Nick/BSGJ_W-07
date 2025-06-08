using Cysharp.Threading.Tasks;
using InGame.Model;
using UnityEngine;

namespace InGame.Model
{
    public class AlienModel :  MonoBehaviour, IEnemyModel
    {
        // IEnemyModelの実装
    [SerializeField] private float _limitMoveDistance = 20f;
    public float LimitMoveDistance => _limitMoveDistance;
    
    [SerializeField] private float _explosionPower = 5f;
    public float ExplosionPower => _explosionPower;

    public Rigidbody2D Rb { get; private set; }
    public float CurrentTime { get; set; }
    public float IntervalTime { get; set; }
    public Vector3 Angle { get; set; }
    
    private AlienManager _manager;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// AlienManagerによって呼び出され、初期化を行う
    /// </summary>
    public void Initialize(AlienManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// アクティブになった時に状態をリセットする
    /// </summary>
    public void ResetState()
    {
        CurrentTime = 0f;
        // 最初の動きのインターバルを即時〜短めに設定
        IntervalTime = Random.Range(0f, 1.5f); 
        this.transform.localScale = Vector3.one; // 必要であればサイズなどもリセット
    }

    /// <summary>
    /// IEnemyModelで定義されたデフォルトの移動処理。
    /// このメソッドはAlienManagerのUpdateから呼び出される。
    /// </summary>
    public void Move()
    {
        CurrentTime += Time.deltaTime;

        if (CurrentTime >= IntervalTime)
        {
            CurrentTime = 0f; // タイマーリセット

            // 新しい角度を決定
            int num = Random.Range(0, 360);
            float rad = Mathf.Deg2Rad * num;
            Angle = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            
            // 次の行動までの時間を設定
            IntervalTime = Random.Range(1f, 5f);
        }
        
        // 速度を直接設定
        Rb.linearVelocity = Angle;
    }

    /// <summary>
    /// 自分自身をプールに戻すメソッド
    /// </summary>
    public void ReturnToPool()
    {
        // Managerがnullでなければプールに戻す
        _manager?.ReturnAlien(this);
    }

    public async UniTask TakeDamage()
    {
        
    }

    // 例：プレイヤーの弾に当たった時などに呼ばれることを想定
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            // ここに被弾処理（エフェクト生成など）
            
            // 自分をプールに戻す
            ReturnToPool();
        }
    }
    }
}

