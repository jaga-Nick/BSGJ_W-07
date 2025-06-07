using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// 家電（Enemy）のViewの管理クラス
    /// </summary>
    public class ElectronicsView : MonoBehaviour
    {
        private Animator _animator;
        
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        /// <summary>
        /// 座標の更新
        /// </summary>
        /// <param name="newPosition"></param>
        public void UpdatePosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        /// <summary>
        /// アニメーションの再生
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