using UnityEngine;
using InGame.Model;
using InGame.View;

namespace InGame.Presenter
{
    /// <summary>
    /// ModelとViewを仲介するPresenter。
    /// </summary>
    [RequireComponent(typeof(AlienModel), typeof(AlienView), typeof(Rigidbody2D))]
    public class AlienPresenter : MonoBehaviour
    {
        // 自身が担当するModel
        public AlienModel Model { get; private set; }

        // 自身が担当するView
        public AlienView View { get; private set; }
        // 自分を管理するManagerへの参照
        private AlienManager _manager;

        // Unityイベント: オブジェクトが有効になる前に一度だけ呼ばれる
        private void Awake()
        {
            // --- 自身が持つ各コンポーネントの参照を取得 ---
            Model = GetComponent<AlienModel>();
            View = GetComponent<AlienView>();
            
            Model.SetRigidbody(GetComponent<Rigidbody2D>());
            View.SetSprite(GetComponent<SpriteRenderer>());
            

            // --- Modelのイベントを購読 ---
            Model.OnDamaged += OnDamaged;
            Model.OnReturnedToPool += OnReturnedToPool;
        }
        
        /// <summary>
        /// Managerから呼び出され、Managerへの参照を受け取り初期化を行う。
        /// </summary>
        public void Initialize(AlienManager manager)
        {
            // Managerを保持
            _manager = manager;
        }

        // --- イベントハンドラ ---
        private void OnDamaged()
        {

        }


        private void OnReturnedToPool()
        {
            View.OnReturnToPool();
            _manager?.ReturnAlien(this);
        }
    }
}