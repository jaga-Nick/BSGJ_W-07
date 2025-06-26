using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Common;
using System.Threading;
using System;

namespace Common
{
    /// <summary>
    /// カメラの追従、ポストプロセスエフェクトの実行と制御をすべて担当する統合マネージャー。
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraManager : DestroyAvailable_SingletonMonoBehaviourBase<CameraManager>
    {
        [Header("ポストプロセス設定")]
        [Tooltip("ポストプロセス用のマテリアル")]
        [SerializeField] private Material effectMaterial;

        [Header("カメラ設定")]
        [Header("Z軸オフセット")]
        [Tooltip("プレイヤーからのZ軸オフセット")]
        [SerializeField] private float offsetZ = -10f;

        [Header("デフォルトカメラサイズ")]
        [SerializeField] private float _defaultSize = 5.625f;

        [Header("カメラ拡縮設定")]
        [Tooltip("拡大（ズームイン）する際のカメラサイズ")]
        [SerializeField] private float zoomInSize = 4.0f;
        [Tooltip("縮小（ズームアウト）する際のカメラサイズ")]
        [SerializeField] private float zoomOutSize = 7.0f;

        [Space(10)]

        [Tooltip("拡大（ズームイン）にかかる時間")]
        [SerializeField] private float zoomInTime = 0.1f;
        [Tooltip("縮小（ズームアウト）にかかる時間")]
        [SerializeField] private float zoomOutTime = 0.1f;
        [Tooltip("デフォルトサイズに戻るのにかかる時間")]
        [SerializeField] private float defaultTime = 0.3f;

        [Space(10)]

        [Tooltip("ズームした状態で待機する時間")]
        [SerializeField] private float zoomHoldTime = 0.2f;

        [Header("色覚異常対応モード（初期設定）")]
        [Tooltip("起動時に色覚異常対応モードを有効にするか")]
        [SerializeField] private bool enableColorblindModeOnStart = false;
        [Tooltip("適用する色覚異常のタイプ (0:1型, 1:2型など)")]
        [SerializeField] private int colorblindType = 0;

        // --- 内部状態 ---
        private Camera _camera;
        private Transform _target;
        private Vector3 _offset;
        private bool _isFocusEnabled;
        private Vector4 _focusPoint;
        private bool _isColorblindModeEnabled;
        private CancellationTokenSource _cameraSizeCts;

        private void Awake()
        {
            // 自身のCameraコンポーネントを取得
            _camera = GetComponent<Camera>();

            _cameraSizeCts = new CancellationTokenSource();
        }

        private void Start()
        {
            if (effectMaterial == null)
            {
                Debug.LogWarning("Effect Materialが設定されていません。シェーダーエフェクトは機能しません。", this);
            }

            // カメラのオフセットを初期化
            _offset = new Vector3(0, 0, offsetZ);

            // カメラのサイズを初期化
            _camera.orthographicSize = _defaultSize;

            // 起動時の色覚異常モードを設定
            _isColorblindModeEnabled = enableColorblindModeOnStart;

            // 追従対象のプレイヤーを自動で探す
            FindTargetPlayer();
        }
        

        private void LateUpdate()
        {
            UpdateShaderParameters();
            if (_target == null)
            {
                FindTargetPlayer();
                return;
            }
            
            // ターゲットを追従する
            Vector3 targetPosition = _target.position;
            Vector3 finalPosition = targetPosition + _offset;
            transform.position = finalPosition;
        }

        private void FindTargetPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _target = playerObj.transform;
            }
        }


        #region カメラワーク(FoV)

        private void CancelPreviousSizeAnimation()
        {
            _cameraSizeCts?.Cancel();
            _cameraSizeCts = new CancellationTokenSource();
        }

        private async UniTask AnimateFoV(float targetValue, float duration, CancellationToken cancellationToken)
        {
            float startValue = _camera.orthographicSize;
            float elapsedTime = 0f;
            if (duration <= 0f) { _camera.orthographicSize = targetValue; return; }
            try { while (elapsedTime < duration) { elapsedTime += Time.deltaTime; float t = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / duration); float currentValue = Mathf.Lerp(startValue, targetValue, t); _camera.orthographicSize = currentValue; await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); } } catch (OperationCanceledException) { return; }
            _camera.orthographicSize = targetValue;
        }


        /// <summary>
        /// カメラをズームインし、一定時間後に元のサイズに戻す
        /// </summary>
        public async UniTask ZoomInAndReturn()
        {
            CancelPreviousSizeAnimation();
            try
            {
                await AnimateFoV(zoomInSize, zoomInTime, _cameraSizeCts.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(zoomHoldTime), cancellationToken: _cameraSizeCts.Token);
                await AnimateFoV(_defaultSize, defaultTime, _cameraSizeCts.Token);
            }
            catch (OperationCanceledException)
            {
                // 一連の動作がキャンセルされた
            }
        }

        /// <summary>
        /// カメラをズームアウトし、一定時間後に元のサイズに戻す
        /// </summary>
        public async UniTask ZoomOutAndReturn()
        {
            CancelPreviousSizeAnimation();
            try
            {
                await AnimateFoV(zoomOutSize, zoomOutTime, _cameraSizeCts.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(zoomHoldTime), cancellationToken: _cameraSizeCts.Token);
                await AnimateFoV(_defaultSize, defaultTime, _cameraSizeCts.Token);
            }
            catch (OperationCanceledException)
            {
                // 一連の動作がキャンセルされた
            }
        }


        /// <summary>
        /// カメラをズームインする
        /// </summary>
        public void ZoomIn()
        {
            CancelPreviousSizeAnimation();
            AnimateFoV(zoomInSize, zoomInTime, _cameraSizeCts.Token).Forget();
        }

        /// <summary>
        /// カメラをズームアウトする
        /// </summary>
        public void ZoomOut()
        {
            CancelPreviousSizeAnimation();
            AnimateFoV(zoomOutSize, zoomOutTime, _cameraSizeCts.Token).Forget();
        }

        /// <summary>
        /// カメラをデフォルトのサイズに戻す
        /// </summary>
        public void DefaultFoV()
        {
            CancelPreviousSizeAnimation();
            AnimateFoV(_defaultSize, defaultTime, _cameraSizeCts.Token).Forget();
        }

        #endregion


        /// <summary>
        /// シェーダーに必要なパラメータを更新
        /// </summary>
        private void UpdateShaderParameters()
        {
            if (effectMaterial == null) return;
            
            // 視線注視
            if (_isFocusEnabled)
            {
                effectMaterial.EnableKeyword("FOCUS_ON");
                effectMaterial.SetVector("_FocusPoint", _focusPoint);
            }
            else
            {
                effectMaterial.DisableKeyword("FOCUS_ON");
            }

            // 色覚異常対応
            if (_isColorblindModeEnabled)
            {
                effectMaterial.EnableKeyword("COLORBLIND_ON");
                effectMaterial.SetInt("_ColorblindType", colorblindType);
            }
            else
            {
                effectMaterial.DisableKeyword("COLORBLIND_ON");
            }
        }

        #region Public API (他のスクリプトからこれらを呼び出して制御します)

        /// <summary>
        /// カメラが追従するターゲットを設定
        /// </summary>
        public void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }

        /// <summary>
        /// 指定したワールド座標に焦点を合わせ、周辺をぼかす
        /// </summary>
        public void SetFocus(Vector3 worldPosition)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(worldPosition);
            _isFocusEnabled = true;
            _focusPoint = new Vector4(
                screenPos.x / _camera.pixelWidth, 
                screenPos.y / _camera.pixelHeight, 0, 0);
        }

        /// <summary>
        /// 焦点（ぼかし効果）を解除
        /// </summary>
        public void ClearFocus()
        {
            _isFocusEnabled = false;
        }

        /// <summary>
        /// 色覚異常対応モードの有効/無効を切り替る
        /// </summary>
        public void SetColorblindMode(bool isEnabled)
        {
            _isColorblindModeEnabled = isEnabled;
        }

        /// <summary>
        /// 適用する色覚異常のタイプを変更
        /// </summary>
        public void SetColorblindType(int type)
        {
            colorblindType = type;
        }

        #endregion

        #region Render Pipeline Hooks (ポストプロセス実行)

        // --- Built-in Render Pipeline用 ---
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (GraphicsSettings.defaultRenderPipeline != null)
            {
                // URP/HDRPがアクティブな場合は、BiRPのOnRenderImageを無効にする
                Graphics.Blit(source, destination);
                return;
            }

            if (effectMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }
            Graphics.Blit(source, destination, effectMaterial);
        }
        
        // --- URP/HDRP用 ---
        private void OnEnable()
        {
            if (GraphicsSettings.defaultRenderPipeline != null)
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            if (GraphicsSettings.defaultRenderPipeline != null)
                RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != this._camera || effectMaterial == null) return;
            
            // URPでのポストプロセスの実装は、カスタムレンダラーフィーチャーとパスで行うのが一般的です。
            CommandBuffer cmd = CommandBufferPool.Get("ShaderEffects");
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        #endregion
        
    }
}