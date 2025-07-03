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
        private WaypointMarkerView _view;
        private WaypointMarkerModel _model = new WaypointMarkerModel();
        
        
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
            // 自身のGameObjectにあるViewコンポーネントを取得
            _view = GetComponent<WaypointMarkerView>();
            if (_view == null)
            {
                Debug.LogError("同じGameObjectにWaypointMarkerViewが見つかりません！", this);
                enabled = false;
                return;
            }
            
            // ViewとModelの初期化
            _view.Initialize();
            _model.SetMainCamera(Camera.main);
        }
        
        private void Start()
        {
            // 初期状態では非表示
            _view.SetVisibility(false);
        }


        private void LateUpdate()
        {
            // Modelにターゲットの可視性判定を依頼
            var (isOnScreen, screenPosition) = _model.CheckTargetVisibility();

            // 結果をViewに反映
            _view.SetVisibility(!isOnScreen);

            // マーカーを表示する場合
            if (!isOnScreen)
            {
                // Viewから見た目の設定を取得
                var margins = _view.GetMargins();
                var distance = _view.GetArrowDistanceFromIcon();

                // Modelにマーカーの位置・回転計算を依頼
                var (iconPos, arrowPos, arrowRot) = _model.CalculateMarkerTransform(screenPosition, margins, distance);

                // 計算結果をViewの更新メソッドに渡す
                _view.SetIconScreenPosition(iconPos);
                _view.SetArrowScreenPosition(arrowPos);
                _view.SetArrowRotation(arrowRot);
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
                _model.SetTarget(motherShip.gameObject.transform);
            }
        }
    }
}
