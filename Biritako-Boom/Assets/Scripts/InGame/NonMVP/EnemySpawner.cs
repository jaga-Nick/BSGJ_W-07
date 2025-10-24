using System.Collections.Generic;

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Common;
using UnityEngine;
using System;

namespace InGame.NonMVP
{
    /// <summary>
    /// Enemyのスポナー
    /// </summary>
    public class EnemySpawner : MonoBehaviour
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
        public int CurrentElectronics { get; set; }
        public int CurrentUfo { get; set; }
        public int CurrentAlien { get; set; }
        
        
        /// <summary>
        /// Camera
        /// </summary>
        private Camera _camera;
        // デフォルトを設定して静的解析の警告を軽減（Awake で上書きされる）
        private float _cameraHeight = 5f;
        private float _cameraWidth = 5f;

        /// <summary>
        /// マップの範囲を定義するコライダー
        /// </summary>
        [Header("マップの範囲を定義するコライダー")]
        [SerializeField] private Collider2D mapBoundary;
        private Bounds _mapBounds;

        /// <summary>
        /// Cameraの設定
        /// </summary>
        private void Awake()
        {
            _camera = Camera.main;
            // カメラのワールド幅・高さを計算する
            if (_camera != null)
            {
                if (_camera.orthographic)
                {
                    // orthographic の場合、orthographicSize が画面縦の半分
                    _cameraHeight = _camera.orthographicSize;
                    _cameraWidth = _cameraHeight * _camera.aspect;
                }
                else
                {
                    // perspective の場合は、カメラからZ=0平面までの距離でViewport->Worldを使って算出
                    var camPos = _camera.transform.position;
                    var distance = Mathf.Abs(camPos.z); // カメラとワールドZ=0面の距離
                    var bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, distance));
                    var topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, distance));
                    _cameraWidth = (topRight.x - bottomLeft.x) / 2f;
                    _cameraHeight = (topRight.y - bottomLeft.y) / 2f;
                }
            }
            else
            {
                // Main Camera が見つからない場合のフォールバック値
                _cameraHeight = 5f; // 任意のデフォルト半高さ
                _cameraWidth = _cameraHeight * (Screen.width / (float)Screen.height);
            }
        }

        private void Start()
        {
            // マップの範囲を取得
            if (mapBoundary != null)
            {
                _mapBounds = mapBoundary.bounds;
            }
            else
            {
                Debug.LogWarning("Map Boundaryが設定されていません。デフォルトの範囲を使用します。");
                _mapBounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 0f));
            }
            // タイマーをスポーン時間のインターバルにセット
            _timer = spawnInterval;
            // UFOをスポーンする
            SpawnUfo().Forget();
            // 母艦UFOをスポーンする
            SpawnMotherShip(motherShipAddress, new Vector3(0f, 30f, 0f), CancellationToken.None).Forget();
            // AlienManagerの参照を取得
            _alienManager = FindFirstObjectByType<AlienManager>();
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
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
                // フィールドを使うように変更（パラメータ名の隠蔽を避ける）
                electronics.transform.position = DetermineElectronicsSpawnPoints();
            
                // 家電の数をインクリメント
                CurrentElectronics++;
                
                await UniTask.Yield();
            }
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
            // マップの範囲内でランダムな位置を生成
            Vector3 spawnPosition;
            do
            {
                spawnPosition = new Vector3(
                    UnityEngine.Random.Range(_mapBounds.min.x, _mapBounds.max.x),
                    UnityEngine.Random.Range(_mapBounds.min.y, _mapBounds.max.y),
                    0f
                );
            }
            // カメラの視界内ならやり直し（画面外になるまで繰り返し）
            while (IsPositionVisible(spawnPosition));

            return spawnPosition;
        }

        /// <summary>
        /// 指定された位置がカメラの視界内かどうかを判定
        /// </summary>
        private bool IsPositionVisible(Vector3 worldPosition)
        {
            if (_camera == null) return false;

            var viewportPoint = _camera.WorldToViewportPoint(worldPosition);
            return viewportPoint is { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 };
        }

        /// <summary>
        /// 家電のスポーンされる座標を決定する
        /// </summary>
        private Vector3 DetermineElectronicsSpawnPoints()
        {
            // ランダムなUFOを選択
            Vector3 ufoPosition;
            if (_ufoList.Count > 0)
            {
                var randomUfo = _ufoList[UnityEngine.Random.Range(0, _ufoList.Count)];
                ufoPosition = randomUfo.transform.position;
            }
            else
            {
                // UFOがいない場合は、マップ内のランダムな位置を使用
                ufoPosition = new Vector3(
                    UnityEngine.Random.Range(_mapBounds.min.x, _mapBounds.max.x),
                    UnityEngine.Random.Range(_mapBounds.min.y, _mapBounds.max.y),
                    0f
                );
            }

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
        /// ランダムな値を取得。カメラ中心を基準にして画面外の範囲を返すオーバーロード
        /// </summary>
        /// <param name="center">カメラ中心のXまたはY座標</param>
        /// <param name="limit">カメラのワールド座標での半分の幅または高さ（例: _cameraWidth）</param>
        /// <param name="buffer">画面の端からどれだけ外側にスポーンさせるかのバッファ</param>
        /// <returns>カメラの描画範囲外となるXまたはY座標値の片方</returns>
        private static float DynamicRandomRun(float center, float limit, float buffer)
        {
            // より遠くにスポーンするように、最小距離をbufferの分だけ離す
            var minDist = limit + buffer;
            var maxDist = limit + buffer * 2f; // 最大距離は最小距離の2倍
            return UnityEngine.Random.value < 0.5f ? UnityEngine.Random.Range(center - maxDist, center - minDist) : UnityEngine.Random.Range(center + minDist, center + maxDist);
        }


        /// <summary>
        /// 家電が死んだら家電カウントを減らす
        /// </summary>
        /// <param name="deadCount"></param>
        public void OnElectronicsDead(int deadCount)
        {
            CurrentElectronics -= deadCount;
        }

        
        /// <summary>
        /// UFOが死んだらUFOカウントを減らし、その位置にエイリアンをスポーンする
        /// </summary>
        /// <param name="ufo"></param>
        public void OnUfoDead(GameObject ufo)
        {
            // UFOの最後の位置を記録
            var lastUfoPosition = ufo.transform.position;
            
            // UFOをリストから削除し、カウントを減らす
            _ufoList.Remove(ufo);
            CurrentUfo--;


            // UFOが倒された正確な位置にエイリアンをスポーン
            SpawnAlien(lastUfoPosition).Forget();
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
        /// UFOが爆発されたところにスポーンする
        /// </summary>
        public async UniTask SpawnAlien(Vector3 position)
        {
            await  UniTask.Delay(2000, cancellationToken: this.GetCancellationTokenOnDestroy());
            Debug.Log($"Spawning {numberOfSpawnAlien} aliens at position {position}"); // デバッグログを追加
            
            for (var i = 0; i < numberOfSpawnAlien; i++)
            {
                if (CurrentAlien >= 20) break; // 各生成時に上限チェック
                _alienManager.SpawnAlien(position, 1);
                CurrentAlien++;
            }
        }
    }
}
