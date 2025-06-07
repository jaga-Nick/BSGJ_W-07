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
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private float timer;
        [SerializeField] private int maxEnemies = 15;

        /// <summary>
        /// Prefab
        /// </summary>
        [SerializeField] private GameObject[] electronicsPrefabs;
        [SerializeField] private GameObject ufoPrefab;
        
        /// <summary>
        /// 現在のEnemyの数
        /// </summary>
        public int CurrentEnemies { get; set; } = 0;

        private void Start()
        {
            timer = spawnInterval;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (!(timer <= 0) || CurrentEnemies >= maxEnemies) return;
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
            CurrentEnemies++;
        }
        

        /// <summary>
        /// Enemyが死んだらカウントを1減らす
        /// </summary>
        public void OnEnemyDeath()
        {
            CurrentEnemies--;
        }
    }
}