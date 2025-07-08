using System.Collections.Generic;
using System.Linq;

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Common;
using InGame.Presenter;
using UnityEngine;
using System;

namespace InGame.NonMVP
{
    /// <summary>
    /// Enemyのスポナー
    /// </summary>
    public class EnemySpawner : DestroyAvailable_SingletonMonoBehaviourBase<EnemySpawner>
    {
        /// <summary>
        /// スポーンのプロパティ
        /// </summary>
        [Header("スポーンのプロパティ")]
        [Header("スポーン時間のインターバル")]
        [SerializeField] private float spawnInterval = 3f;
        [Header("タイマー")]
        [SerializeField] private float timer;
        [Header("生成される家電の上限数")]
        [SerializeField] private int maxElectronics = 50;
        [Header("一度に生成される家電の数")]
        [SerializeField] private int numberOfSpawnElectronics = 3;
        [Header("生成されるUFOの上限数")]
        [SerializeField] private int maxUfo = 14;
        [Header("UFO周辺の半径")]
        [SerializeField] private float spawnRadius = 2.0f;
        [Header("UFO直下の除外範囲")]
        [SerializeField] private float exclusionRadius = 0.5f;
        
        [Header("宇宙人スポーン時間のインターバル")]
        [SerializeField] private int alienSpawnInterval = 10;
        [Header("一度に生成される宇宙人の数")]
        [SerializeField] private int numberOfSpawnAlien = 5;
        
        /// <summary>
        /// 外部マネージャーへの参照
        /// </summary>
        private AlienManager _alienManager;

        /// <summary>
        /// Addressableアセットのキー
        /// </summary>
        [Header("母艦のアドレス")]
        [SerializeField] private string _motherShipAddress = "Enemy_MotherShip";

        public static event Action OnGenerateMotherShip;


        /// <summary>
        /// Prefab
        /// </summary>
        [Header("Prefab")]
        [Header("家電")]
        [SerializeField] private GameObject[] electronicsPrefabs;
        [Header("UFO")]
        [SerializeField] private GameObject ufoPrefabs;
        
        
        /// <summary>
        /// 生成されたUFOたちの管理
        /// </summary>
        private List<GameObject> ufosList = new List<GameObject>();
        
        
        /// <summary>
        /// 現在のEnemyの数
        /// </summary>
        public int CurrentElectronics { get; set; } = 0;
        public int CurrentUfo { get; set; } = 0;

        private void Start()
        {
            //アタッチされているのでそのままを維持する為に格納。（一つのみを想定しているので維持しない場合も放置でもよい。
            instance = this;

            // タイマーをスポーン時間のインターバルにセット
            timer = spawnInterval;
            // UFOをスポーンする
            SpawnUfo();
            // 母艦をスポーンする
            SpawnMotherShip(_motherShipAddress, new Vector3(0f, 30f, 0f), CancellationToken.None).Forget();
            
            _alienManager = FindObjectOfType<AlienManager>();
            SpawnAlien().Forget();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (!(timer <= 0) || CurrentElectronics >= maxElectronics) return;
            SpawnElectronics();
            timer = spawnInterval;
        }

        /// <summary>
        /// 家電の生成
        /// </summary>
        private void SpawnElectronics()
        {
            for (var i = 0; i < numberOfSpawnElectronics; i++)
            {
                // 家電を選択して生成する
                var randomIndex = UnityEngine.Random.Range(0, electronicsPrefabs.Length);
                var electronics = Instantiate(electronicsPrefabs[randomIndex]);
            
                // UFOの座標をランダムに取得
                var ufoRandomIndex = UnityEngine.Random.Range(0, ufosList.Count);
                var ufoPosition = ufosList[ufoRandomIndex].transform.position;
            
                // UFOがカメラ内にいるときは対象から外す
                var viewportPosition = Camera.main.WorldToViewportPoint(ufoPosition);
                var isInView = viewportPosition.x is >= 0 and <= 1 && viewportPosition.y is >= 0 and <= 1;
                if (isInView) continue;
                
                // 家電に付与されているPresenterを取得
                var presenter = electronics.GetComponent<ElectronicsPresenter>();
            
                // Presenterで決定した座標をもとに初期座標を決定
                var spawnPosition = presenter.DetermineSpawnPoints(ufoPosition, spawnRadius, exclusionRadius);
                electronics.transform.position = spawnPosition;
            
                // 家電の数をインクリメント
                CurrentElectronics++;
            }
        }

        /// <summary>
        /// UFOの生成
        /// </summary>
        private void SpawnUfo()
        {
            for (var i = 0; i < maxUfo; i++)
            {
                // UFOのPrefabを選択
                var ufo = Instantiate(ufoPrefabs);
                
                // 名前にIndexをつける
                ufo.name = $"UFO_{i}";
            
                // UFOに付与されているPresenterを取得
                var presenter = ufo.GetComponent<UfoPresenter>();
            
                // Presenterで決定した座標をもとに初期座標を決定
                var spawnPosition = presenter.DetermineSpawnPoints();
                ufo.transform.position = spawnPosition;
                
                // UFOを生成する
                // Instantiate(ufo, spawnPosition, Quaternion.identity);
                
                // UFO管理リストに追加
                ufosList.Add(ufo);
                
                // UFOの数をインクリメント
                CurrentUfo++;
            }
        }
        

        /// <summary>
        /// 家電が死んだら家電カウントを1減らす
        /// </summary>
        public void OnElectronicsDeath()
        {
            CurrentElectronics--;
        }

        public void OnUfoDeath(GameObject ufo)
        {
            ufosList.Remove(ufo);
            CurrentUfo--;
        }



        /// <summary>
        /// MotherShipの生成とスポーン
        /// </summary>
        public async UniTask SpawnMotherShip(string address, Vector3 position, CancellationToken cancellationToken)
        {
            // Addressables経由でプレハブを非同期ロード
            var handle = Addressables.LoadAssetAsync<GameObject>(address);

            using (new HandleDisposable<GameObject>(handle))
            {
                var prefab = await handle;
                // ロードしたプレハブからGameObjectをインスタンス化
                Instantiate(prefab,position,Quaternion.identity);
            }
            OnGenerateMotherShip?.Invoke();
        }
        
        
        /// <summary>
        /// Alienのスポーン
        /// </summary>
        public async UniTask SpawnAlien()
        {
            // AlienManagerが設定されているか確認
            if (_alienManager == null)
            {
                Debug.LogError("EnemySpawnerにAlienManagerが設定されていません！");
                return;
            }

            while (this.isActiveAndEnabled)
            {
                for (int i = 0; i < numberOfSpawnAlien; i++)
                {
                    Vector3 position = _alienManager.GetRandomPosition();
                    var viewPosition = Camera.main.WorldToViewportPoint(position);
                    var isInView = viewPosition.z > 0 && 
                                   viewPosition.x >= 0 && viewPosition.x <= 1 && 
                                   viewPosition.y >= 0 && viewPosition.y <= 1;
                    if (isInView) continue;
                    
                    // 実際のスポーン処理はAlienManagerにすべて任せる
                    _alienManager.SpawnAlien(position, 1);
                }
                
                await UniTask.Delay(alienSpawnInterval * 100, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }
    }
}