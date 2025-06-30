using System;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;

namespace InGame.Presenter
{
    /// <summary>
    /// Waypointマーカーのロジックを担当するPresenter。
    /// ViewのRectTransformの操作も含め、すべてのロジックを担う。
    /// </summary>
    public class WaypointMarkerPresenter : MonoBehaviour
    {
        /// <summary>
        /// view
        /// </summary>
        private WaypointMarkerView _view;
        
        /// <summary>
        /// 位置案内アイコンの設定
        /// </summary>
        [Header("設定")]
        [Tooltip("画面の端からのマージン（ピクセル）")]
        [SerializeField] private ScreenMargins margins = new ScreenMargins { top = 150f, bottom = 50f, left = 50f, right = 50f };
        [Tooltip("アイコンから矢印までの距離")]
        [SerializeField] private float arrowDistanceFromIcon = 40f;

        /// <summary>
        /// Presenterが管理する内部フィールド
        /// </summary>
        private Camera _mainCamera;
        private Transform _target;


        private void OnEnable()
        {
            // イベントに自分の更新処理を登録
            EnemySpawner.OnGenerateMotherShip += FindAndSetTarget;
        }

        private void OnDisable()
        {
            // オブジェクトが破棄される際などに、イベントから登録を解除（重要）
            EnemySpawner.OnGenerateMotherShip -= FindAndSetTarget;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _view = GetComponent<WaypointMarkerView>();
            _view.SetVisibility(false);
            EnemySpawner.OnGenerateMotherShip += FindAndSetTarget;
        }


        private void LateUpdate()
        {
            if (!_target) return;

            var targetScreenPosition = _mainCamera.WorldToScreenPoint(_target.position);

            
            var isTargetVisible = targetScreenPosition.z > 0 &&
                                  targetScreenPosition.x > 0 && targetScreenPosition.x < Screen.width &&
                                  targetScreenPosition.y > 0 && targetScreenPosition.y < Screen.height;
            

            _view.SetVisibility(!isTargetVisible);

            if (!isTargetVisible)
            {
                UpdateMarkerPositionAndRotation(targetScreenPosition);
            }
        }

        private void FindAndSetTarget()
        {
            _target = FindObjectOfType<MotherShipPresenter>().gameObject.transform;
        }

        private void UpdateMarkerPositionAndRotation(Vector3 targetScreenPosition)
        {
            if (targetScreenPosition.z < 0)
            {
                targetScreenPosition *= -1;
            }
            
            // アイコンの位置を、個別のマージンを使って画面内にクランプする
            var iconPosition = new Vector3(
                Mathf.Clamp(targetScreenPosition.x, margins.left, Screen.width - margins.right),
                Mathf.Clamp(targetScreenPosition.y, margins.bottom, Screen.height - margins.top),
                0f
            );
            _view.SetIconPosition(iconPosition);

            // アイコンからターゲットのスクリーン座標への方向を計算
            var direction = (targetScreenPosition - iconPosition).normalized;
            
            // 矢印の回転を計算
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var arrowRotation = Quaternion.Euler(0, 0, angle);
            
            // 矢印の位置をアイコンの位置からdirectionの方向に一定距離離れた場所に設定
            var arrowPosition = iconPosition + direction * arrowDistanceFromIcon;
            
            // Viewに矢印の位置と回転を指示
            _view.SetArrowPosition(arrowPosition);
            _view.SetArrowRotation(arrowRotation);
        }
    }
    
    
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
}