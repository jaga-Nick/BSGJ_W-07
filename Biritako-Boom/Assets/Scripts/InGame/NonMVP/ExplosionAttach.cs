using Cysharp.Threading.Tasks;
using InGame.Model;
using System;
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
        private int _damage;
        private Animator _animator;

        private CancellationTokenSource _cancellationTokenSource;
        
        /// <summary>
        /// ゲームオブジェクトを爆発させる
        /// </summary>
        /// <param name="animationName"></param>
        public async UniTask Explosion(string animationName)
        {
            try
            {
                var token = _cancellationTokenSource.Token;
                var linked = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
                var linkedToken = linked.Token;

                // Animatorの取得
                _animator = gameObject.GetComponent<Animator>();
                // アニメーションの再生
                _animator.Play(animationName);
                // アニメーションが終わるまで待機
                await UniTask.WaitUntil(() => {
                    var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
                },cancellationToken:linkedToken);
                // アニメーションが終わったら爆発のオブジェクトを破棄
                Destroy(gameObject);
            }
            catch (OperationCanceledException)
            {

            }
        }

        public void SetDamage(int num)
        {
            _damage = num;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("ここで一応プレイヤーかIEnemyかの処理")
            IEnemyModel enemies = collision.GetComponents<MonoBehaviour>().OfType<IEnemyModel>().FirstOrDefault();
            if(enemies != null) 
            {
                enemies.OnDamage(_damage); 
            }
        }
    }
}
