using System.Collections.Generic;
using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// Transformの揺れを処理するコンポーネントです。
    /// </summary>
    [AddComponentMenu("Shake Effect/Shaker")]
    public class Shaker : MonoBehaviour
    {
        /// <summary>
        /// Shakerの静的リスト。
        /// "グローバルシェイカーに追加"がtrueの場合、Awake時にこのリストに追加されます。
        /// </summary>
        public static List<Shaker> GlobalShakers = new List<Shaker>();

        /// <summary>
        /// 全てのグローバルShakerを揺らします。
        /// </summary>
        public static ShakeInstance ShakeAll(IShakeParameters shakeData, int? seed = null)
        {
            ShakeInstance shakeInstance = new ShakeInstance(shakeData, seed);
            AddShakeAll(shakeInstance);
            return shakeInstance;
        }

        /// <summary>
        /// 各Shakerに個別のShakeInstanceを使用して、全てのグローバルShakerを揺らします。
        /// </summary>
        public static void ShakeAllSeparate(IShakeParameters shakeData, List<ShakeInstance> shakeInstances = null, int? seed = null)
        {
            if (shakeInstances != null)
                shakeInstances.Clear();

            for (int i = 0; i < GlobalShakers.Count; i++)
            {
                if (!GlobalShakers[i].gameObject.activeInHierarchy)
                    continue;

                ShakeInstance shakeInstance = GlobalShakers[i].Shake(shakeData, seed);

                if (shakeInstances != null && shakeInstance != null)
                    shakeInstances.Add(shakeInstance);
            }
        }

        /// <summary>
        /// 位置と距離を使用してシェイクの強度を調整し、全てのグローバルShakerでシェイクを開始します。
        /// </summary>
        public static void ShakeAllFromPoint(Vector3 point, float maxDistance, IShakeParameters shakeData, List<ShakeInstance> shakeInstances = null, int? seed = null)
        {
            if (shakeInstances != null)
                shakeInstances.Clear();

            for (int i = 0; i < GlobalShakers.Count; i++)
            {
                if (!GlobalShakers[i].gameObject.activeInHierarchy)
                    continue;

                ShakeInstance shakeInstance = GlobalShakers[i].ShakeFromPoint(point, maxDistance, shakeData, seed);

                if (shakeInstances != null && shakeInstance != null)
                    shakeInstances.Add(shakeInstance);
            }
        }
        
        /// <summary>
        /// 既存のShakeInstanceを全てのグローバルShakerに追加します。
        /// </summary>
        public static void AddShakeAll(ShakeInstance shakeInstance)
        {
            for (int i = 0; i < GlobalShakers.Count; i++)
            {
                if (!GlobalShakers[i].gameObject.activeInHierarchy)
                    continue;

                GlobalShakers[i].AddShake(shakeInstance);
            }
        }

        [SerializeField]
        private bool addToGlobalShakers;

        private List<ShakeInstance> activeShakes = new List<ShakeInstance>();

        private void Awake()
        {
            if (addToGlobalShakers)
                GlobalShakers.Add(this);
        }

        private void OnDestroy()
        {
            if (addToGlobalShakers)
                GlobalShakers.Remove(this);
        }

        private void Update()
        {
            ShakeResult shake = new ShakeResult();

            for (int i = activeShakes.Count - 1; i >= 0; i--)
            {
                if (activeShakes[i].IsFinished())
                {
                    activeShakes.RemoveAt(i);
                    continue;
                }
                shake += activeShakes[i].UpdateShake(Time.deltaTime);
            }

            transform.localPosition = shake.PositionShake;
            transform.localEulerAngles = shake.RotationShake;
        }

        /// <summary>
        /// シェイクを開始します。
        /// </summary>
        public ShakeInstance Shake(IShakeParameters shakeData, int? seed = null)
        {
            ShakeInstance shakeInstance = new ShakeInstance(shakeData, seed);
            AddShake(shakeInstance);
            return shakeInstance;
        }

        /// <summary>
        /// 位置と距離を使用してシェイクの強度を調整し、シェイクを開始します。
        /// </summary>
        public ShakeInstance ShakeFromPoint(Vector3 point, float maxDistance, IShakeParameters shakeData, int? seed = null)
        {
            float distance = Vector3.Distance(transform.position, point);

            if (distance < maxDistance)
            {
                ShakeInstance shakeInstance = new ShakeInstance(shakeData, seed);
                float scale = 1 - Mathf.Clamp01(distance / maxDistance);
                shakeInstance.SetStrengthScale(scale);
                shakeInstance.SetRoughnessScale(scale);
                AddShake(shakeInstance);
                return shakeInstance;
            }

            return null;
        }

        /// <summary>
        /// 既存のShakeInstanceをこのShakerのアクティブなシェイクリストに追加します。
        /// </summary>
        public void AddShake(ShakeInstance shakeInstance)
        {
            activeShakes.Add(shakeInstance);
        }
    }
}