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

        /// <summary>
        /// Prefab
        /// </summary>
        [Header("Prefab")]
        [Header("家電")]
        [SerializeField] private GameObject[] electronicsPrefabs;
        [Header("UFO")]
        [SerializeField] private GameObject[] ufoPrefabs;
        
        /// <summary>
        /// UFOの生成設定（もしかしたらUfoPresenterに移行）
        /// </summary>
        [Header("UFOの生成設定")]
        [Header("生成されるUFO間の最小距離")]
        [SerializeField] private float minDistanceBetweenUfo = 0.01f;
        

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
            var electronics = Instantiate(electronicsPrefabs[randomIndex]);
            
            // 家電に付与されているPresenterを取得
            var presenter = electronics.GetComponent<ElectronicsPresenter>();
            
            // Presenterで決定した座標をもとに初期座標を決定
            var spawnPosition = presenter.DetermineSpawnPoints();
            electronics.transform.position = spawnPosition;
            
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
                var ufo = Instantiate(ufoPrefabs[randomIndex]);
            
                // UFOに付与されているPresenterを取得
                var presenter = ufo.GetComponent<UfoPresenter>();
            
                // Presenterで決定した座標をもとに初期座標を決定
                var spawnPosition = presenter.DetermineSpawnPoints();
                ufo.transform.position = spawnPosition;
                
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