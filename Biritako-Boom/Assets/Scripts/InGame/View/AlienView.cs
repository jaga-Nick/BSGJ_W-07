using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// エイリアンの見た目を担当するクラス。
    /// </summary>
    public class AlienView : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;


        public void SetSprite(SpriteRenderer spriteRenderer)
        {
            this._spriteRenderer = spriteRenderer;
        }

        /// <summary>
        /// Alienの向きを進行方向に合わせて変える
        /// </summary>
        public void FlipAlien(bool isRight)
        {
            if (isRight)
            {
                _spriteRenderer.flipX = false; // 右向き
            }
            else
            {
                _spriteRenderer.flipX = true; // 左向き
            }
        }


        /// <summary>
        /// プールに戻る際に呼び出される処理。
        /// </summary>
        public void OnReturnToPool()
        {
            // ここで消滅エフェクトの再生などを行う
        }
    }
}