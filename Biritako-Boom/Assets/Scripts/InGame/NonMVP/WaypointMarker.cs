using UnityEngine;
using UnityEngine.UI;

namespace InGame.NonMVP
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
    /// Mothershipの方向を示すWayPointマーカーの、Model・View・Presenterの機能を統合した単一コンポーネント。
    /// このスクリプトをUI要素（アイコンや矢印を含む親オブジェクト）にアタッチして使用します。
    /// </summary>
    public class WaypointMarker : MonoBehaviour
    {
        [Header("ターゲット設定")]
        [Tooltip("目標とするMothershipオブジェクトのTransform")]
        private Transform _targetTransform; // スポナーで外部から設定される
        
        [Header("UI要素")]
        [SerializeField] private RectTransform iconRectTransform;
        [SerializeField] private RectTransform arrowRectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image arrowImage;

        [Header("見た目の設定")]
        [Tooltip("画面の端からのマージン（ピクセル）")]
        [SerializeField] private ScreenMargins margins = new ScreenMargins { };
        [Tooltip("アイコンから矢印までの距離")]
        [SerializeField] private float arrowDistanceFromIcon;
        
        [Header("距離による見た目の調整")]
        [Tooltip("この距離を境にマーカーの透明度が変化し始める (遠いほど薄くなる)")]
        [SerializeField] private float maxFadeDistance = 100f; 
        [Tooltip("最大距離時 (maxFadeDistance) の最小透明度 (0.0～1.0)")]
        [SerializeField] [Range(0f, 1f)] private float minFadeAlpha = 0.3f; 
        [Tooltip("近距離時の最大透明度 (1.0)")]
        [SerializeField] [Range(0f, 1f)] private float maxFadeAlpha = 1.0f; 
        
        private Canvas _parentCanvas;
        private Camera _uiCamera;
        private Camera _mainCamera; // ワールド座標計算に使用するメインカメラ

        // --- 初期化 ---

        private void Start()
        {
            // --- View初期化 ---
            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas == null)
            {
                Debug.LogError("WaypointIndicator must be a child of a Canvas.");
                this.enabled = false;
                return;
            }

            // CanvasのRenderModeに応じて、座標変換に使うカメラを決定する
            _uiCamera = (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _parentCanvas.worldCamera;
            // --- Model初期化 ---
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera is not found in the scene.");
                this.enabled = false;
                return;
            }
        }

        /// <summary>
        /// スポナーなど外部から追跡対象のターゲットを設定します。
        /// スポナーで母艦UFOが生成された後に、このメソッドを呼び出す必要があります。
        /// </summary>
        /// <param name="target">追跡対象のTransform</param>
        public void SetTargetTransform(Transform target)
        {
            _targetTransform = target;
            // ターゲットが設定されたらマーカーを非表示にしてリセット
            SetVisibility(false);
        }
        
        /// <summary>
        /// ターゲットまでの距離に基づいてマーカーの透明度を調整します。
        /// </summary>
        private void AdjustMarkerAppearanceByDistance()
        {
            if (_targetTransform == null) return;
            
            // 距離を計算
            float distance = Vector3.Distance(_mainCamera.transform.position, _targetTransform.position);

            // 距離の割合を計算 (0:遠い/最大距離, 1:近い/ゼロ距離)
            // maxFadeDistanceを超えた場合は0、ゼロ距離の場合は1になる
            float distanceRatio = 1f - Mathf.Clamp01(distance / maxFadeDistance);

            // 透明度を調整 (LerpでminFadeAlphaからmaxFadeAlphaへ補間)
            float alpha = Mathf.Lerp(minFadeAlpha, maxFadeAlpha, distanceRatio);
            
            SetAlpha(iconImage, alpha);
            SetAlpha(arrowImage, alpha);
        }

        private void Update()
        {
            if (_targetTransform == null)
            {
                SetVisibility(false);
                return;
            }
            
            // 1. Model層ロジック: ターゲットの可視性をチェック
            var (isOnScreen, viewportPoint) = CheckTargetVisibility();

            if (isOnScreen)
            {
                // 2. View層ロジック: 画面内の場合、非表示
                SetVisibility(false);
            }
            else
            {
                // 2. View層ロジック: 画面外の場合、表示
                SetVisibility(true);

                // 3. Model層ロジック: マーカーの位置と回転を計算 (バグ修正ロジック含む)
                var (iconPos, arrowPos, rotation) = CalculateMarkerTransform(viewportPoint);
                
                // 4. View層ロジック: UI要素を更新
                SetIconScreenPosition(iconPos);
                SetArrowScreenPosition(arrowPos);
                SetArrowRotation(rotation);
                AdjustMarkerAppearanceByDistance();
            }
        }

        #region Model (計算ロジック)

        /// <summary>
        /// ターゲットが画面内にいるかどうかを判定
        /// </summary>
        private (bool IsOnScreen, Vector3 ViewportPosition) CheckTargetVisibility()
        {
            if (_mainCamera == null)
            {
                // この場合はエラーを出す代わりに非表示状態を返す
                return (false, Vector3.zero);
            }
            
            // ワールド座標をViewport座標に変換 (0.0～1.0の範囲, Zはカメラからの距離)
            var viewportPoint = _mainCamera.WorldToViewportPoint(_targetTransform.position);

            // ターゲットがカメラの前方にあり、かつ画面の境界内にいるかを判定
            bool isBehind = viewportPoint.z < 0;
            bool isOnScreen = !isBehind && 
                              viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
                              viewportPoint.y >= 0 && viewportPoint.y <= 1;

            return (isOnScreen, viewportPoint);
        }

        /// <summary>
        /// マーカーのアイコンと矢印の位置・回転を計算 (バグ修正ロジック)
        /// </summary>
        private (Vector2 IconScreenPosition, Vector2 ArrowScreenPosition, Quaternion ArrowRotation) CalculateMarkerTransform(Vector3 viewportPoint)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // --- 1. カメラの後ろにある場合の処理 ---
            var isBehind = viewportPoint.z < 0;
            if (isBehind)
            {
                // カメラの後ろにある場合、Viewport座標を反転させて、画面の反対側を指すようにする
                viewportPoint.x = 1f - viewportPoint.x;
                viewportPoint.y = 1f - viewportPoint.y;
            }

            // --- 2. 画面の中心を基準としたUI座標の計算 ---
            
            // Viewport座標 (0～1) をScreen座標 (0～W, 0～H) のベクトルに変換
            Vector2 screenVector = new Vector2(viewportPoint.x * screenWidth, viewportPoint.y * screenHeight);
            
            // Screen座標を画面の中心を原点(0,0)とした相対座標に変換
            Vector2 screenCenter = new Vector2(screenWidth / 2f, screenHeight / 2f);
            Vector2 directionFromCenter = screenVector - screenCenter;

            // --- 3. 画面の端にクランプ (マージン適用) ---
            
            // 各マージンを個別に適用するため、方向ベクトルに応じて適切なマージンを選ぶ
            var currentMaxX = (directionFromCenter.x > 0) 
                ? screenWidth / 2f - margins.right 
                : screenWidth / 2f - margins.left;
            
            var currentMaxY = (directionFromCenter.y > 0) 
                ? screenHeight / 2f - margins.top 
                : screenHeight / 2f - margins.bottom;

            // ベクトルを矩形(currentMaxX, currentMaxY)にクリッピングする
            var clippedPoint = ClipToRectangle(directionFromCenter, currentMaxX, currentMaxY);

            // 最終的なアイコンのスクリーン座標 (画面左下原点)
            var iconScreenPosition = screenCenter + clippedPoint;
            
            // --- 4. 回転の計算 ---
            
            // 画面中心からマーカーの位置 (clippedPoint) への角度を計算
            var angle = Mathf.Atan2(clippedPoint.y, clippedPoint.x) * Mathf.Rad2Deg;

            // Z軸回転のQuaternionを作成
            var arrowRotation = Quaternion.Euler(0, 0, angle);

            // --- 5. 矢印の位置の計算 ---
            
            // アイコンの位置から、回転角度(angle)に基づきarrowDistanceFromIconだけ離れた位置
            var arrowOffset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad), 
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * arrowDistanceFromIcon;
            
            var arrowScreenPosition = iconScreenPosition + arrowOffset;
            
            return (iconScreenPosition, arrowScreenPosition, arrowRotation);
        }
        
        /// <summary>
        /// ベクトルを矩形の境界線にクリッピングするヘルパーメソッド
        /// </summary>
        private Vector2 ClipToRectangle(Vector2 direction, float maxX, float maxY)
        {
            if (direction.sqrMagnitude < 0.0001f)
            {
                return Vector2.zero;
            }

            // X軸とY軸方向の比率を計算 (境界に先に当たる軸を見つける)
            float ratioX = maxX / Mathf.Abs(direction.x);
            float ratioY = maxY / Mathf.Abs(direction.y);

            // どちらの軸で境界に先に当たるかを判断 (最も小さい比率を採用)
            float ratio = Mathf.Min(ratioX, ratioY);

            // 境界線にクリッピングされた座標を返す
            return direction * ratio;
        }

        #endregion

        #region View (UI操作ロジック)

        /// <summary>
        /// マーカー全体の表示・非表示を切り替える
        /// </summary>
        private void SetVisibility(bool isVisible)
        {
            if (iconImage != null) iconImage.enabled = isVisible;
            if (arrowImage != null) arrowImage.enabled = isVisible;
        }
        
        /// <summary>
        /// Imageコンポーネントの透明度を設定します。
        /// </summary>
        private void SetAlpha(Image image, float alpha)
        {
            if (image == null) return;
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        /// <summary>
        /// スクリーン座標を元にアイコンの位置を設定
        /// </summary>
        private void SetIconScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(iconRectTransform, screenPosition);
        }

        /// <summary>
        /// スクリーン座標を元に矢印の位置を設定
        /// </summary>
        private void SetArrowScreenPosition(Vector2 screenPosition)
        {
            SetElementPosition(arrowRectTransform, screenPosition);
        }

        /// <summary>
        /// 矢印の回転を設定
        /// </summary>
        private void SetArrowRotation(Quaternion rotation)
        {
            if (arrowRectTransform != null) arrowRectTransform.rotation = rotation;
        }

        /// <summary>
        /// スクリーン座標を、Canvas上のローカル座標に変換してUI要素を配置。
        /// </summary>
        private void SetElementPosition(RectTransform element, Vector2 screenPosition)
        {
            if (element == null || _parentCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.GetComponent<RectTransform>(),
                screenPosition,
                _uiCamera, // Initializeで設定した正しいカメラ情報を使用
                out Vector2 localPoint
            );
            element.anchoredPosition = localPoint;
        }

        #endregion
    }
}