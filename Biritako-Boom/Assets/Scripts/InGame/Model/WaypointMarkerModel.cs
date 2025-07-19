using UnityEngine;
using InGame.Presenter;
using InGame.View;

namespace InGame.Model
{
    /// <summary>
    /// Waypointマーカーの計算ロジックと状態を管理するModel。
    /// </summary>
    public class WaypointMarkerModel
    {
        private Transform _target;
        private Camera _mainCamera;

        #region アクセスメソッド
        
        /// <summary>
        /// 追跡対象のターゲットを設定します。
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// 計算に使用するカメラを設定します。
        /// </summary>
        public void SetMainCamera(Camera camera)
        {
            _mainCamera = camera;
        }

        #endregion
        

        /// <summary>
        /// ターゲットが画面内にいるかどうかを判定
        /// </summary>
        /// <returns>画面内にいればtrue、およびターゲットのスクリーン座標</returns>
        public (bool IsOnScreen, Vector3 ScreenPosition) CheckTargetVisibility()
        {
            // ターゲットまたはカメラが未設定の場合は、常に見えないものとして扱う
            if (_target == null || _mainCamera == null)
            {
                return (false, Vector3.zero);
            }
            

            // ターゲットのワールド座標をスクリーン座標に変換
            Vector3 targetScreenPosition = _mainCamera.WorldToScreenPoint(_target.position);

            // ターゲットがカメラの前方にあり、かつ画面の境界内にいるかを判定
            bool isOnScreen = targetScreenPosition.z >= 0 &&
                              targetScreenPosition.x >= 0 && targetScreenPosition.x <= Screen.width &&
                              targetScreenPosition.y >= 0 && targetScreenPosition.y <= Screen.height;



            return (isOnScreen, targetScreenPosition);
        }

        /// <summary>
        /// マーカーのアイコンと矢印の位置・回転を計算
        /// </summary>
        /// <param name="targetScreenPosition">ターゲットのスクリーン座標</param>
        /// <param name="margins">画面端からのマージン</param>
        /// <param name="arrowDistanceFromIcon">アイコンと矢印の距離</param>
        /// <returns>アイコンのスクリーン座標、矢印のスクリーン座標、矢印の回転</returns>
        public (Vector3 IconPosition, Vector3 ArrowPosition, Quaternion ArrowRotation) CalculateMarkerTransform(Vector3 targetScreenPosition, ScreenMargins margins, float arrowDistanceFromIcon)
        {
            // ターゲットがカメラの後方にある場合、座標を反転させて向きを補正
            if (targetScreenPosition.z < 0)
            {
                targetScreenPosition *= -1;
            }
            
            // アイコンを配置すべきスクリーン座標を計算
            Vector3 iconPosition = new Vector3(
                Mathf.Clamp(targetScreenPosition.x, margins.left, Screen.width - margins.right),
                Mathf.Clamp(targetScreenPosition.y, margins.bottom, Screen.height - margins.top),
                0f
            );

            // アイコンからターゲットへの方向ベクトルを計算
            Vector3 direction = (targetScreenPosition - iconPosition).normalized;
            
            // 矢印の回転を計算
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion arrowRotation = Quaternion.Euler(0, 0, angle);
            
            // 矢印の位置を計算
            Vector3 arrowPosition = iconPosition + direction * arrowDistanceFromIcon;
            
            return (iconPosition, arrowPosition, arrowRotation);
        }
    }
}
