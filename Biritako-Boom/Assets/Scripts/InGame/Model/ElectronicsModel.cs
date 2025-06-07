using UnityEngine;

namespace InGame.Model
{

    public class ElectronicsModel : IEnemyModel
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
        /// 爆発力
        /// </summary>
        public float ExplosionPower { get; }
        
        /// <summary>
        /// 家電のモーション
        /// </summary>
        public virtual void Move()
        {
            CurrentTime += Time.deltaTime;

            if (IntervalTime >= CurrentTime)
            {
                var num = Random.Range(1, 360);
                var value = Mathf.Deg2Rad * num;

                Angle = new Vector3(Mathf.Cos(value), Mathf.Sin(value), 0);
                
                //次の時間設定
                IntervalTime=Random.Range(1, 5);
            }
            Rb.linearVelocity = Angle;
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