using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.NonMVP
{
    public class EnemyEffect : MonoBehaviour
    {
        [FormerlySerializedAs("_effectGenPos")] [Header("Effect生成位置"), SerializeField]
        private GameObject effectGenPos;
        [FormerlySerializedAs("_effectObj")] [Header("Effectプレハブ"), SerializeField]
        private GameObject effectObj;
        
        
        public void GenerateEffect()
        {
            Vector3 pos = transform.position;
            Instantiate(effectObj, pos, Quaternion.identity);
        }
    }
}

