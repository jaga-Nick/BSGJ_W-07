using UnityEngine;


namespace Common
{
    public class SingletonMonoBehaviourBase<T> : MonoBehaviour where T : SingletonMonoBehaviourBase<T>
    {
        protected static T instance;

        public static T Instance(bool destroy = false)
        {
            if (instance == null)
            {
                var gameObject = new GameObject(typeof(T).Name);
                instance = gameObject.AddComponent<T>();
                if (destroy == false)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            return instance;
        }
    }
}
