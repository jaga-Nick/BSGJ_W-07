using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// 破壊可能でどこからでも呼び出せるようにする。
    /// </summary>
    public class GenerateExplosionManager:DestroyAvailable_SingletonMonoBehaviourBase<GenerateExplosionManager>
    {
        [Header("カットインに必要")]
        [SerializeField]
        private Canvas CutInCanvas;
        [SerializeField]
        private GameObject CutIn;

        [Header("爆発大")]
        [SerializeField]
        private float BigExplosionSize = 0.25f;
        [SerializeField]
        private int BigDamage = 100;

        [Header("爆発中")]
        [SerializeField]
        private float MiddleCollisionSize=0.25f;
        [SerializeField]
        private int MiddleDamage = 75;

        [Header("爆発小")]
        [SerializeField]
        private float SmallCollisionSize=0.25f;
        [SerializeField]
        private int SmallDamage = 50;


        [Header("基準値")]
        [SerializeField]
        private GameObject ExplosionObject;

        public void Awake()
        {
            //デバッグ用の保険。
            instance = this;
        }

        public GameObject GenerateCutIn()
        {
            GameObject cutin=Instantiate(CutIn, CutInCanvas.transform);
            return cutin;
        }

        /// <summary>
        /// 爆発生成(InterfaceでSwitch変わりしたほうがいいけどプランナーに調節させたらでええわ。）
        /// </summary>
        public async void Factory(Vector3 point,int ExplosionPower)
        {
            GameObject gameObject = null;

            Vector3 vec = new Vector3(point.x, point.y,0);
            switch (ExplosionPower)
            {
                case 0:
                    gameObject=Instantiate(ExplosionObject, vec, Quaternion.identity);

                    ExplosionAttach smallExplosionAttach=gameObject.GetComponent<ExplosionAttach>();
                    CircleCollider2D collider_sma=gameObject.GetComponent<CircleCollider2D>();
                    collider_sma.radius=SmallCollisionSize;

                    smallExplosionAttach.SetDamage(SmallDamage);
                    await smallExplosionAttach.Explosion("SmallExplosion");
                    break;
                case 1:
                    gameObject =Instantiate(ExplosionObject, vec, Quaternion.identity);
                    ExplosionAttach middleExplosionAttach = gameObject.GetComponent<ExplosionAttach>();
                    CircleCollider2D collider_mid = gameObject.GetComponent<CircleCollider2D>();
                    collider_mid.radius = SmallCollisionSize;

                    middleExplosionAttach.SetDamage(MiddleDamage);
                    await middleExplosionAttach.Explosion("MiddiumExplosion");
                    break;
                case 2:
                    gameObject=Instantiate(ExplosionObject, vec, Quaternion.identity);
                    CircleCollider2D collider_big = gameObject.GetComponent<CircleCollider2D>();
                    collider_big.radius = SmallCollisionSize;
                    ExplosionAttach bigExplosionAttach = gameObject.GetComponent<ExplosionAttach>();

                    bigExplosionAttach.SetDamage(BigDamage);
                    await bigExplosionAttach.Explosion("BigExplosion");
                    break;
            }
        }
    }
}