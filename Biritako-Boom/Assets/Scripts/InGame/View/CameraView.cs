// InGame/View/CameraView.cs

using UnityEngine;
using UnityEngine.Rendering;

namespace InGame.View
{
    /// <summary>
    /// カメラの見た目（Transformとポストエフェクト）を操作するクラス
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraView : MonoBehaviour
    {
        [Tooltip("ポストプロセス用のマテリアル")]
        [SerializeField] private Material effectMaterial;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }
        
        /// <summary>
        /// カメラの位置を設定します
        /// </summary>
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        /// <summary>
        /// カメラのローカル位置を設定します (エフェクトによる揺れなどに使用)
        /// </summary>
        public void SetLocalPosition(Vector3 newLocalPosition)
        {
            transform.localPosition = newLocalPosition;
        }

        /// <summary>
        /// シェーダーに必要なパラメータを更新します
        /// </summary>
        public void UpdateShaderParameters(bool focusEnabled, Vector4 focusPoint, bool colorblindEnabled, int colorblindType)
        {
            if (effectMaterial == null) return;
            
            // 視線注視
            if (focusEnabled)
            {
                effectMaterial.EnableKeyword("FOCUS_ON");
                effectMaterial.SetVector("_FocusPoint", focusPoint);
            }
            else
            {
                effectMaterial.DisableKeyword("FOCUS_ON");
            }

            // 色覚異常対応
            if (colorblindEnabled)
            {
                effectMaterial.EnableKeyword("COLORBLIND_ON");
                effectMaterial.SetInt("_ColorblindType", colorblindType);
            }
            else
            {
                effectMaterial.DisableKeyword("COLORBLIND_ON");
            }
        }
        
        public Vector3 GetWorldPositionFromScreen(Vector3 worldPos)
        {
            return _camera.WorldToScreenPoint(worldPos);
        }

        public float GetPixelWidth() => _camera.pixelWidth;
        public float GetPixelHeight() => _camera.pixelHeight;


        #region Render Pipeline Hooks
        // --- Built-in Render Pipeline用 ---
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (effectMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }
            Graphics.Blit(source, destination, effectMaterial);
        }
        
        // --- URP用 (URPを使用している場合、こちらが呼ばれるように設定が必要) ---
        // この部分はURPのバージョンや設定によって書き方が変わる可能性があります
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
            
            // URPでのポストプロセスの実装例（詳細はプロジェクト設定によります）
            CommandBuffer cmd = CommandBufferPool.Get("CameraEffects");
            // cmd.Blit(...) を使ってエフェクトを適用
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        #endregion
    }
}