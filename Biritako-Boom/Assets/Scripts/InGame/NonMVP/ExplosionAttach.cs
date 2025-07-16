using Cysharp.Threading.Tasks;
using InGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// 爆発Animationを取得し消滅するだけ。
    /// </summary>
    public class ExplosionAttach : MonoBehaviour
    {
        private int Damage;
        private Animator animator;

        private CancellationTokenSource token;
        public async UniTask Explosion(string AnimationName)
        {
            try
            {
                CancellationToken t=token.Token;
                var linked =CancellationTokenSource.CreateLinkedTokenSource(t, this.GetCancellationTokenOnDestroy());
                var linkedtoken = linked.Token;

                animator = gameObject.GetComponent<Animator>();
                animator.Play(AnimationName);
                await UniTask.WaitUntil(() => {
                    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.IsName(AnimationName) && stateInfo.normalizedTime >= 1f;
                },cancellationToken:linkedtoken);
                Destroy(gameObject);
            }
            catch (OperationCanceledException)
            {

            }
            
        }

        public void SetDamage(int num)
        {
            Damage = num;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("ここで一応プレイヤーかIEnemyかの処理")
            IEnemyModel enemies = collision.GetComponents<MonoBehaviour>().OfType<IEnemyModel>().FirstOrDefault();
            if(enemies != null) 
            {
                enemies.OnDamage(Damage); 
            }
        }
    }
}
