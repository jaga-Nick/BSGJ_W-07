using UnityEngine;
using InGame.Model;
using InGame.View;

namespace InGame.Presenter
{
    /// <summary>
    /// ModelとViewを仲介するPresenter。
    /// 自身がアタッチされたGameObjectのコンポーネント間の設定も行う。
    /// </summary>
    [RequireComponent(typeof(AlienModel), typeof(AlienView), typeof(Rigidbody2D))]
    public class AlienPresenter : MonoBehaviour
    {
        // 自身が担当するModel。publicプロパティで外部(Manager)に公開する
        public IEnemyModel Model { get; private set; }

        // 自身が担当するView
        private AlienView _view;
        // 自分を管理するManagerへの参照
        private AlienManager _manager;

        // Unityイベント: オブジェクトが有効になる前に一度だけ呼ばれる
        private void Awake()
        {
            // --- 自身が持つ各コンポーネントの参照を取得 ---
            var modelComponent = GetComponent<AlienModel>();
            _view = GetComponent<AlienView>();
            var rb = GetComponent<Rigidbody2D>();

            // --- Modelに必要な参照を設定（注入）する ---
            modelComponent.SetRigidbody(rb);

            // --- PresenterがModelとして保持するのはインターフェース型 ---
            this.Model = modelComponent;

            // --- Modelのイベントを購読 ---
            // Modelの型をキャストしてイベントにアクセス
            ((AlienModel)this.Model).OnDamaged += OnDamaged;
            ((AlienModel)this.Model).OnReturnedToPool += OnReturnedToPool;
        }
        
        // (OnCollisionEnter2D, OnDestroyは前回同様)
        
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
            _view.PlayHitEffect();
        }


        private void OnReturnedToPool()
        {
            _view.OnReturnToPool();
            _manager?.ReturnAlien(this);
        }
    }
}