using UnityEngine;
using UnityEngine.UI;

namespace InGame.View
{
    /// <summary>
    /// 上下左右で異なるマージンを設定するための構造体
    /// </summary>
    [System.Serializable]
    public struct ScreenMargins
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }

    /// <summary>
    /// Waypointマーカーの表示更新のみを行うView
    /// </summary>
    public class WaypointMarkerView : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private RectTransform iconRectTransform;
        [SerializeField] private RectTransform arrowRectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image arrowImage;

        [Header("見た目の設定")]
        [Tooltip("画面の端からのマージン（ピクセル）")]
        [SerializeField] private ScreenMargins margins = new ScreenMargins { top = 150f, bottom = 50f, left = 50f, right = 50f };
        [Tooltip("アイコンから矢印までの距離")]
        [SerializeField] private float arrowDistanceFromIcon = 40f;
        
        private Canvas _parentCanvas;
        private Camera _uiCamera;

        // --- 初期化 ---

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 親であるCanvasのコンポーネントを取得
            _parentCanvas = GetComponentInParent<Canvas>();

            // CanvasのRenderModeに応じて、座標変換に使うカメラを決定する
            if (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = null; // Overlayモードではカメラはnull
            }
            else
            {
                _uiCamera = _parentCanvas.worldCamera; // それ以外のモードではCanvas設定のカメラを使用
            }
        }

        #region アクセスメソッド

        /// <summary>
        /// 設定されているマージン情報を返す
        /// </summary>
        public ScreenMargins GetMargins() => margins;
        
        /// <summary>
        /// 設定されているアイコンと矢印の距離を返す
        /// </summary>
        public float GetArrowDistanceFromIcon() => arrowDistanceFromIcon;

        #endregion

        // --- 表示更新メソッド ---

        /// <summary>
        /// マーカー全体の表示・非表示を切り替える
        /// </summary>
        public void SetVisibility(bool isVisible)
        {
            if (iconImage != null) iconImage.enabled = isVisible;
            if (arrowImage != null) arrowImage.enabled = isVisible;
        }

        /// <summary>
        /// スクリーン座標を元にアイコンの位置を設定
        /// </summary>
        public void SetIconScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(iconRectTransform, screenPosition);
        }

        /// <summary>
        /// スクリーン座標を元に矢印の位置を設定
        /// </summary>
        public void SetArrowScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(arrowRectTransform, screenPosition);
        }

        /// <summary>
        /// 矢印の回転を設定
        /// </summary>
        public void SetArrowRotation(Quaternion rotation)
        {
            if (arrowRectTransform != null) arrowRectTransform.rotation = rotation;
        }

        /// <summary>
        /// スクリーン座標を、Canvas上のローカル座標に変換してUI要素を配置。
        /// </summary>
        private void SetElementPosition(RectTransform element, Vector2 screenPosition)
        {
            if (element == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.GetComponent<RectTransform>(),
                screenPosition,
                _uiCamera, // Initializeで設定した正しいカメラ情報を使用
                out Vector2 localPoint
            );
            element.anchoredPosition = localPoint;
        }
        
    }
}
