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


        /// <summary>
        /// 停止するときのアニメーションの再生
        /// </summary>
        /// <param name="frequencyAmplitude"></param>
        /// <param name="frequencySpeed"></param>
        /// <param name="initialPosition"></param>
        /// <returns></returns>
        public IEnumerator PlayStopAnimation(float frequencyAmplitude, float frequencySpeed, Vector3 initialPosition)
        {
            var timer = 0f;
            while (timer < 3.0f)
            {
                var yOffset = Mathf.Sin(timer * frequencySpeed) * frequencyAmplitude;
                transform.position = new Vector3(initialPosition.x, initialPosition.y + yOffset, initialPosition.z);
                
                timer += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 死んだらオブジェクトを破壊する
        /// </summary>
        public void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}