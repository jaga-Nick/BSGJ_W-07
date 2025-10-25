using UnityEngine;

namespace Common.GameSystem
{
    /// <summary>
    /// 破壊可能シングルトン
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DestroyAvailableSingletonMonoBehaviourBase<T> : MonoBehaviour where T : DestroyAvailableSingletonMonoBehaviourBase<T>
    {
        /// <summary>
        /// 静的な変数
        /// </summary>
        protected static T instance;

        /// <summary>
        /// 本体の取得
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            // なければ、GameObjectで生成しDontdestroyに移動させる
            if (instance == null)
            {
                var gameObject = new GameObject(typeof(T).Name);
                instance = gameObject.AddComponent<T>();
            }
            return instance;
        }
    }
}
