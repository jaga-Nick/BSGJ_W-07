using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Ending.Presenter
{
    public class EndingPresenter : MonoBehaviour
    {
        [Header("UI / Image")]
        [Tooltip("一枚絵を表示するImageコンポーネント")]
        [SerializeField] private Image _image;

        [Tooltip("リザルト画面のUIオブジェクト")]
        [SerializeField] private GameObject resultUI;

        [Header("スプライト設定")]
        [Tooltip("表示する一枚絵のスプライト配列")]
        [SerializeField] private Sprite[] endingSprites;

        [Header("表示タイミング設定")]
        [Tooltip("各絵の表示時間（秒）")]
        [SerializeField] private float displayTime = 2.0f;

        [Tooltip("最後の絵の表示時間（秒）")]
        [SerializeField] private float lastImageDisplayTime = 4.0f;

        [Tooltip("絵が切り替わる際のディゾルブ（フェード）時間（秒）")]
        [SerializeField] private float fadeDuration = 1.0f;

        async void Start()
        {
            // --- 初期設定 ---
            resultUI.SetActive(false);

            // スプライトが設定されていない場合は、何もしないで終了
            if (endingSprites == null || endingSprites.Length == 0)
            {
                Debug.LogWarning("表示するエンディングスプライトが設定されていません。");
                // すぐにリザルトを表示するなどのフォールバック処理
                resultUI.SetActive(true); 
                return;
            }

            // 最初の画像をフェードインで表示
            _image.sprite = endingSprites[0];
            await FadeAsync(1f, fadeDuration, this.GetCancellationTokenOnDestroy());

            // --- 紙芝居ループ ---
            for (int i = 0; i < endingSprites.Length; i++)
            {
                bool isLastImage = (i == endingSprites.Length - 1);

                // 現在の画像を指定時間だけ待機して表示
                float currentWaitTime = isLastImage ? lastImageDisplayTime : displayTime;
                await UniTask.Delay(TimeSpan.FromSeconds(currentWaitTime), cancellationToken: this.GetCancellationTokenOnDestroy());

                // もし最後の画像なら、ループを抜けてリザルト表示へ
                if (isLastImage)
                {
                    break;
                }

                // --- ディゾルブ切り替え処理 ---
                // 1. 現在の画像をフェードアウト
                await FadeAsync(0f, fadeDuration, this.GetCancellationTokenOnDestroy());

                // 2. 次のスプライトに差し替え
                _image.sprite = endingSprites[i + 1];

                // 3. 新しい画像をフェードイン
                await FadeAsync(1f, fadeDuration, this.GetCancellationTokenOnDestroy());
            }

            // --- 終了処理 ---
            resultUI.SetActive(true);
        }

        /// <summary>
        /// 指定したアルファ値に、指定時間をかけてImageをフェードさせる
        /// </summary>
        /// <param name="targetAlpha">目標のアルファ値 (0=透明, 1=不透明)</param>
        /// <param name="duration">フェードにかける時間</param>
        /// <param name="cancellationToken">キャンセル用トークン</param>
        private async UniTask FadeAsync(float targetAlpha, float duration, CancellationToken cancellationToken)
        {
            Color color = _image.color;
            float startAlpha = color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // 経過時間から現在のアルファ値を計算
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                color.a = newAlpha;
                _image.color = color;

                // 1フレーム待機
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsedTime += Time.deltaTime;
            }

            // 最終的なアルファ値を確実に設定
            color.a = targetAlpha;
            _image.color = color;
        }
    }
}