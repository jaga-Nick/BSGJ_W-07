using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;

public class ResultScreenView : MonoBehaviour // Viewとしての役割を明示
{
    public RawImage screenshotDisplay; // スクリーンショットを表示するRawImage

    /// <summary>
    /// 指定されたパスのスクリーンショットを非同期で読み込み、表示します。
    /// </summary>
    /// <param name="imagePath">読み込む画像のフルパス</param>
    public async UniTask LoadAndDisplayScreenshotAsync(string imagePath)
    {
        if (screenshotDisplay == null)
        {
            Debug.LogError("[ResultScreenView] ScreenshotDisplay (RawImage) is not assigned.");
            return;
        }

        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
        {
            Debug.LogError($"[ResultScreenView] Screenshot file not found or path is invalid: {imagePath}");
            screenshotDisplay.texture = null; // またはデフォルト画像を表示
            screenshotDisplay.gameObject.SetActive(false); // 画像がない場合は非表示にするなど
            return;
        }

        byte[] pngData = null;
        try
        {
            // ファイル読み込みをワーカースレッドで非同期に実行
            pngData = await UniTask.RunOnThreadPool(() => File.ReadAllBytes(imagePath));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ResultScreenView] Failed to read screenshot file at {imagePath}: {e.Message}");
            screenshotDisplay.texture = null;
            screenshotDisplay.gameObject.SetActive(false);
            return;
        }

        if (pngData == null || pngData.Length == 0)
        {
            Debug.LogError("[ResultScreenView] Screenshot data is empty after reading file.");
            screenshotDisplay.texture = null;
            screenshotDisplay.gameObject.SetActive(false);
            return;
        }

        // Texture2Dへの変換 (メインスレッドで実行する必要がある)
        // Texture2DのコンストラクタやLoadImageはメインスレッドで行う
        Texture2D tex = new Texture2D(2, 2); // 初期サイズはダミー。LoadImageで適切なサイズに変更される。
        if (tex.LoadImage(pngData)) // PNG/JPGデータをTexture2Dにロード
        {
            // 以前のテクスチャがあれば破棄 (メモリリーク対策)
            if (screenshotDisplay.texture != null && screenshotDisplay.texture != tex)
            {
                Destroy(screenshotDisplay.texture);
            }
            screenshotDisplay.texture = tex;
            screenshotDisplay.gameObject.SetActive(true);
            // RawImageのアスペクト比をテクスチャに合わせる (任意)
            // screenshotDisplay.GetComponent<AspectRatioFitter>()?.aspectRatio = (float)tex.width / tex.height;
        }
        else
        {
            Debug.LogError("[ResultScreenView] Failed to load texture from png data.");
            Destroy(tex); // 失敗したら作成したTexture2Dを破棄
            screenshotDisplay.texture = null;
            screenshotDisplay.gameObject.SetActive(false);
        }
    }

    public void ClearScreenshot()
    {
        if (screenshotDisplay != null && screenshotDisplay.texture != null)
        {
            Destroy(screenshotDisplay.texture);
            screenshotDisplay.texture = null;
            screenshotDisplay.gameObject.SetActive(false);
        }
    }
}