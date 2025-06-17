using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Common
{
    /// <summary>
    /// カメラの追従、ポストプロセスエフェクトの実行と制御をすべて担当する統合マネージャー。
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraManager : MonoBehaviour
    {
        [Header("ポストプロセス設定")]
        [Tooltip("ポストプロセス用のマテリアル")]
        [SerializeField] private Material effectMaterial;

        [Header("カメラ追従設定")]
        [Tooltip("プレイヤーからのZ軸オフセット")]
        [SerializeField] private float offsetZ = -10f;

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
        
        private void Awake()
        {
            // 自身のCameraコンポーネントを取得
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            if (effectMaterial == null)
            {
                Debug.LogWarning("Effect Materialが設定されていません。シェーダーエフェクトは機能しません。", this);
            }

            // カメラのオフセットを初期化
            _offset = new Vector3(0, 0, offsetZ);

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