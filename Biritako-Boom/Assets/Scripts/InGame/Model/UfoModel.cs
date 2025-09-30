using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using System;
using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// UFO（Enemy）の管理クラス
    /// </summary>
    public class UfoModel : MonoBehaviour, IEnemyModel
    {
        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float limitMoveDistance { get; }
        public Rigidbody2D rb { get; set; }
        public float currentTime { get; set; }
        public float intervalTime { get; set; }
        public Vector3 angle { get; set; }
        
        /// <summary>
        /// 速さと座標
        /// </summary>
        public float speed { get; set; }
        public Vector3 position { get; set; }
        
        /// <summary>
        /// UFOのmodel管理
        /// </summary>
        public event Action ufoHpChanged;
        
        /// <summary>
        /// 爆発力
        /// </summary>
        public float explosionPower { get; }
        
        /// <summary>
        /// UFOのHP管理のフィールド
        /// </summary>
        public int minUfoHp { get; set; } = 0;
        public int maxUfoHp { get; set; } = 100;

        /// <summary>
        /// IEnemyModelの現在のHP
        /// </summary>
        int IEnemyModel.currentHp { get; set; } = 1;
        

        /// <summary>
        /// UFOのスコア管理のフィールド
        /// </summary>
        public int ufoScore { get; set; }
        public int ufoDeadScore { get; set; }
        public int currentScore { get; set; }


        /// <summary>
        /// UFOがダメージを受けた時の処理。
        /// </summary>
        /// <param name="damage"></param>
        void IEnemyModel.OnDamage(int damage)
        {
            // UFOのHPを減らす
            DecrementUfoHp(damage);
            // スコアを増やす
            ScoreModel.Instance().IncrementScore(ufoDeadScore);
            // HPが0になったら殺す
            if (((IEnemyModel)this).currentHp <= 0)
            {
                ((IEnemyModel)this).OnDead();
            }
        }

        /// <summary>
        /// UFOのHPをamountに応じて減らす。
        /// </summary>
        /// <param name="amount"></param>
        private void DecrementUfoHp(int amount)
        {
            ((IEnemyModel)this).currentHp -= amount;
            ((IEnemyModel)this).currentHp = Mathf.Clamp(((IEnemyModel)this).currentHp, minUfoHp, maxUfoHp);
            UpdateUfoHp();
        }
        

        /// <summary>
        /// UFOのHPをもとに戻す。全回復。
        /// </summary>
        public void RestoreUfoHp()
        {
            ((IEnemyModel)this).currentHp = maxUfoHp;
        }

        /// <summary>
        /// HP変更イベント。
        /// </summary>
        private void UpdateUfoHp()
        {
            ufoHpChanged?.Invoke();
        }
        
        /// <summary>
        /// UFOの死亡時処理
        /// </summary>
        /// <returns></returns>
        async UniTask IEnemyModel.OnDead()
        {
            // スコアを加算する
            ScoreModel.Instance().IncrementScore(ufoScore);
            // 死んだときの爆発エフェクトを表示
            GenerateExplosionManager.Instance(true).Factory(gameObject.transform.position, 2);
            // SpawnerのUFOカウントを減らしたことを通知
            var enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
            enemySpawner.OnUfoDead(gameObject);
            // UFOを破棄
            GameObject.Destroy(gameObject);
        }
    }
}
