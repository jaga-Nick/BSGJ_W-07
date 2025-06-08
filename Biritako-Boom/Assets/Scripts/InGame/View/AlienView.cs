using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// エイリアンの見た目を担当するクラス。
    /// Presenterからの指示を受けて、アニメーションやエフェクトを再生する。
    /// </summary>
    public class AlienView : MonoBehaviour
    {
        // Unityエディタから設定するAnimatorコンポーネント
        [SerializeField] private Animator _animator;
        // Unityエディタから設定するヒットエフェクトのParticleSystem
        [SerializeField] private ParticleSystem _hitEffect;

        /// <summary>
        /// ヒットアニメーションを再生します。
        /// </summary>
        public void PlayHitAnimation()
        {
            // Animatorに"Hit"というトリガーをセットしてアニメーションを起動
            //_animator?.SetTrigger("Hit");
        }

        /// <summary>
        /// ヒットエフェクトを再生します。
        /// </summary>
        public void PlayHitEffect()
        {
            // パーティクルシステムが設定されていれば再生
            //_hitEffect?.Play();
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