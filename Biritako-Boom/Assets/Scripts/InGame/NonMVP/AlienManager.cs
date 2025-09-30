using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets; // Addressablesの機能を使うために必要
using Cysharp.Threading.Tasks;
using InGame.Presenter;
using Random = UnityEngine.Random;


namespace InGame.NonMVP
{
    /// <summary>
    /// エイリアンの生成、更新、オブジェクトプールを管理するクラス。
    /// </summary>
    public class AlienManager : MonoBehaviour
    {
        [Header("プール設定")]
        [Tooltip("Addressablesに設定したエイリアンのアドレス")]
        [SerializeField] private string characterAddress = "Enemy_Alien"; // ここでキーを指定

        [Header("エイリアン最大数")]
        [SerializeField] private int initialPoolSize = 50;

        [Header("マップ左下")]
        [SerializeField] private Vector2 spawnAreaMin = new Vector2(-20f, -20f);
        [Header("マップ右上")]
        [SerializeField] private Vector2 spawnAreaMax = new Vector2(20f, 20f);

        // プール（待機中のエイリアン）を管理するキュー
        private readonly Queue<AlienPresenter> pool = new Queue<AlienPresenter>();
        // 活動中のエイリアンを管理するリスト
        private readonly List<AlienPresenter> activeAliens = new List<AlienPresenter>();

        // Addressablesからロードしたプレハブの実体
        private GameObject alienPrefab;
        // 初期化が完了したかどうかのフラグ
        private bool isInitialized = false;


        private void Start()
        {
            InitializePool();

            // SpawnTest().Forget();

        }

        private void Update()
        {
            for (int i = activeAliens.Count - 1; i >= 0; i--)
            {
                var presenter = activeAliens[i];
                presenter.model.Move();
                presenter.view.FlipAlien(presenter.model.GetFlip());
            }


        }

        /// <summary>
        /// オブジェクトが破棄される時に呼ばれる
        /// </summary>
        private void OnDestroy()
        {
            // Managerが破棄される際に、ロードしたAddressableアセットを解放する
            if (alienPrefab != null)
            {
                Addressables.Release(alienPrefab);
            }
        }

        /// <summary>
        /// 文字列キーを使用してAddressablesからプレハブをロードし、プールを非同期で初期化。
        /// </summary>
        public async UniTask InitializePool()
        {
            // 既に初期化済みなら処理を中断
            if (isInitialized) return;


            // 文字列のキーを使ってAddressablesからプレハブを非同期でロード
            alienPrefab = await Addressables.LoadAssetAsync<GameObject>(characterAddress).ToUniTask();

            // 初期プールサイズ分だけ、あらかじめエイリアンを生成してプールしておく
            for (int i = 0; i < initialPoolSize; i++)
            {
                GeneratePoolAlien();
            }

            // 初期化完了フラグを立てる
            isInitialized = true;
        }

        /// <summary>
        /// 指定した位置にエイリアンを一体出現させます。
        /// </summary>
        public void SpawnAlien(Vector3 position, int initialHp)
        {
            // プールが空の場合、動的に新しいエイリアンを生成して補充する
            if (pool.Count == 0) return;

            // プールから待機中のPresenterを一つ取り出す
            var presenter = pool.Dequeue();
            // Presenter（GameObject）の座標を指定された位置に設定
            presenter.transform.position = position;

            // Modelの型をキャストして状態設定メソッドを呼び出す
            ((InGame.Model.AlienModel)presenter.model).SetInitialState(initialHp);

            // GameObjectをアクティブにして画面に表示
            presenter.gameObject.SetActive(true);
            // 活動中リストに追加
            activeAliens.Add(presenter);
        }

        /// <summary>
        /// 指定されたエイリアンを非アクティブ化し、プールに戻します。
        /// </summary>
        public void ReturnAlien(AlienPresenter presenter)
        {
            // 対象がnullなら何もしない
            if (presenter == null) return;

            // 活動中リストから対象を削除できたら（二重返却防止）
            if (activeAliens.Remove(presenter))
            {
                // GameObjectを非アクティブ化
                presenter.gameObject.SetActive(false);
                // プール（キュー）の末尾に戻す
                pool.Enqueue(presenter);
            }
        }

        /// <summary>
        /// エイリアンのインスタンスを一体生成し、非アクティブ状態でプールに追加する内部メソッド。
        /// </summary>
        private void GeneratePoolAlien()
        {
            // ロード済みのプレハブからGameObjectをインスタンス化。親を自分に設定。
            var alienObj = Instantiate(alienPrefab, this.transform);
            // 生成したGameObjectからPresenterコンポーネントを取得
            var presenter = alienObj.GetComponent<AlienPresenter>();
            // PresenterにManager自身を教えて初期化する
            presenter.Initialize(this);

            // 最初は非表示にしておく
            alienObj.SetActive(false);
            // プールに追加
            pool.Enqueue(presenter);
        }

        public async UniTask SpawnTest()
        {
            // このコンポーネントが有効である限りループを続ける
            while (this.isActiveAndEnabled)
            {
                // エイリアンを一体生成する
                SpawnAlien(new Vector3(3.0f, 3.0f, 0.0f), 1);

                // 10秒間待機する。
                // CancellationTokenを指定することで、このオブジェクトが破棄された時に待機を安全に中断できる
                await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }

        public Vector3 GetRandomPosition()
        {
            // 1. マップ境界内でランダムな位置を生成
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            return new Vector3(randomX, randomY, 0f);
        }
    }
}