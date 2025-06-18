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
        // Inspectorで設定しやすいように、ImageではなくRectTransformを直接参照します
        [SerializeField] private RectTransform iconRectTransform;
        [SerializeField] private RectTransform arrowRectTransform;
        
        // Imageコンポーネントはenabledの切り替えに使うため、こちらも参照します
        [SerializeField] private Image iconImage;
        [SerializeField] private Image arrowImage;

        /// <summary>
        /// マーカー全体の表示・非表示を切り替える
        /// </summary>
        public void SetVisibility(bool isVisible)
        {
            if (iconImage != null) iconImage.enabled = isVisible;
            if (arrowImage != null) arrowImage.enabled = isVisible;
        }

        /// <summary>
        /// アイコンの位置を設定する
        /// </summary>
        public void SetIconPosition(Vector3 position)
        {
            if (iconRectTransform != null) iconRectTransform.position = position;
        }

        /// <summary>
        /// 矢印の位置を設定する
        /// </summary>
        public void SetArrowPosition(Vector3 position)
        {
            if (arrowRectTransform != null) arrowRectTransform.position = position;
        }

        /// <summary>
        /// 矢印の回転を設定する
        /// </summary>
        public void SetArrowRotation(Quaternion rotation)
        {
            if (arrowRectTransform != null) arrowRectTransform.rotation = rotation;
        }
    }
}