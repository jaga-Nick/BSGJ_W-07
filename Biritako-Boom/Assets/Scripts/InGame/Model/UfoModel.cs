using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

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

        int IEnemyModel.CurrentHp { get; set; } = 1;
        

        /// <summary>
        /// UFOのスコア管理のフィールド
        /// </summary>
        public int UfoScore { get; set; }
        public int CurrentScore { get; set; }


        /// <summary>
        /// UFOのHPをamountに応じて増やす。
        /// </summary>
        /// <param name="amount"></param>
        public void IncrementUfoHp(int amount)
        {
            ((IEnemyModel)this).CurrentHp += amount;
            ((IEnemyModel)this).CurrentHp = Mathf.Clamp(((IEnemyModel)this).CurrentHp, MinUfoHp, MaxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOのHPをamountに応じて減らす。
        /// </summary>
        /// <param name="amount"></param>
        public void DecrementUfoHp(int amount)
        {
            ((IEnemyModel)this).CurrentHp -= amount;
            ((IEnemyModel)this).CurrentHp = Mathf.Clamp(((IEnemyModel)this).CurrentHp, MinUfoHp, MaxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOが死んだかどうかのブール判定。
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return ((IEnemyModel)this).CurrentHp <= 0;
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
        /// ランダムな方向を取得
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomDirection()
        {
            var randomIndex = Random.Range(0, 4);
            return randomIndex switch
            {
                0 => Vector2.up,
                1 => Vector2.down,
                2 => Vector2.left,
                3 => Vector2.right,
                _ => Vector2.down
            };
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
        /// 死亡時処理
        /// </summary>
        /// <returns></returns>
        public async UniTask OnDead()
        {
            
        }

        /*
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
        */
    }
}
