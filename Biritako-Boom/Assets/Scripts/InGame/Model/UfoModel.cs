using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace InGame.Model
{
    /// <summary>
    /// UFO（Enemy）の管理クラス
    /// </summary>
    public class UfoModel : IEnemyModel
    {
        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float LimitMoveDistance { get; }
        public Rigidbody2D Rb { get; }
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
        public int CurrentUfoHp { get; set; }
        

        /// <summary>
        /// UFOのスコア管理のフィールド
        /// </summary>
        private const int Score = 100;
        public int CurrentScore { get; set; }


        /// <summary>
        /// UFOのHPをamountに応じて増やす。
        /// </summary>
        /// <param name="amount"></param>
        public void IncrementUfoHp(int amount)
        {
            CurrentUfoHp += amount;
            CurrentUfoHp = Mathf.Clamp(CurrentUfoHp, MinUfoHp, MaxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOのHPをamountに応じて減らす。
        /// </summary>
        /// <param name="amount"></param>
        public void DecrementUfoHp(int amount)
        {
            CurrentUfoHp -= amount;
            CurrentUfoHp = Mathf.Clamp(CurrentUfoHp, MinUfoHp, MaxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOが死んだかどうかのブール判定。
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return CurrentUfoHp <= 0;
        }

        /// <summary>
        /// UFOのHPをもとに戻す。全回復。
        /// </summary>
        public void RestoreUfoHp()
        {
            CurrentUfoHp = MaxUfoHp;
        }

        /// <summary>
        /// HP変更イベント。
        /// </summary>
        private void UpdateUfoHp()
        {
            UfoHpChanged?.Invoke();
        }
        
        /// <summary>
        /// UFOの座標をセットする
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPositon(Vector3 newPosition)
        {
            Position = newPosition;
        }
        
        /// <summary>
        /// UFOを破壊する、
        /// </summary>
        /// <param name="ufoInstance"></param>
        public void DestroyUfo(GameObject ufoInstance)
        {
            if (!ufoInstance) return;
            Addressables.ReleaseInstance(ufoInstance); 
            ufoInstance = null;
        }
    }
}
