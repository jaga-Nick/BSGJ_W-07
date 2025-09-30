using UnityEngine;

namespace InGame.NonMVP
{
    public class EnemyEffect : MonoBehaviour
    {
        [Header("Effect生成位置"), SerializeField]
        private GameObject effectGenPos;
        [Header("Effectプレハブ"), SerializeField]
        private GameObject effectObj;
        
        
        public void GenerateEffect()
        {
            Vector3 pos = transform.position;
            Instantiate(effectObj, pos, Quaternion.identity);
        }
    }
}

