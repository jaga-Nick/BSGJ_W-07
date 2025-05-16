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
        /// �R���Z���g���h���b�v(�Ԃ�)�֐��ł����C������Ȃ��̂ň�ƍl��int�ŕԂ��Ă��܂�
        /// 
        /// </summary>

        public int DropConcent()
        {
            // �Ƃ肠����1��Ԃ�
            return 1;
        }

        /// <summary>
        /// /// <summary>
        /// Addressables�Ő�������Ă���(����)UFO��j�󂵂܂�
        /// </summary>
        /// <param name="ufoInstance"> �������̃C���X�^���X��n���Ă������� </param>
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
