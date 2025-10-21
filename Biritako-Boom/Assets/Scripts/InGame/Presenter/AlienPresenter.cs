using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
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
        // --- 既存のインスタンスメンバ ---
        // 自身が担当するModel
        public AlienModel Model { get; private set; }

        // 自身が担当するView
        public AlienView View { get; private set; }
        // 自分を管理するManagerへの参照
        // private AlienManager _manager;
        // (統合) Manager への参照は不要。プールは静的に管理する。

        // --- 静的プール管理（AlienManager の機能を統合） ---
        private static readonly Queue<AlienPresenter> _pool = new Queue<AlienPresenter>();
        private static readonly List<AlienPresenter> _activeAliens = new List<AlienPresenter>();
        private static GameObject _alienPrefab;
        private static bool _isInitialized;
        // デフォルト設定（必要に応じて変更できます）
        private static int _initialPoolSize = 50;
        private static string _characterAddress = "Enemy_Alien";

        // Unityイベント: オブジェクトが有効になる前に一度だけ呼ばれる
        private void Awake()
        {
            // --- 自身が持つ各コンポーネントの参照を取得 ---
            Model = GetComponent<AlienModel>();
            View = GetComponent<AlienView>();

            Model.SetRigidbody(GetComponent<Rigidbody2D>());
            View.SetSprite(GetComponent<SpriteRenderer>());


            // --- Modelのイベントを購読 ---
            // OnDamaged は現在プロジェクト内でハンドリングされていないため購読しない
            Model.OnReturnedToPool += OnReturnedToPool;
        }
        
        // 古い Manager 初期化は不要。代わりに静的な初期化を用いる。
        /// <summary>
        /// Addressables からプレハブをロードし、プールを初期化します（静的）。
        /// </summary>
        public static async UniTask InitializePool(int initialPoolSize = 50, string characterAddress = null)
        {
            if (_isInitialized) return;

            if (!string.IsNullOrEmpty(characterAddress)) _characterAddress = characterAddress;
            _initialPoolSize = initialPoolSize;

            // Addressables からプレハブをロード
            _alienPrefab = await Addressables.LoadAssetAsync<GameObject>(_characterAddress).ToUniTask();

            // 初期プール生成
            for (int i = 0; i < _initialPoolSize; i++)
            {
                GeneratePoolAlien();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// プールから一体を取り出して出現させる（静的）。
        /// </summary>
        public static void SpawnAlien(Vector3 position, int initialHp)
        {
            if (_pool.Count == 0) return;

            var presenter = _pool.Dequeue();
            presenter.transform.position = position;
            presenter.Model.SetInitialState(initialHp);
            presenter.gameObject.SetActive(true);
            _activeAliens.Add(presenter);
        }

        /// <summary>
        /// 指定されたエイリアンをプールに戻す（静的）。
        /// </summary>
        public static void ReturnAlien(AlienPresenter presenter)
        {
            if (presenter == null) return;
            if (_activeAliens.Remove(presenter))
            {
                presenter.gameObject.SetActive(false);
                _pool.Enqueue(presenter);
            }
        }

        /// <summary>
        /// Addressables からロードしたプレハブを解放する（静的クリーンナップ）。
        /// </summary>
        public static UniTask OnDestroyAsync()
        {
            if (_alienPrefab != null)
            {
                Addressables.Release(_alienPrefab);
                _alienPrefab = null;
            }

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// プール用に1体生成する内部ヘルパー（静的）。
        /// </summary>
        private static void GeneratePoolAlien()
        {
            var alienObj = GameObject.Instantiate(_alienPrefab);
            var presenter = alienObj.GetComponent<AlienPresenter>();
            // Awake 内で必要なセットアップが行われるため、ここでは Presenter をキューするだけ
            alienObj.SetActive(false);
            _pool.Enqueue(presenter);
        }
         
        private void OnReturnedToPool()
        {
            View.OnReturnToPool();
            // 統合された静的プール管理に返却
            ReturnAlien(this);
        }
    }
}