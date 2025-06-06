using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// スクリーンショットの撮影処理、パス解決、ファイル保存を行うコアコンポーネント。
/// </summary>
public class ScreenSnapShot : MonoBehaviour
{
    private Camera _cam; // 撮影用カメラ
    private string _fileNameBase; // ベースファイル名
    private string _saveDirPath;  // 保存ディレクトリの絶対パス
    private LayerMask _uiMask;    // 除外するUIレイヤー

    private const string EXT_PNG = ".png";
    private const string BUILD_SUBDIR = "Screenshots";
    private const string EDITOR_SUBDIR = "EditorScreenshots_Default";
    private const string FALLBACK_SUBDIR = "Screenshots_Fallback";
    
    /// <summary>
    /// このコンポーネントが正常に初期化されたかどうかを示します。
    /// </summary>
    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// スクリーンショット撮影機能を初期化し、相対パスを絶対パスに変換します。
    /// </summary>
    /// <param name="camera">撮影に使用するカメラ</param>
    /// <param name="uiExcludeMask">キャプチャ時に非表示にするUIレイヤー</param>
    /// <param name="baseName">デフォルトのファイル名のベース</param>
    /// <param name="editorRelPath">
    /// 【エディタ実行時のみ使用】プロジェクトルートからの相対保存ディレクトリパスのヒント
    /// 空の場合、ユーザーに選択を促すか、デフォルトパスを使用します。
    /// </param>
    public void Initialize(Camera camera, LayerMask uiExcludeMask, string baseName, string editorRelPath = null)
    {
        _cam = camera;
        _uiMask = uiExcludeMask;
        _fileNameBase = string.IsNullOrEmpty(baseName) ? "Screenshot" : baseName;

        if (_cam == null)
        {
            Debug.LogError("[ScreenSnapShot] 撮影用カメラがnullです。初期化に失敗しました。", this);
            IsInitialized = false;
            return;
        }

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(editorRelPath))
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                _saveDirPath = Path.Combine(projectRoot, editorRelPath);
                Debug.Log($"[ScreenSnapShot] エディタモード: 指定された相対パス \"{editorRelPath}\" から絶対パス \"{_saveDirPath}\" に解決しました。");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ScreenSnapShot] エディタモード: 相対パス \"{editorRelPath}\" の解決に失敗。エラー: {ex.Message}。フォルダ選択またはデフォルトパスにフォールバックします。", this);
                _saveDirPath = GetEditorFallbackSavePath();
            }
        }
        else
        {
            _saveDirPath = GetEditorFallbackSavePath();
        }
#else
        try
        {
            _saveDirPath = Path.Combine(Application.persistentDataPath, BUILD_SUBDIR);
            Debug.Log($"[ScreenSnapShot] ビルドモード: 保存先を \"{_saveDirPath}\" に設定しました。");
        }
        catch (System.Exception ex)
        {
             Debug.LogError($"[ScreenSnapShot] ビルドモード: Application.persistentDataPath を使用したパス構築に失敗。エラー: {ex.Message}", this);
            _saveDirPath = Application.persistentDataPath;
        }
#endif

        try
        {
            if (string.IsNullOrEmpty(_saveDirPath))
            {
                 throw new DirectoryNotFoundException("最終的な保存ディレクトリパスが空です。");
            }
            if (!Directory.Exists(_saveDirPath))
            {
                Directory.CreateDirectory(_saveDirPath);
                Debug.Log($"[ScreenSnapShot] 保存先ディレクトリを作成しました: \"{_saveDirPath}\"");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ScreenSnapShot] ディレクトリ \"{_saveDirPath}\" の作成またはアクセスに失敗。エラー: {ex.Message}", this);
            _saveDirPath = Path.Combine(Application.temporaryCachePath, FALLBACK_SUBDIR);
            try
            {
                if (!Directory.Exists(_saveDirPath)) Directory.CreateDirectory(_saveDirPath);
                Debug.LogWarning($"[ScreenSnapShot] 最終フォールバックとして保存先ディレクトリを \"{_saveDirPath}\" に設定しました。");
            }
            catch (System.Exception fallbackEx)
            {
                Debug.LogError($"[ScreenSnapShot] 最終フォールバック用ディレクトリの作成にも失敗。エラー: {fallbackEx.Message}", this);
                IsInitialized = false;
                return;
            }
        }

        Debug.Log($"[ScreenSnapShot] 初期化成功。保存先ディレクトリ: \"{_saveDirPath}\", ベースファイル名: \"{_fileNameBase}\"");
        IsInitialized = true;
    }

#if UNITY_EDITOR
    private string GetEditorFallbackSavePath()
    {
        Debug.Log("[ScreenSnapShot] エディタモード: 保存先フォルダ選択ダイアログを開きます。");
        string selectedPath = EditorUtility.OpenFolderPanel("スクリーンショットの保存先フォルダを選択してください", Application.persistentDataPath, "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            return selectedPath;
        }
        else
        {
            string defaultPath = Path.Combine(Application.persistentDataPath, EDITOR_SUBDIR);
            Debug.LogWarning($"[ScreenSnapShot] エディタモード: フォルダ選択がキャンセルされました。デフォルトの保存先 \"{defaultPath}\" を使用します。");
            return defaultPath;
        }
    }
#endif

    /// <summary>
    /// スクリーンショットを非同期で撮影し、保存します。
    /// </summary>
    /// <returns>保存されたファイルのフルパス</returns>
    public async UniTask<string> CaptureScreenshotAsync()
    {
        if (!IsInitialized || _cam == null)
        {
            Debug.LogError("[ScreenSnapShot] 初期化されていないか、カメラがnullです。撮影できません。", this);
            return null;
        }
        if (string.IsNullOrEmpty(_saveDirPath))
        {
            Debug.LogError("[ScreenSnapShot] 重大なエラー: 保存先ディレクトリパスが設定されていません。撮影できません。", this);
            return null;
        }

        // ファイル名をベース名から生成
        string filePath = Path.Combine(_saveDirPath, _fileNameBase + EXT_PNG);

        int captureWidth = _cam.pixelWidth;
        int captureHeight = _cam.pixelHeight;

        if (captureWidth <= 0 || captureHeight <= 0)
        {
            Debug.LogWarning($"[ScreenSnapShot] カメラ ({_cam.name}) から無効な撮影サイズ ({captureWidth}x{captureHeight})。画面サイズをフォールバックとして使用します。");
            captureWidth = Screen.width;
            captureHeight = Screen.height;
            if (captureWidth <= 0 || captureHeight <= 0)
            {
                Debug.LogError("[ScreenSnapShot] 画面サイズへのフォールバックも無効。撮影できません。", this);
                return null;
            }
        }

        int originalMask = _cam.cullingMask;
        RenderTexture rt = null;
        Texture2D tex = null;

        try
        {
            if (_uiMask.value != 0)
            {
                _cam.cullingMask = originalMask & ~_uiMask.value;
            }

            rt = new RenderTexture(captureWidth, captureHeight, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);
            _cam.targetTexture = rt;

            if (GraphicsSettings.currentRenderPipeline != null)
            {
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            }
            else
            {
                _cam.Render();
                await UniTask.WaitForEndOfFrame();
            }

            RenderTexture.active = rt;
            tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            
            if (!Directory.Exists(_saveDirPath))
            {
                Directory.CreateDirectory(_saveDirPath);
                Debug.LogWarning($"[ScreenSnapShot] 保存先ディレクトリが撮影時に存在しませんでした。再作成: \"{_saveDirPath}\"");
            }
            
            await UniTask.RunOnThreadPool(() => File.WriteAllBytes(filePath, pngData));
            Debug.Log($"[ScreenSnapShot] スクリーンショットを保存しました: \"{filePath}\"");
            return filePath;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ScreenSnapShot] スクリーンショット撮影または \"{filePath}\" への保存に失敗。エラー: {e.Message}\n{e.StackTrace}", this);
            return null;
        }
        finally
        {
            _cam.cullingMask = originalMask;
            if (_cam.targetTexture == rt) _cam.targetTexture = null;
            if (RenderTexture.active == rt) RenderTexture.active = null;
            if (rt != null) Destroy(rt);
            if (tex != null) Destroy(tex);
        }
    }
}