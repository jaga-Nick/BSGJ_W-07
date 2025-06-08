// InGame/Presenter/CameraPresenter.cs

using InGame.Model;
using InGame.View;
using UnityEngine;

namespace InGame.Presenter
{
    /// <summary>
    /// カメラのロジックを制御するクラス
    /// </summary>
    public class CameraPresenter : MonoBehaviour
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private float offsetZ = -10f;

        private CameraModel _cameraModel;
        private Transform _target;

        void Start()
        {
            // インスタンスの生成と初期化を分離する
            // 1. まずインスタンスを生成する
            _cameraModel = new CameraModel();
            
            // 2. 次にInitializeメソッドで初期値を設定する
            _cameraModel.Initialize(new Vector3(0, 0, offsetZ));
        }

        public void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 targetPosition = _target.position;
            Vector3 newPosition = targetPosition + _cameraModel.GetOffset();
            
            cameraView.SetPosition(newPosition);
        }
    }
}