using UnityEngine;


namespace Common
{
    public class SingletonMonoBehaviourBase<T> : MonoBehaviour where T : SingletonMonoBehaviourBase<T>
    {
        protected static T instance;

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            if (instance == null)
            {
                var gameObject = new GameObject(typeof(T).Name);
                instance = gameObject.AddComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
    }
}
