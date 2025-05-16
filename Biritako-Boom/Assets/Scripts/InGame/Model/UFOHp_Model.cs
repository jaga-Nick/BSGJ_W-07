using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UFO
{
    public class UfoHp_Model : MonoBehaviour
    {
        private int _hp = 100;

        public void ResetHP()
        {
            _hp = 100;
        }

        public int GetUFOHp()
        {
            return _hp;
        }

        public void TakeDamage(int dmg)
        {
            _hp -= dmg;
        }

        public bool IsDead()
        {
            return _hp <= 0;
        }


        /// <summary>
        /// コンセントをドロップ(返す)関数ですが，分からないので一個と考えintで返しています
        /// 
        /// </summary>

        public int DropConcent()
        {
            // とりあえず1を返す
            return 1;
        }

        /// <summary>
        /// /// <summary>
        /// Addressablesで生成されている(仮定)UFOを破壊します
        /// </summary>
        /// <param name="ufoInstance"> 生成時のインスタンスを渡してください </param>
        public void DestroyUFO(GameObject ufoInstance)
        {
            if (ufoInstance != null)
            {
                Addressables.ReleaseInstance(ufoInstance); 
                ufoInstance = null;
            }
        }

    }
}
