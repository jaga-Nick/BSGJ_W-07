using UnityEngine;

namespace InGame.NonMVP
{
    public class GenerateExplosionManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject BigExplosion;
        [SerializeField]
        private GameObject MiddleExplosion;
        [SerializeField]
        private GameObject SmallExplosion;

        /// <summary>
        /// 爆発
        /// </summary>
        public void Factory(int ExplosionPower)
        {

        }
    }
}