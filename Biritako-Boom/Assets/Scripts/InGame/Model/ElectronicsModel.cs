using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using UnityEngine;

namespace InGame.Model
{

    public class ElectronicsModel : MonoBehaviour, IEnemyModel
    {
        public void Initialize(Rigidbody2D rb,Vector3 trans)
        {
            this.rb = rb;
            this.Position=trans;
        }

        int IEnemyModel.currentHp { get; set; } = 1;

        /// <summary>
        /// 移動許容距離
        /// </summary>
        public float limitMoveDistance { get; }
        public Rigidbody2D rb { get; set; }
        public float currentTime { get; set; }
        public float intervalTime { get; set; }
        public Vector3 angle { get; set; }
        
        /// <summary>
        /// 速さと向きと座標
        /// </summary>
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        
        /// <summary>
        /// 爆発力
        /// </summary>
        public float explosionPower { get; }
        
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
        /// 家電の座標をセットする
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPositon(Vector3 newPosition)
        {
            Position = newPosition;
        }
       

        /// <summary>
        /// 家電の死亡時処理
        /// </summary>
        async UniTask IEnemyModel.OnDead()
        {
            // 
        }
    }
}