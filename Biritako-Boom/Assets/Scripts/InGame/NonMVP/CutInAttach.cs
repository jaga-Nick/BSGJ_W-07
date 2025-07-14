using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.NonMVP
{
    public class CutInAttach : MonoBehaviour
    {
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