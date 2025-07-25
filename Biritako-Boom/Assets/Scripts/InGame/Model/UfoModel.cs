﻿using Cysharp.Threading.Tasks;
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
        public float LimitMoveDistance { get; }
        public Rigidbody2D Rb { get; set; }
        public float CurrentTime { get; set; }
        public float IntervalTime { get; set; }
        public Vector3 Angle { get; set; }
        
        /// <summary>
        /// 速さと座標
        /// </summary>
        public float Speed { get; set; }
        public Vector3 Position { get; set; }
        
        /// <summary>
        /// UFOのmodel管理
        /// </summary>
        public event Action UfoHpChanged;
        
        /// <summary>
        /// 爆発力
        /// </summary>
        public float ExplosionPower { get; }
        
        /// <summary>
        /// UFOのHP管理のフィールド
        /// </summary>
        public int MinUfoHp { get; set; } = 0;
        public int MaxUfoHp { get; set; } = 100;

        /// <summary>
        /// IEnemyModelの現在のHP
        /// </summary>
        int IEnemyModel.CurrentHp { get; set; } = 1;
        

        /// <summary>
        /// UFOのスコア管理のフィールド
        /// </summary>
        public int UfoScore { get; set; }
        public int UfoDeadScore { get; set; }
        public int CurrentScore { get; set; }


        /// <summary>
        /// UFOがダメージを受けた時の処理。
        /// </summary>
        /// <param name="damage"></param>
        void IEnemyModel.OnDamage(int damage)
        {
            // UFOのHPを減らす
            DecrementUfoHp(damage);
            // スコアを増やす
            ScoreModel.Instance().IncrementScore(UfoDeadScore);
            // HPが0になったら殺す
            if (((IEnemyModel)this).CurrentHp <= 0)
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
            ((IEnemyModel)this).CurrentHp -= amount;
            ((IEnemyModel)this).CurrentHp = Mathf.Clamp(((IEnemyModel)this).CurrentHp, MinUfoHp, MaxUfoHp);
            UpdateUfoHp();
        }
        

        /// <summary>
        /// UFOのHPをもとに戻す。全回復。
        /// </summary>
        public void RestoreUfoHp()
        {
            ((IEnemyModel)this).CurrentHp = MaxUfoHp;
        }

        /// <summary>
        /// HP変更イベント。
        /// </summary>
        private void UpdateUfoHp()
        {
            UfoHpChanged?.Invoke();
        }
        
        /// <summary>
        /// UFOの死亡時処理
        /// </summary>
        /// <returns></returns>
        async UniTask IEnemyModel.OnDead()
        {
            // スコアを加算する
            ScoreModel.Instance().IncrementScore(UfoScore);
            // 死んだときの爆発エフェクトを表示
            GenerateExplosionManager.Instance().Factory(gameObject.transform.position, 2);
            // SpawnerのUFOカウントを減らしたことを通知
            var enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
            enemySpawner.OnUfoDead(gameObject);
            // UFOを破棄
            GameObject.Destroy(gameObject);
        }
    }
}
