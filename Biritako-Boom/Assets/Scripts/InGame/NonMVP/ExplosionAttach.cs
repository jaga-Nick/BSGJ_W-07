using Cysharp.Threading.Tasks;
using InGame.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// 爆発Animationを取得し消滅するだけ。
    /// </summary>
    public class ExplosionAttach : MonoBehaviour
    {
        private int Damage=0;


        private Animator animator;
        public async UniTask Explosion(string AnimationName)
        {
            animator = gameObject.GetComponent<Animator>();
            animator.Play(AnimationName);
            await UniTask.WaitUntil(() => {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName(AnimationName) && stateInfo.normalizedTime >= 1f;
            });
            Destroy(gameObject);
        }

        public void SetDamage()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("ここで一応プレイヤーかIEnemyかの処理")
            IEnemyModel enemies = collision.GetComponents<MonoBehaviour>().OfType<IEnemyModel>().FirstOrDefault();

            enemies.OnDamage(Damage);
            //ここでIEnemhyModelの処理を呼び出す様にする
            //死亡確認
            //enemies?.IsDead();
        }
    }
}
