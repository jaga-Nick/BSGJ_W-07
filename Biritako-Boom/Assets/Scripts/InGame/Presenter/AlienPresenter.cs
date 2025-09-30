using UnityEngine;
using InGame.Model;
using InGame.View;
using InGame.NonMVP;

namespace InGame.Presenter
{
    /// <summary>
    /// ModelとViewを仲介するPresenter。
    /// </summary>
    [RequireComponent(typeof(AlienModel), typeof(AlienView), typeof(Rigidbody2D))]
    public class AlienPresenter : MonoBehaviour
    {
        // 自身が担当するModel
        public AlienModel model { get; private set; }

        // 自身が担当するView
        public AlienView view { get; private set; }
        // 自分を管理するManagerへの参照
        private AlienManager manager;

        // Unityイベント: オブジェクトが有効になる前に一度だけ呼ばれる
        private void Awake()
        {
            // --- 自身が持つ各コンポーネントの参照を取得 ---
            model = GetComponent<AlienModel>();
            view = GetComponent<AlienView>();
            
            model.SetRigidbody(GetComponent<Rigidbody2D>());
            view.SetSprite(GetComponent<SpriteRenderer>());
            

            // --- Modelのイベントを購読 ---
            model.OnDamaged += OnDamaged;
            model.OnReturnedToPool += OnReturnedToPool;
        }
        
        /// <summary>
        /// Managerから呼び出され、Managerへの参照を受け取り初期化を行う。
        /// </summary>
        public void Initialize(AlienManager manager)
        {
            // Managerを保持
            this.manager = manager;
        }

        // --- イベントハンドラ ---
        private void OnDamaged()
        {

        }


        private void OnReturnedToPool()
        {
            view.OnReturnToPool();
            manager?.ReturnAlien(this);
        }
    }
}