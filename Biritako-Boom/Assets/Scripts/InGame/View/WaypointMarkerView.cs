using UnityEngine;
using UnityEngine.UI;

namespace InGame.View
{
    /// <summary>
    /// WaypointマーカーのUI要素の参照を保持し、Presenterからの指示で表示を更新する「純粋な」View。
    /// </summary>
    public class WaypointMarkerView : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private RectTransform iconRectTransform;
        [SerializeField] private RectTransform arrowRectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image arrowImage;

        private Canvas _parentCanvas;
        private Camera _uiCamera; // RenderModeに応じて設定される、座標変換用のカメラ

        private void Awake()
        {
            // 親であるCanvasのコンポーネントを取得
            _parentCanvas = GetComponentInParent<Canvas>();

            // CanvasのRenderModeに応じて、座標変換に使うカメラを決定する
            if (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Overlayモードでは、カメラはnullを指定する必要がある
                _uiCamera = null;
            }
            else
            {
                // ScreenSpaceCameraモードやWorldSpaceモードでは、Canvasに設定されているカメラを使用
                _uiCamera = _parentCanvas.worldCamera;
            }
        }


        /// <summary>
        /// マーカー全体の表示・非表示を切り替える
        /// </summary>
        public void SetVisibility(bool isVisible)
        {
            if (iconImage != null) iconImage.enabled = isVisible;
            if (arrowImage != null) arrowImage.enabled = isVisible;
        }

        /// <summary>
        /// 矢印の回転を設定する
        /// </summary>
        public void SetArrowRotation(Quaternion rotation)
        {
            if (arrowRectTransform != null) arrowRectTransform.rotation = rotation;
        }


        /// <summary>
        /// スクリーン座標を元にアイコンの位置を設定します
        /// </summary>
        public void SetIconScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(iconRectTransform, screenPosition);
        }

        /// <summary>
        /// スクリーン座標を元に矢印の位置を設定します
        /// </summary>
        public void SetArrowScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(arrowRectTransform, screenPosition);
        }

        /// <summary>
        /// スクリーン座標を、Canvas Scalerの影響を考慮したUIのローカル座標に変換して配置する
        /// </summary>
        private void SetElementPosition(RectTransform element, Vector2 screenPosition)
        {
            if (element == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.GetComponent<RectTransform>(),
                screenPosition,
                _uiCamera, // Awakeで設定した正しいカメラ情報を使用
                out Vector2 localPoint
            );
            element.anchoredPosition = localPoint;
        }
    }
}
