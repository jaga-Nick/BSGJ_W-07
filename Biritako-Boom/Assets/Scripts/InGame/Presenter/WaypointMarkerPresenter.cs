using System;
using InGame.Model;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;

namespace InGame.Presenter
{
    /// <summary>
    /// WaypointマーカーのModelとViewを仲介するPresenter。
    /// </summary>
    public class WaypointMarkerPresenter : MonoBehaviour
    {
        private WaypointMarkerView view;
        private WaypointMarkerModel model = new WaypointMarkerModel();

        [SerializeField] private Camera camera;
        
        
        private void OnEnable()
        {
            EnemySpawner.OnGenerateMotherShip += FindAndSetTarget;
        }
        
        private void OnDisable()
        {
            EnemySpawner.OnGenerateMotherShip -= FindAndSetTarget;
        }
        
        private void Awake()
        {
            // 自身のGameObjectにあるViewコンポーネントを取得
            view = GetComponent<WaypointMarkerView>();
            if (view == null)
            {
                Debug.LogError("同じGameObjectにWaypointMarkerViewが見つかりません！", this);
                enabled = false;
                return;
            }
            
            // ViewとModelの初期化
            view.Initialize();
            if (camera == null)
            {
                model.SetMainCamera(Camera.main);
            }
            else
            {
                model.SetMainCamera(camera);
            }
        }
        
        private void Start()
        {
            // 初期状態では非表示
            view.SetVisibility(false);
        }


        private void LateUpdate()
        {
            
            // Modelにターゲットの可視性判定を依頼
            var (isOnScreen, screenPosition) = model.CheckTargetVisibility();

            // 結果をViewに反映
            view.SetVisibility(!isOnScreen);

            // マーカーを表示する場合
            if (!isOnScreen)
            {
                // Viewから見た目の設定を取得
                var margins = view.GetMargins();
                var distance = view.GetArrowDistanceFromIcon();

                // Modelにマーカーの位置・回転計算を依頼
                var (iconPos, arrowPos, arrowRot) = model.CalculateMarkerTransform(screenPosition, margins, distance);

                // 計算結果をViewの更新メソッドに渡す
                view.SetIconScreenPosition(iconPos);
                view.SetArrowScreenPosition(arrowPos);
                view.SetArrowRotation(arrowRot);
            }
        }
        

        /// <summary>
        /// EnemySpawnerのイベントから呼び出され、ターゲットをModelに設定
        /// </summary>
        private void FindAndSetTarget()
        {
            var motherShip = FindObjectOfType<MotherShipPresenter>();
            if (motherShip != null)
            {
                // 見つけたターゲットをModelに設定
                model.SetTarget(motherShip.gameObject.transform);
            }
        }
    }
}
