using System.Collections;
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

        private GameObject enemyPrefab;
        private Transform[] spawnPoints;

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
                SpawnEnemy();
            }
        }

        /// <summary>
        /// Enemyをスポーンする
        /// </summary>
        private void SpawnEnemy()
        {
            // 座標を取得する
            var index = Random.Range(0, spawnPoints.Length);
            // 生成する
            Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
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