using Setting;
using System;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// 破壊可能でどこからでも呼び出せるようにする。
    /// </summary>
    public class GenerateExplosionManager:DestroyAvailable_SingletonMonoBehaviourBase<GenerateExplosionManager>
    {
        /// <summary>
        /// カットインパラメタ
        /// </summary>
        [Header("カットインに必要")]
        [SerializeField] private Canvas cutInCanvas;
        [SerializeField] private GameObject cutIn;
        
        /// <summary>
        /// 爆発パラメタ
        /// </summary>
        [Header("爆発大")]
        [SerializeField] private float bigCollisionSize = 0.25f;
        [SerializeField] private int bigDamage = 1;
        
        [Header("爆発中")]
        [SerializeField] private float mediumCollisionSize = 0.25f;
        [SerializeField] private int mediumDamage = 1;
        
        [Header("爆発小")]
        [SerializeField] private float smallCollisionSize = 0.25f;
        [SerializeField] private int smallDamage = 1;

        /// <summary>
        /// 爆発エフェクトのPrefab
        /// </summary>
        [Header("爆発Prefab")]
        [SerializeField] private GameObject explosionObject;

        public void Awake()
        {
            //デバッグ用の保険。
            instance = this;
        }

        /// <summary>
        /// カットインの呼び出し
        /// </summary>
        public GameObject GenerateCutIn()
        {
            var catIn = Instantiate(cutIn, cutInCanvas.transform);
            return catIn;
        }

        /// <summary>
        /// 爆発生成(InterfaceでSwitch変わりしたほうがいいけどプランナーに調節させたらでええわ。）
        /// </summary>
        public async void Factory(Vector3 point, int explosionPower)
        {
            try
            {
                GameObject gameObject = null;

                var vec = new Vector3(point.x, point.y, 0);
                switch (explosionPower)
                {
                    case 0:
                        AudioManager.Instance().LoadSoundEffect("ExplosionSmall");
                        PlayExplosionAnimation(vec, smallCollisionSize, smallDamage, "SmallExplosion");
                        break;
                    case 1:
                        AudioManager.Instance().LoadSoundEffect("ExplosionMiddle");
                        PlayExplosionAnimation(vec, mediumCollisionSize, mediumDamage, "MediumExplosion");
                        break;
                    case 2:
                        AudioManager.Instance().LoadSoundEffect("ExplosionLarge");
                        PlayExplosionAnimation(vec, bigCollisionSize, bigDamage, "BigExplosion");
                        break;
                }
            }
            catch (Exception e)
            {
                // キャンセル処理
            }
        }

        /// <summary>
        /// 爆発のアニメーション再生
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="collisionSize"></param>
        /// <param name="damage"></param>
        /// <param name="animationName"></param>
        private async void PlayExplosionAnimation(
            Vector3 vector, 
            float collisionSize, 
            int damage,
            string animationName)
        {
            var gameObject = Instantiate(explosionObject, vector, Quaternion.identity);
            var explosionAttach = gameObject.GetComponent<ExplosionAttach>();
            var explosionCollider = gameObject.GetComponent<CircleCollider2D>();
            explosionCollider.radius = collisionSize;
            explosionAttach.SetDamage(damage);
            await explosionAttach.Explosion(animationName);
        }
    }
}