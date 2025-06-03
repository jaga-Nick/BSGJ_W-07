using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// Abstructでの敵（家電）のmodel管理
    /// </summary>
    public interface IEnemyModel
    {
        //public int MaxUfoHp { get;}
        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float LimitMoveDistance { get; }

        public Rigidbody2D Rb { get;}
        public float CurrentTime { get; set; }
        public float IntervalTime { get; set; }
        public Vector3 Angle { get; set; }
        public void Move()
        {
            CurrentTime += Time.deltaTime;

            if (IntervalTime >= CurrentTime)
            {
                int num=Random.Range(1, 360);
                var value = Mathf.Deg2Rad*num;

                Angle = new Vector3(Mathf.Cos(value), Mathf.Sin(value), 0);
                
                //次の時間設定
                IntervalTime=Random.Range(1, 5);
            }
            Rb.linearVelocity = Angle;
        }
        public void AngleChange(int num)
        {

        }
    }
}