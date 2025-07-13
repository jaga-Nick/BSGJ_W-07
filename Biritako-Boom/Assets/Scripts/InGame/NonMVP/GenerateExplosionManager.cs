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
        [SerializeField] private float bigExplosionSize = 0.25f;
        [SerializeField] private int bigDamage = 1;
        
        [Header("爆発中")]
        [SerializeField] private float middleCollisionSize = 0.25f;
        [SerializeField] private int middleDamage = 1;
        
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
        public void GenerateCutIn()
        {
            Instantiate(cutIn, cutInCanvas.transform);
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
                        gameObject = Instantiate(explosionObject, vec, Quaternion.identity);
                        var smallExplosionAttach = gameObject.GetComponent<ExplosionAttach>();
                        var smallCollider = gameObject.GetComponent<CircleCollider2D>();
                        smallCollider.radius = smallCollisionSize;
                        smallExplosionAttach.SetDamage(smallDamage);
                        await smallExplosionAttach.Explosion("SmallExplosion");
                        break;
                    case 1:
                        gameObject = Instantiate(explosionObject, vec, Quaternion.identity);
                        var mediumExplosionAttach = gameObject.GetComponent<ExplosionAttach>();
                        var mediumCollider = gameObject.GetComponent<CircleCollider2D>();
                        mediumCollider.radius = smallCollisionSize;
                        mediumExplosionAttach.SetDamage(middleDamage);
                        await mediumExplosionAttach.Explosion("MediumExplosion");
                        break;
                    case 2:
                        gameObject = Instantiate(explosionObject, vec, Quaternion.identity);
                        var bigCollider = gameObject.GetComponent<CircleCollider2D>();
                        bigCollider.radius = smallCollisionSize;
                        var bigExplosionAttach = gameObject.GetComponent<ExplosionAttach>();
                        bigExplosionAttach.SetDamage(bigDamage);
                        await bigExplosionAttach.Explosion("BigExplosion");
                        break;
                }
            }
            catch (Exception e)
            {
                // キャンセル処理
            }
        }
    }
}