using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.NonMVP
{
    public class CutInAttach : MonoBehaviour
    {
        private Animator _animator;
        public async UniTask ActCutIn()
        {
            _animator = gameObject.GetComponent<Animator>();
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.Play("CutInBumb");
            await UniTask.WaitUntil(() => {
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName("CutInBumb") && stateInfo.normalizedTime >= 1f;
            });
            Destroy(gameObject);
        }
    }
}