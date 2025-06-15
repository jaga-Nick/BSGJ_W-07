using System.Collections.Generic;
using System.Linq;
using InGame.Presenter;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Enemyのスポナー
    /// </summary>
    public class EnemySpawner : MonoBehaviour
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
        [SerializeField] private int maxElectronics = 15;
        [Header("生成されるUFOの上限数")]
        [SerializeField] private int maxUfo = 14;
        [Header("UFO周辺の半径")]
        [SerializeField] private float spawnRadius = 2.0f;
        [Header("UFO直下の除外範囲")]
        [SerializeField] private float exclusionRadius = 0.5f;

        /// <summary>
        /// Prefab
        /// </summary>
        [Header("Prefab")]
        [Header("家電")]
        [SerializeField] private GameObject[] electronicsPrefabs;
        [Header("UFO")]
        [SerializeField] private GameObject[] ufoPrefabs;
        

        /// <summary>
        /// スポーンされたEnemyの座標を保持するリスト
        /// </summary>
        public List<Vector3> spawnElectronicsPositions;
        public List<Vector3> spawnUfoPositions;
        
        
        /// <summary>
        /// 現在のEnemyの数
        /// </summary>
        public int CurrentElectronics { get; set; } = 0;
        public int CurrentUfo { get; set; } = 0;

        private void Start()
        {
            // タイマーをスポーン時間のインターバルにセット
            timer = spawnInterval;
            SpawnUfo();
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
            // 家電を選択して生成する
            var randomIndex = Random.Range(0, electronicsPrefabs.Length);
            var electronics = electronicsPrefabs[randomIndex];
            
            // UFOの座標をランダムに取得
            var ufoRandomIndex = Random.Range(0, ufoPrefabs.Length);
            var ufoPosition = ufoPrefabs[ufoRandomIndex].transform.position;
            
            // 家電に付与されているPresenterを取得
            var presenter = electronics.GetComponent<ElectronicsPresenter>();
            
            // Presenterで決定した座標をもとに初期座標を決定
            var spawnPosition = presenter.DetermineSpawnPoints(ufoPosition, spawnRadius, exclusionRadius);
            electronics.transform.position = spawnPosition;
            
            // 家電を生成する
            Instantiate(electronics, spawnPosition, Quaternion.identity);
            
            // 家電の数をインクリメント
            CurrentElectronics++;
        }

        /// <summary>
        /// UFOの生成
        /// </summary>
        private void SpawnUfo()
        {
            for (var i = 0; i < maxUfo; i++)
            {
                // UFOをランダムに選択する
                var randomIndex = Random.Range(0, ufoPrefabs.Length);
                var ufo = ufoPrefabs[randomIndex];
            
                // UFOに付与されているPresenterを取得
                var presenter = ufo.GetComponent<UfoPresenter>();
            
                // Presenterで決定した座標をもとに初期座標を決定
                var spawnPosition = presenter.DetermineSpawnPoints();
                ufo.transform.position = spawnPosition;
                
                // UFOを生成する
                Instantiate(ufo, spawnPosition, Quaternion.identity);
                
                // 生成したUFOの座標を配列に追加する
                spawnUfoPositions.Add(spawnPosition);
                
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

        public void OnUfoDeath()
        {
            CurrentUfo--;
        }
    }
}