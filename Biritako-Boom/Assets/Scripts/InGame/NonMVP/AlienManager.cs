using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using InGame.Model;

/// <summary>
/// エイリアンの生成・更新・破棄・オブジェクトプールを管理するクラス。
/// 全てのアクティブなエイリアンのMove()をこのクラスのUpdateで一括処理する。
/// </summary>



public class AlienManager : MonoBehaviour
{
    
    public static AlienManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private AssetReferenceGameObject _alienPrefabReference; // Addressableの参照
    [SerializeField] private int _initialPoolSize = 50;

    [Header("Runtime Info")]
    [SerializeField, Tooltip("プール内で待機中のエイリアン")]
    private Queue<AlienModel> _pool = new Queue<AlienModel>();
    
    [SerializeField, Tooltip("現在アクティブなエイリアン")]
    private List<AlienModel> _activeAliens = new List<AlienModel>();

    private GameObject _alienPrefab;
    private bool _isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Addressablesからプレハブをロードし、プールを初期化する非同期メソッド
    /// </summary>
    public async UniTask InitializeAsync()
    {
        if (_isInitialized) return;

        // 1. Addressablesからプレハブをロード
        _alienPrefab = await _alienPrefabReference.LoadAssetAsync<GameObject>().ToUniTask();

        if (_alienPrefab == null)
        {
            Debug.LogError("エイリアンのプレハブがAddressablesからロードできませんでした。");
            return;
        }

        // 2. プールを初期化
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateAndPoolAlien();
        }

        _isInitialized = true;
        Debug.Log("AlienManagerの初期化が完了しました。");
    }
    
    /// <summary>
    /// マネージャーのUpdateループ。アクティブな全エイリアンを一括で更新する。
    /// </summary>
    private void Update()
    {
        if (!_isInitialized) return;

        // 後ろからループすることで、ループ中に対象が削除されても安全に処理できる
        for (int i = _activeAliens.Count - 1; i >= 0; i--)
        {
            AlienModel alien = _activeAliens[i];
            
            // 移動処理の呼び出し
            alien.Move();
            
            // 画面外に出るなど、生存条件をチェックしてプールに戻す
            // (この距離はカメラの位置やゲームの仕様に合わせて調整)
            if (Vector3.Distance(Vector3.zero, alien.transform.position) > alien.LimitMoveDistance)
            {
                alien.ReturnToPool();
            }
        }
    }

    /// <summary>
    /// 指定した位置にエイリアンを一体スポーンさせる
    /// </summary>
    /// <param name="position">スポーンさせる座標</param>
    /// <returns>スポーンさせたエイリアンのインスタンス。失敗時はnull</returns>
    public AlienModel SpawnAlien(Vector3 position)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("AlienManagerが初期化されていません。");
            return null;
        }

        // プールが空なら、動的に新しいインスタンスを生成
        if (_pool.Count == 0)
        {
            Debug.LogWarning("プールが枯渇したため、新しいエイリアンを動的に生成します。");
            CreateAndPoolAlien();
        }

        AlienModel alien = _pool.Dequeue();
        alien.transform.position = position;
        alien.ResetState(); // 状態をリセット
        alien.gameObject.SetActive(true);
        _activeAliens.Add(alien);
        
        return alien;
    }

    /// <summary>
    /// 指定されたエイリアンをプールに戻す
    /// </summary>
    /// <param name="alien">プールに戻すエイリアン</param>
    public void ReturnAlien(AlienModel alien)
    {
        if (alien == null) return;

        // Listから削除する際は、インスタンスを直接探して削除するのが安全
        if (_activeAliens.Remove(alien))
        {
            alien.gameObject.SetActive(false);
            _pool.Enqueue(alien);
        }
    }

    /// <summary>
    /// エイリアンを一体生成し、非アクティブ状態でプールに追加するヘルパーメソッド
    /// </summary>
    private void CreateAndPoolAlien()
    {
        GameObject alienObj = Instantiate(_alienPrefab, this.transform);
        AlienModel alien = alienObj.GetComponent<AlienModel>();
        alien.Initialize(this); // Managerへの参照を渡す
        alienObj.SetActive(false);
        _pool.Enqueue(alien);
    }

    private void OnDestroy()
    {
        // Addressablesのリソースを解放
        if (_alienPrefab != null)
        {
            _alienPrefabReference.ReleaseAsset();
        }
    }
    
}