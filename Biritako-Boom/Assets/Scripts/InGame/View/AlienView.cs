using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// エイリアンの見た目を担当するクラス。
    /// </summary>
    public class AlienView : MonoBehaviour
    {
        private Animator _animator;





        /// <summary>
        /// プールに戻る際に呼び出される処理。
        /// </summary>
        public void OnReturnToPool()
        {
            // ここで消滅エフェクトの再生などを行う
        }
    }
}