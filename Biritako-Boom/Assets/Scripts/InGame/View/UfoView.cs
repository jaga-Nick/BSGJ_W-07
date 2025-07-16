using System.Collections;
using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// UFO（Enemy）のViewの管理クラス
    /// </summary>
    public class UfoView : MonoBehaviour
    {
        private Animator _animator;
        
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        
        /// <summary>
        /// 動くときのアニメーションの再生
        /// </summary>
        /// <param name="isMoving"></param>
        public void PlayMoveAnimation(bool isMoving)
        {
            if (_animator)
            {
                _animator.SetBool(IsMoving, isMoving);
            }
        }
    }
}