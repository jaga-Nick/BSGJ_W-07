using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets; // Addressablesの機能を使うために必要
using Cysharp.Threading.Tasks;
using InGame.Presenter;
using Random = UnityEngine.Random;

/// <summary>
/// エイリアンの生成、更新、オブジェクトプールを管理するクラス。
/// </summary>
public class AlienManager : MonoBehaviour
{
    [Header("プール設定")]
    [Tooltip("Addressablesに設定したエイリアンのアドレス")]
    [SerializeField] private string _characterAddress = "Enemy_Alien"; // ここでキーを指定

    [Header("エイリアン最大数")]
    [SerializeField] private int _initialPoolSize = 50;
    
    [Header("マップ左下")]
    [SerializeField] private Vector2 _spawnAreaMin　= new Vector2(-20f, -20f);
    [Header("マップ右上")]
    [SerializeField] private Vector2 _spawnAreaMax　= new Vector2(20f, 20f);
    
    // プール（待機中のエイリアン）を管理するキュー
    private readonly Queue<AlienPresenter> _pool = new Queue<AlienPresenter>();
    // 活動中のエイリアンを管理するリスト
    private readonly List<AlienPresenter> _activeAliens = new List<AlienPresenter>();

    // Addressablesからロードしたプレハブの実体
    private GameObject _alienPrefab;
    // 初期化が完了したかどうかのフラグ
    private bool _isInitialized = false;
    

    private void Start()
    {
        InitializePool();
        
        // SpawnTest().Forget();
        
    }
    
    private void Update()
    {
        for (int i = _activeAliens.Count - 1; i >= 0; i--)
        {
            var presenter = _activeAliens[i];
            presenter.Model.Move(); 
            presenter.View.FlipAlien(presenter.Model.GetFlip());
        }
        

    }

    /// <summary>
    /// オブジェクトが破棄される時に呼ばれる
    /// </summary>
    private void OnDestroy()
    {
        // Managerが破棄される際に、ロードしたAddressableアセットを解放する
        if (_alienPrefab != null)
        {
            Addressables.Release(_alienPrefab);
        }
    }

    /// <summary>
    /// 文字列キーを使用してAddressablesからプレハブをロードし、プールを非同期で初期化。
    /// </summary>
    public async UniTask InitializePool()
    {
        // 既に初期化済みなら処理を中断
        if (_isInitialized) return;

        
        // 文字列のキーを使ってAddressablesからプレハブを非同期でロード
        _alienPrefab = await Addressables.LoadAssetAsync<GameObject>(_characterAddress).ToUniTask();
        
        // 初期プールサイズ分だけ、あらかじめエイリアンを生成してプールしておく
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GeneratePoolAlien();
        }

        // 初期化完了フラグを立てる
        _isInitialized = true;
    }

    /// <summary>
    /// 指定した位置にエイリアンを一体出現させます。
    /// </summary>
    public void SpawnAlien(Vector3 position, int initialHp)
    {
        // プールが空の場合、動的に新しいエイリアンを生成して補充する
        if (_pool.Count == 0) return;
        
        // プールから待機中のPresenterを一つ取り出す
        var presenter = _pool.Dequeue();
        // Presenter（GameObject）の座標を指定された位置に設定
        presenter.transform.position = position;

        // Modelの型をキャストして状態設定メソッドを呼び出す
        ((InGame.Model.AlienModel)presenter.Model).SetInitialState(initialHp);

        // GameObjectをアクティブにして画面に表示
        presenter.gameObject.SetActive(true);
        // 活動中リストに追加
        _activeAliens.Add(presenter);
    }
    
    /// <summary>
    /// 指定されたエイリアンを非アクティブ化し、プールに戻します。
    /// </summary>
    public void ReturnAlien(AlienPresenter presenter)
    {
        // 対象がnullなら何もしない
        if (presenter == null) return;
        
        // 活動中リストから対象を削除できたら（二重返却防止）
        if (_activeAliens.Remove(presenter))
        {
            // GameObjectを非アクティブ化
            presenter.gameObject.SetActive(false);
            // プール（キュー）の末尾に戻す
            _pool.Enqueue(presenter);
        }
    }
    
    /// <summary>
    /// エイリアンのインスタンスを一体生成し、非アクティブ状態でプールに追加する内部メソッド。
    /// </summary>
    private void GeneratePoolAlien()
    {
        // ロード済みのプレハブからGameObjectをインスタンス化。親を自分に設定。
        var alienObj = Instantiate(_alienPrefab, this.transform);
        // 生成したGameObjectからPresenterコンポーネントを取得
        var presenter = alienObj.GetComponent<AlienPresenter>();
        // PresenterにManager自身を教えて初期化する
        presenter.Initialize(this);

        // 最初は非表示にしておく
        alienObj.SetActive(false);
        // プールに追加
        _pool.Enqueue(presenter);
    }

    public async UniTask SpawnTest()
    {
        // このコンポーネントが有効である限りループを続ける
        while (this.isActiveAndEnabled)
        {
            // エイリアンを一体生成する
            SpawnAlien(new Vector3(3.0f, 3.0f, 0.0f), 1);
            
            // 10秒間待機する。
            // CancellationTokenを指定することで、このオブジェクトが破棄された時に待機を安全に中断できる
            await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    public Vector3 GetRandomPosition()
    {
        // 1. マップ境界内でランダムな位置を生成
        float randomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
        float randomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
        return  new Vector3(randomX, randomY, 0f);
    }
}