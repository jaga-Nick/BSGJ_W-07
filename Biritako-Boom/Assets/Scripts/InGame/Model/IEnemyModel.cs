using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// Interfaceでの敵（家電）のmodel管理
    /// </summary>
    public interface IEnemyModel
    {
        public int CurrentHp { get; set; }

        //public int MaxUfoHp { get;}
        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float LimitMoveDistance { get; }
        public Rigidbody2D Rb { get;}
        public float CurrentTime { get; set; }
        public float IntervalTime { get; set; }
        public Vector3 Angle { get; set; }
        
        /// <summary>
        /// 爆発力
        /// </summary>
        public float ExplosionPower { get;}

        /// <summary>
        /// ダメージを食らうときの処理
        /// </summary>
        /// <param name="damage"></param>
        public void OnDamage(int damage) 
        {
            CurrentHp -= damage;
            if (CurrentHp <= 0) OnDead();
        }


        /// <summary>
        /// 何の家電かを数字で判別する
        /// </summary>
        /// <returns></returns>
        public virtual int GetEnemyType()
        {
            // 家電の場合1
            // UFOの場合2
            // 母艦UFOの場合3
            return 0;
        }

        /// <summary>
        /// ランダムに動きます。
        /// </summary>
        public virtual void Move()
        {
            CurrentTime += Time.deltaTime;

            if (IntervalTime >= CurrentTime)
            {
                var num=Random.Range(1, 360);
                var value = Mathf.Deg2Rad*num;

                Angle = new Vector3(Mathf.Cos(value), Mathf.Sin(value), 0);
                
                //次の時間設定
                IntervalTime=Random.Range(1, 5);
            }
            Rb.linearVelocity = Angle;
        }

        /// <summary>
        /// 死亡時の演出、処理。
        /// </summary>
        /// <returns></returns>
        public UniTask OnDead();
    }
}