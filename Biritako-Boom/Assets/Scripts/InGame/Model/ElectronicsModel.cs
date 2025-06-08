using UnityEngine;

namespace InGame.Model
{

    public class ElectronicsModel : IEnemyModel
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
        /// 爆発力
        /// </summary>
        public float ExplosionPower { get; }
        
        /// <summary>
        /// 何の家電かを数字で判別する
        /// </summary>
        /// <returns></returns>
        public virtual int GetEnemyType()
        {
            // 家電の場合1
            // UFOの場合2
            // 母艦UFOの場合3
            return 1;
        }
        
        /// <summary>
        /// 家電のモーション
        /// </summary>
        public virtual void Move()
        {
            // 上下左右どちらかに進む
            
        }

        /// <summary>
        /// 家電の座標をセットする
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPositon(Vector3 newPosition)
        {
            Position = newPosition;
        }
        
        /// <summary>
        /// 家電が死んだときの処理
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDead()
        {
            return false;
        }
    }
}