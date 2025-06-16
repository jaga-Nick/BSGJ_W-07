// InGame/Presenter/CameraPresenter.cs

using Cysharp.Threading.Tasks; // UniTask
using System.Threading;       // CancellationToken
using InGame.Model;
using InGame.View;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace InGame.Presenter
{
    /// <summary>
    /// カメラのロジックとエフェクトを制御するクラス
    /// </summary>
    public class CameraPresenter : MonoBehaviour
    {
        [Header("必須項目")]
        [SerializeField] private CameraView cameraView;

        [Header("カメラ追従設定")]
        [SerializeField] private float offsetZ = -10f;
        
        [Header("パーリンノイズによる手ブレ効果を有効にするか")]
        [SerializeField] private bool enablePerlinNoiseShake = false;

        [Header("手ブレの強さ（揺れの大きさ）")]
        [SerializeField] private float perlinNoiseStrength = 0.1f;

        [Header("色覚異常対応モードを有効にするか")]
        [SerializeField] private bool enableColorblindMode = false;

        [Header("適用する色覚異常のタイプ")]
        [Tooltip("0:1型, 1:2型など、シェーダーで定義したタイプを数字で指定します。")]
        [SerializeField] private int colorblindType = 0;

        private CameraModel _cameraModel;
        private Transform _target;
        private Vector3 _initialLocalPosition;

        void Start()
        {
            _cameraModel = new CameraModel();
            _cameraModel.Initialize(new Vector3(0, 0, offsetZ));
            
            // エフェクト設定をModelに反映（セッターを使用）
            _cameraModel.SetIsPerlinNoiseShakeEnabled(enablePerlinNoiseShake);
            _cameraModel.SetPerlinNoiseStrength(perlinNoiseStrength);
            _cameraModel.SetIsColorblindModeEnabled(enableColorblindMode);
            _cameraModel.SetColorblindType(colorblindType);

            _initialLocalPosition = cameraView.transform.localPosition;
        }

        /// <summary>
        /// 外部から追従対象を設定します
        /// </summary>
        public void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }

        void Update()
        {
            // PerlinNoise手ブレのオフセット計算（ゲッター/セッターを使用）
            if (_cameraModel.GetIsPerlinNoiseShakeEnabled())
            {
                float x = (Mathf.PerlinNoise(Time.time, 0) - 0.5f) * _cameraModel.GetPerlinNoiseStrength();
                float y = (Mathf.PerlinNoise(0, Time.time) - 0.5f) * _cameraModel.GetPerlinNoiseStrength();
                _cameraModel.SetEffectOffset(new Vector3(x, y, 0));
            }
            else if (_cameraModel.GetEffectOffset() != Vector3.zero && !_isShaking) 
            {
                 _cameraModel.SetEffectOffset(Vector3.zero);
            }

            // シェーダーパラメータをViewに渡す（ゲッターを使用）
            cameraView.UpdateShaderParameters(
                _cameraModel.GetIsFocusEnabled(), 
                _cameraModel.GetFocusPoint(),
                _cameraModel.GetIsColorblindModeEnabled(),
                _cameraModel.GetColorblindType()
            );
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                _target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            
            // 1. ターゲット追従の基本位置を計算
            Vector3 targetPosition = _target.position;
            Vector3 basePosition = targetPosition + _cameraModel.GetOffset();

            // 2. 基本位置にエフェクトによるオフセットを加算（ゲッターを使用）
            Vector3 finalPosition = basePosition + _cameraModel.GetEffectOffset();
            
            // 3. Viewに最終的な位置を指示
            cameraView.SetPosition(finalPosition);
        }

        #region Public Methods for Effects

        private bool _isShaking = false; 

        /// <summary>
        /// カメラを揺らします
        /// </summary>
        public void Shake(float duration, float magnitude)
        {
            if (_isShaking) return;
            ShakeAsync(duration, magnitude, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask ShakeAsync(float duration, float magnitude, CancellationToken ct)
        {
            _isShaking = true;
            float elapsed = 0.0f;

            while (elapsed < duration && !ct.IsCancellationRequested)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                _cameraModel.SetEffectOffset(new Vector3(x, y, 0)); // セッターを使用

                magnitude = Mathf.Lerp(magnitude, 0, elapsed / duration);
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            _cameraModel.SetEffectOffset(Vector3.zero); // セッターを使用
            _isShaking = false;
        }

        /// <summary>
        /// 指定したワールド座標に焦点を合わせます
        /// </summary>
        public void SetFocus(Vector3 worldPosition)
        {
            Vector3 screenPos = cameraView.GetWorldPositionFromScreen(worldPosition);
            _cameraModel.SetIsFocusEnabled(true); // セッターを使用
            _cameraModel.SetFocusPoint(new Vector4( // セッターを使用
                screenPos.x / cameraView.GetPixelWidth(), 
                screenPos.y / cameraView.GetPixelHeight(), 0, 0));
        }

        /// <summary>
        /// 焦点を解除します
        /// </summary>
        public void ClearFocus()
        {
            _cameraModel.SetIsFocusEnabled(false); // セッターを使用
        }
        
        #endregion
        
        
        
        
        
        
        
        #region Debug Methods

        [ContextMenu("Debug/Shake (Short, Weak)")]
        private void DebugShakeShortWeak()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }
            Shake(0.5f, 0.2f);
        }

        [ContextMenu("Debug/Shake (Long, Strong)")]
        private void DebugShakeLongStrong()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }
            Shake(1.5f, 0.8f);
        }

        [ContextMenu("Debug/Focus on Target")]
        private void DebugFocusOnTarget()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }
            if (_target == null)
            {
                Debug.LogWarning("ターゲットが設定されていません。");
                return;
            }
            Debug.Log($"ターゲット '{_target.name}' にフォーカスします。");
            SetFocus(_target.position);
        }

        [ContextMenu("Debug/Clear Focus")]
        private void DebugClearFocus()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }
            Debug.Log("フォーカスを解除します。");
            ClearFocus();
        }

        [ContextMenu("Debug/Toggle Perlin Noise Shake")]
        private void DebugTogglePerlinNoiseShake()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }
            
            // インスペクターの値とModelの値を両方切り替える
            enablePerlinNoiseShake = !enablePerlinNoiseShake;
            _cameraModel.SetIsPerlinNoiseShakeEnabled(enablePerlinNoiseShake);
            Debug.Log($"Perlin Noise手ブレ: {enablePerlinNoiseShake}");

            // インスペクターの表示を更新し、変更を保存する
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }

        [ContextMenu("Debug/Toggle Colorblind Mode")]
        private void DebugToggleColorblindMode()
        {
            if (!Application.isPlaying) { Debug.LogWarning("ゲーム実行中に使用してください"); return; }

            enableColorblindMode = !enableColorblindMode;
            _cameraModel.SetIsColorblindModeEnabled(enableColorblindMode);
            Debug.Log($"色覚異常モード: {enableColorblindMode}");
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }

        #endregion
        
        
        
        
        
    }
}