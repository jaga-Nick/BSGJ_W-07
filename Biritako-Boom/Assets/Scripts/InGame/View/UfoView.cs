using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// UFO（Enemy）のViewの管理クラス
    /// </summary>
    public class UfoView : MonoBehaviour
    {
        private Animator animator;
        
        private static readonly int isMoving = Animator.StringToHash("isMoving");
        
        /// <summary>
        /// 動くときのアニメーションの再生
        /// </summary>
        /// <param name="isMoving"></param>
        public void PlayMoveAnimation(bool isMoving)
        {
            if (animator)
            {
                animator.SetBool(UfoView.isMoving, isMoving);
            }
        }
    }
}