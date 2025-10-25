using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace InGame.NonMVP
{
    public class EnemyEffectAttach : MonoBehaviour
    {
        [FormerlySerializedAs("_name")] [SerializeField]
        private String name;
        void Awake()
        {
            //生成
            ActEffect().Forget();
        }

        private Animator _animator;
        public async UniTask ActEffect()
        {
            _animator = gameObject.GetComponent<Animator>();
            _animator.Play(name);
            await UniTask.WaitUntil(() => {
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName(name) && stateInfo.normalizedTime >= 1f;
            });
            Destroy(gameObject);
        }
    }
}