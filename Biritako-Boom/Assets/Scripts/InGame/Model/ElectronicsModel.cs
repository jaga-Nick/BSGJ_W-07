using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Model
{

    public class ElectronicsModel : MonoBehaviour,IEnemyModel
    {
        public void Initialize(Rigidbody2D rb,Vector3 trans)
        {
            this.Rb = rb;
            this.Position=trans;
        }

        int IEnemyModel.CurrentHp { get; set; } = 1;

        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float LimitMoveDistance { get; }
        public Rigidbody2D Rb { get; set; }
        public float CurrentTime { get; set; }
        public float IntervalTime { get; set; }
        public Vector3 Angle { get; set; }
        
        /// <summary>
        /// 速さと向きと座標
        /// </summary>
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
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
        /// 家電の座標をセットする
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPositon(Vector3 newPosition)
        {
            Position = newPosition;
        }
       

        public async UniTask OnDead()
        {
            
        }
    }
}