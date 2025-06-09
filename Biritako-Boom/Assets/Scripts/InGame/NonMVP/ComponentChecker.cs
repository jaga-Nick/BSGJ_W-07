using InGame.Model;
using UnityEditor.Build.Pipeline;
using UnityEngine;

/// <summary>
/// 
/// </summary>
namespace InGame.NonMVP{
    public class ComponentChecker
    {
        /// <summary>
        /// 周囲最短距離のComponentを取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanRadius"></param>
        /// <returns></returns>
        public T CharacterCheck<T>(Vector3 TransformPosition,float scanRadius) where T :Component
        {
            Debug.Log(scanRadius);
            //2D用のオーバーラップサーチ
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TransformPosition, scanRadius);

            //
            GameObject closestObject = null;
            T closestComponent = null;
            float closestDistance = Mathf.Infinity;

            //探索
            foreach (var hitCollider in hitColliders)
            {
                GameObject obj = hitCollider.gameObject;
                //特定のスクリプトがアタッチされている状況
                T component=obj.GetComponent<T>();
                if (component == null) continue;

                float distance = Vector2.Distance(TransformPosition, obj.transform.position);

                //一番近いオブジェクトを探索
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                    //ここで取得する
                    closestComponent = component;
                }
            }
            if (closestObject != null)
            {
                //Debug.Log($"最も近いオブジェクト: {closestObject.name}（距離: {closestDistance:F2}）");
            }
            return closestComponent;
        }

        public GameObject CharacterCheckGameObject<T>(Vector3 TransformPosition, float scanRadius) where T : Component
        {
            //2D用のオーバーラップサーチ
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TransformPosition, scanRadius);

            //
            GameObject closestObject = null;
            T closestComponent = null;
            float closestDistance = Mathf.Infinity;

            //探索
            foreach (var hitCollider in hitColliders)
            {
                GameObject obj = hitCollider.gameObject;

                //特定のスクリプトがアタッチされている状況
                T component = obj.GetComponent<T>();
                if (component == null) continue;

                float distance = Vector2.Distance(TransformPosition, obj.transform.position);

                //一番近いオブジェクトを探索
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                    //ここで取得する
                    closestComponent = component;
                }
            }
            if (closestObject != null)
            {
                //Debug.Log($"最も近いオブジェクト: {closestObject.name}（距離: {closestDistance:F2}）");
            }
            return closestObject;
        }

        public T InterfaceCheck<T>(Vector3 transformPosition, float scanRadius) where T : class
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transformPosition, scanRadius);

            GameObject closestObject = null;
            T closestComponent = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hitCollider in hitColliders)
            {
                GameObject obj = hitCollider.gameObject;

                // Componentを全部取得し、Tにキャストできるものを探す
                Component[] components = obj.GetComponents<Component>();
                T target = null;

                foreach (var comp in components)
                {
                    target = comp as T;
                    if (target != null) break;
                }

                if (target == null) continue;

                float distance = Vector2.Distance(transformPosition, obj.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                    closestComponent = target;
                }
            }

            return closestComponent;
        }

        /// <summary>
        /// 家電以外は取得しない。
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public IEnemyModel FindClosestEnemyOfTypeOne(Vector3 origin, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

            IEnemyModel closestEnemy = null;
            float closestDist = Mathf.Infinity;

            foreach (var hit in hits)
            {
                if (hit.transform.position == origin) continue;
                // 複数の MonoBehaviour を取得して調べる
                var behaviours = hit.GetComponents<MonoBehaviour>();

                Debug.Log(behaviours);
                foreach (var behaviour in behaviours)
                {
                    if (behaviour is IEnemyModel enemyModel)
                    {
                        if (enemyModel.GetEnemyType() != 1) continue;

                        float dist = Vector2.Distance(origin, hit.transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestEnemy = enemyModel;
                        }
                    }
                }
            }

            return closestEnemy;
        }

        /// <summary>
        /// 家電-GameObjectを返り値にしたバージョン。
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public GameObject FindClosestEnemyOfTypeOneGameObject(Vector3 origin, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

            GameObject closestEnemyGO = null;
            float closestDist = Mathf.Infinity;

            foreach (var hit in hits)
            {
                if (hit.transform.position == origin) continue;

                var behaviours = hit.GetComponents<MonoBehaviour>();
                foreach (var behaviour in behaviours)
                {
                    if (behaviour is IEnemyModel enemyModel)
                    {
                        if (enemyModel.GetEnemyType() != 1) continue;

                        float dist = Vector2.Distance(origin, hit.transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestEnemyGO = hit.gameObject;
                        }
                    }
                }
            }

            Debug.Log(closestEnemyGO);
            return closestEnemyGO;
        }
    }
}