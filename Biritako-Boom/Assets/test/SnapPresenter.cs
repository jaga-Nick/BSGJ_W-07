using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

// using System.IO; // このクラスでは直接使用しなくなりました

/// <summary>
/// スクリーンショット撮影のロジックを制御し、ScreenSnapShotクラスとの連携を行います。
/// ゲーム内の特定のタイミングで撮影を指示する役割を担います。
/// </summary>
public class SnapPresenter : MonoBehaviour
{
    [FormerlySerializedAs("screenshotHelper")]
    [Header("コンポーネント参照")]
    [SerializeField]
    private ScreenSnapShot screenSnapShot = new ScreenSnapShot();
    [SerializeField]
    private Camera mainCamera;

    [Header("スクリーンショット基本設定")]
    [SerializeField]
    private LayerMask uiLayersToExclude = 0; // 除外するUIレイヤーマスク
    [SerializeField]
    private string fileName = "result"; // 保存ファイル名

    [FormerlySerializedAs("editorOnlyRelativeSavePath")]
    [Header("エディタ実行時 設定")]
    [Tooltip("【エディタ実行時のみ】プロジェクトルートからの相対保存ディレクトリパスを指定します。\n例: \"MyGame_Screenshots\" や \"../Temporary/EditorShots\"\n空の場合、ScreenSnapShotがユーザーにフォルダ選択を促すか、デフォルトパスを使用します。\n【ビルド時】この設定は無視されます。")]
    [SerializeField]
    private string SavePath;

    void Start()
    {
        
        if (screenSnapShot == null)
        {
            Debug.LogError("[SnapPresenter] ScreenshotHelper (ScreenSnapShotコンポーネント) がアタッチされていません！", this);
            enabled = false;
            return;
        }
        if (mainCamera == null)
        {
            Debug.LogError("[SnapPresenter] Main Game Camera (撮影用カメラ) がアタッチされていません！", this);
            enabled = false;
            return;
        }
        
        screenSnapShot.Initialize(
            mainCamera,
            uiLayersToExclude,
            fileName, 
            SavePath
        );
    }

    public async void OnClick()
    {
        if (screenSnapShot == null || !screenSnapShot.IsInitialized)
        {
            Debug.LogError("[SnapPresenter] ScreenSnapShotが準備できていないか、初期化されていません。スクリーンショットは撮影できません。", this);
            return;
        }

        // ScreenSnapShotの撮影メソッドを引数なしで呼び出す
        string filePath = await screenSnapShot.CaptureScreenshotAsync();

        if (!string.IsNullOrEmpty(filePath))
        {
            Debug.Log($"[SnapPresenter] スクリーンショットの撮影に成功しました: {filePath}");
            // TODO: リザルト画面に表示する処理などをここに記述
        }
        else
        {
            Debug.LogError("[SnapPresenter] スクリーンショットの撮影に失敗しました。", this);
        }
    }
}