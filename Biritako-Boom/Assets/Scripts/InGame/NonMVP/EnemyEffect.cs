using UnityEngine;

namespace InGame.NonMVP
{
    public class EnemyEffect : MonoBehaviour
    {
        [Header("Effect生成位置"), SerializeField]
        private GameObject _effectGenPos;
        [Header("Effectプレハブ"), SerializeField]
        private GameObject _effectObj;
        
        
        public void GenerateEffect()
        {
            Vector3 pos = transform.position;
            Instantiate(_effectObj, pos, Quaternion.identity);
        }
    }
}

