using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.NonMVP
{
    public class CutInAttach : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            //生成
            ActCutIn().Forget();
        }

        private Animator animator;
        public async UniTask ActCutIn()
        {
            animator = gameObject.GetComponent<Animator>();
            animator.Play("CutInBumb");
            await UniTask.WaitUntil(() => {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName("CutInBumb") && stateInfo.normalizedTime >= 1f;
            });
            Destroy(gameObject);
        }
    }
}