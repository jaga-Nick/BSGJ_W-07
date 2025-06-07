using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// 破壊可能でどこからでも呼び出せるようにする。
    /// </summary>
    public class GenerateExplosionManager:DestroyAvailable_SingletonMonoBehaviourBase<GenerateExplosionManager>
    {
        [Header("爆発大")]
        [SerializeField]
        private GameObject BigExplosion;
        [SerializeField]
        private float BigExplosionCollisionSize;
        [SerializeField]
        private float BigExplosionSize;
        [Header("爆発中")]
        [SerializeField]
        private GameObject MiddleExplosion;
        [SerializeField]
        private float MiddleCollisionSize;
        [Header("爆発小")]
        [SerializeField]
        private GameObject SmallExplosion;
        [SerializeField]
        private float SmallCollisionSize;

        [Header("基準値")]
        [SerializeField]
        private int ExplosionLimit;

        public void Awake()
        {
            //デバッグ用の保険。
            instance = this;
        }

        /// <summary>
        /// 爆発生成
        /// </summary>
        public async UniTask Factory(Vector3 point,int ExplosionPower)
        {
            GameObject gameObject = null;
            switch (ExplosionPower)
            {
                case 0:
                    gameObject=Instantiate(SmallExplosion, point, Quaternion.identity);

                    gameObject.GetComponent<ExplosionAttach>();
                    Animator animator = gameObject.GetComponent<Animator>();

                    break;
                case 1:
                    Instantiate(MiddleExplosion, point, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(BigExplosion, point, Quaternion.identity);
                    break;
            }
        }
    }
}