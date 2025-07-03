using System;
using InGame.Model;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;

namespace InGame.Presenter
{
    /// <summary>
    /// Waypointマーカーのロジックを担当するPresenter。
    /// この最終修正版では、UIの座標系問題を解決し、構造を安定させています。
    /// </summary>
    public class WaypointMarkerPresenter : MonoBehaviour
    {
        private WaypointMarkerView _view;
        private WaypointMarkerModel _model = new WaypointMarkerModel();

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
            // ★修正点：Awakeでのイベント二重登録を削除
        }

        private void Start()
        {
            if (_view == null)
            {
                Debug.LogError("同じGameObjectにWaypointMarkerViewが見つかりません！", this);
                enabled = false;
                return;
            }
            _view.SetVisibility(false);
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // ステップ1：判定は全てスクリーン座標系で行う
            Vector3 targetScreenPosition = _mainCamera.WorldToScreenPoint(_target.position);

            // 可視性判定は、マージンを考慮しない画面全体で行う
            bool isTargetOnScreen = targetScreenPosition.z > 0 &&
                                    targetScreenPosition.x >= 0 && targetScreenPosition.x <= Screen.width &&
                                    targetScreenPosition.y >= 0 && targetScreenPosition.y <= Screen.height;

            _view.SetVisibility(!isTargetOnScreen);

            if (!isTargetOnScreen)
            {
                // ターゲットがカメラの後方にある場合、座標を反転させて向きを補正
                if (targetScreenPosition.z < 0)
                {
                    targetScreenPosition *= -1;
                }

                // 配置ロジックを呼び出す
                UpdateMarkerPositionAndRotation(targetScreenPosition);
            }
        }

        private void FindAndSetTarget()
        {
            var motherShip = FindObjectOfType<MotherShipPresenter>();
            if (motherShip != null)
            {
                _target = motherShip.gameObject.transform;
            }
        }

        private void UpdateMarkerPositionAndRotation(Vector3 targetScreenPosition)
        {
            // アイコンを配置すべきスクリーン座標を計算
            Vector3 iconScreenPosition = new Vector3(
                Mathf.Clamp(targetScreenPosition.x, margins.left, Screen.width - margins.right),
                Mathf.Clamp(targetScreenPosition.y, margins.bottom, Screen.height - margins.top),
                0f
            );

            // ★修正点：Viewへの指示をシンプルに（カメラ情報を渡さない）
            _view.SetIconScreenPosition(iconScreenPosition);

            // 方向の計算はスクリーン座標系で行う
            Vector3 direction = (targetScreenPosition - iconScreenPosition).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion arrowRotation = Quaternion.Euler(0, 0, angle);

            // 矢印の位置を計算
            Vector3 arrowScreenPosition = iconScreenPosition + direction * arrowDistanceFromIcon;

            // ★修正点：Viewへの指示をシンプルに（カメラ情報を渡さない）
            _view.SetArrowScreenPosition(arrowScreenPosition);
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