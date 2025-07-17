using System.Collections.Generic;

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Common;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace InGame.NonMVP
{
    /// <summary>
    /// Enemyのスポナー
    /// </summary>
    public class EnemySpawner : DestroyAvailable_SingletonMonoBehaviourBase<EnemySpawner>
    {
        /// <summary>
        /// 時間設定
        /// </summary>
        [Header("タイマー")]
        [Header("スポーン時間のインターバル")]
        [SerializeField] private float spawnInterval = 3f;
        private float _timer;
        
        /// <summary>
        /// 家電
        /// </summary>
        [Header("生成される家電の上限数")]
        [SerializeField] private int maxElectronics;
        [Header("一度に生成される家電の数")]
        [SerializeField] private int numberOfSpawnElectronics;
        
        /// <summary>
        /// UFO
        /// </summary>
        [Header("生成されるUFOの上限数")]
        [SerializeField] private int maxUfo;
        [Header("UFO周辺の半径")]
        [SerializeField] private float spawnRadius;
        [Header("UFO直下の除外範囲")]
        [SerializeField] private float exclusionRadius;
        
        /// <summary>
        /// 宇宙人
        /// </summary>
        [Header("宇宙人スポーン時間のインターバル")]
        [SerializeField] private int alienSpawnInterval;
        [Header("一度に生成される宇宙人の数")]
        [SerializeField] private int numberOfSpawnAlien;
        
        /// <summary>
        /// 外部マネージャーへの参照
        /// </summary>
        private AlienManager _alienManager;

        /// <summary>
        /// Addressableアセットのキー
        /// </summary>
        [Header("母艦のアドレス")]
        [SerializeField] private string motherShipAddress = "Enemy_MotherShip";

        public static event Action OnGenerateMotherShip;

        /// <summary>
        /// Prefab
        /// </summary>
        [Header("家電Prefab")]
        [SerializeField] private GameObject[] electronicsPrefabs;
        [Header("UFOPrefab")]
        [SerializeField] private GameObject ufoPrefabs;
        
        
        /// <summary>
        /// 生成されたUFOたちの管理
        /// </summary>
        private readonly List<GameObject> _ufoList = new List<GameObject>();
        
        
        /// <summary>
        /// 現在のEnemyの数
        /// </summary>
        public int CurrentElectronics { get; set; } = 0;
        public int CurrentUfo { get; set; } = 0;
        
        
        /// <summary>
        /// Camera
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// Cameraの設定
        /// </summary>
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            //アタッチされているのでそのままを維持する為に格納。（一つのみを想定しているので維持しない場合も放置でもよい。
            instance = this;
            // タイマーをスポーン時間のインターバルにセット
            _timer = spawnInterval;
            // UFOをスポーンする
            SpawnUfo().Forget();
            // 母艦UFOをスポーンする
            SpawnMotherShip(motherShipAddress, new Vector3(0f, 30f, 0f), CancellationToken.None).Forget();
            // 宇宙人をスポーンする
            _alienManager = FindObjectOfType<AlienManager>();
            SpawnAlien().Forget();
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            // Debug.Log(CurrentElectronics);
            if (!(_timer <= 0) || CurrentElectronics >= maxElectronics) return;
            SpawnElectronics().Forget();
            _timer = spawnInterval;
        }

        /// <summary>
        /// 家電のスポーン
        /// </summary>
        private async UniTask SpawnElectronics()
        {
            for (var i = 0; i < numberOfSpawnElectronics; i++)
            {
                // 家電を選択して生成する
                var randomIndex = UnityEngine.Random.Range(0, electronicsPrefabs.Length);
                var electronics = Instantiate(electronicsPrefabs[randomIndex]);
                
                // Presenterで決定した座標をもとに初期座標を決定
                electronics.transform.position = DetermineElectronicsSpawnPoints(spawnRadius, exclusionRadius, _camera);
            
                // 家電の数をインクリメント
                CurrentElectronics++;
                
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 家電のスポーンされる座標を決定する
        /// </summary>
        /// <param name="spawnRadius"></param>
        /// <param name="exclusionRadius"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        private Vector3 DetermineElectronicsSpawnPoints(float spawnRadius, float exclusionRadius, Camera camera)
        {
            
            // カメラ外にいるUFOをランダムで決定する
            var offScreenUfo = new List<Transform>();
            foreach (var ufo in _ufoList)
            {
                var viewportPosition = camera.WorldToViewportPoint(ufo.transform.position);
                var isInView = viewportPosition.x is >= 0 and <= 1 && viewportPosition.y is >= 0 and <= 1;
                if (!isInView) { offScreenUfo.Add(ufo.transform); }
            }
            
            // カメラ外からUFOをランダムで選択
            var ufoRandomIndex = UnityEngine.Random.Range(0, offScreenUfo.Count);
            var ufoPosition = offScreenUfo[ufoRandomIndex].position;
            
            // UFOの座標半径いくらかを取得してポジションを決める
            Vector3 spawnOffset;
            do
            {
                var randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
                spawnOffset = new Vector3(randomCircle.x, randomCircle.y, 0);
            } 
            while (spawnOffset.magnitude < exclusionRadius);
            
            var spawnPosition = ufoPosition + spawnOffset;
            
            return spawnPosition;
        }

        /// <summary>
        /// UFOのスポーン
        /// </summary>
        private async UniTask SpawnUfo()
        {
            for (var i = 0; i < maxUfo; i++)
            {
                // UFOのPrefabを選択
                var ufo = Instantiate(ufoPrefabs);
                
                // 名前にIndexをつける
                ufo.name = $"UFO_{i}";
            
                // Presenterで決定した座標をもとに初期座標を決定
                ufo.transform.position = DetermineUfoSpawnPoints();
                
                // UFO管理リストに追加
                _ufoList.Add(ufo);
                
                // UFOの数をインクリメント
                CurrentUfo++;
                
                await UniTask.Yield();
            }
        }
        
        /// <summary>
        /// UFOのスポーンされる座標を決定する
        /// </summary>
        private Vector3 DetermineUfoSpawnPoints()
        {
            // ランダムな座標を生成
            var randomPositionX = RandomRun();
            var randomPositionY = RandomRun();
            
            // 画面外の座標を取得
            var position = _camera.ViewportToWorldPoint(new Vector3(randomPositionX, randomPositionY, _camera.nearClipPlane));
            return position;
        }
        
        /// <summary>
        /// ランダムな値を取得
        /// </summary>
        /// <returns></returns>
        private static float RandomRun()
        {
            float value;
            do { value = UnityEngine.Random.Range(-1.0f, 2.0f); } while (value is >= 0.0f and <= 1.0f);
            return value;
        }


        /// <summary>
        /// 家電が死んだら家電カウントを減らす
        /// </summary>
        /// <param name="electronics"></param>
        /// <param name="deadCount"></param>
        public void OnElectronicsDead(int deadCount)
        {
            CurrentElectronics -= deadCount;
        }

        public void OnUfoDead(GameObject ufo)
        {
            _ufoList.Remove(ufo);
            CurrentUfo--;
        }
        

        /// <summary>
        /// MotherShipのスポーン
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
            if (!_alienManager)
            {
                Debug.LogError("EnemySpawnerにAlienManagerが設定されていません！");
                return;
            }

            while (this.isActiveAndEnabled)
            {
                for (var i = 0; i < numberOfSpawnAlien; i++)
                {
                    var position = _alienManager.GetRandomPosition();
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