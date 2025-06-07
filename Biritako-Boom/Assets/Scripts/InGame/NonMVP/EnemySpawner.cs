using System.Collections;
using System.Collections.Generic;
using InGame.Presenter;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// Enemyのスポナー
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private float timer;
        [SerializeField] private int maxEnemies = 15;

        [SerializeField] private GameObject[] electronicsPrefabs;

        private ElectronicsPresenter _electronicsPresenter;
        private UfoPresenter _ufoPresenter;
        
        public int CurrentEnemies { get; set; } = 0;

        private void Start()
        {
            timer = spawnInterval;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0　&& CurrentEnemies < maxEnemies)
            {
                // SpawnElectronics();
                timer = spawnInterval;
            }
        }

        private void SpawnElectronics()
        {
            // 家電を選択して生成する
            var randomIndex = Random.Range(0, electronicsPrefabs.Length);
            var electronics = Instantiate(electronicsPrefabs[randomIndex]);
            
            // 家電のPresenterを取得
            _electronicsPresenter = electronics.GetComponent<ElectronicsPresenter>();

            // Presenterで決定した座標をもとに初期座標を決定
            var spawnPosition = _electronicsPresenter.DetermineSpawnPoints();
            electronics.transform.position = spawnPosition;
            
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