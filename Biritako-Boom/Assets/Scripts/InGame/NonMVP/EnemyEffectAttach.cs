using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace InGame.NonMVP
{
    public class EnemyEffectAttach : MonoBehaviour
    {
        [SerializeField]
        private String _name;
        void Awake()
        {
            //生成
            ActEffect().Forget();
        }

        private Animator animator;
        public async UniTask ActEffect()
        {
            animator = gameObject.GetComponent<Animator>();
            animator.Play(_name);
            await UniTask.WaitUntil(() => {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName(_name) && stateInfo.normalizedTime >= 1f;
            });
            Destroy(gameObject);
        }
    }
}