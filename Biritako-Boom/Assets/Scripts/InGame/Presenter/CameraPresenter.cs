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
            _cameraModel = new CameraModel();
            _cameraModel.Initialize(new Vector3(0, 0, offsetZ));
        }

        /// <summary>
        /// 外部から追従対象を設定するための公開メソッド（これが重要）
        /// </summary>
        /// <param name="targetTransform">追従したい対象のTransform</param>
        public void SetTarget(Transform targetTransform)
        {
            _target = targetTransform;
        }

        private void LateUpdate()
        {
            // ターゲットが設定されていなければ何もしない
            if (_target == null)
            {
                return;
            }

            // --- ターゲットを探すロジックはもう不要 ---

            // ターゲットが設定されていれば、シンプルに追従処理を行うだけ
            Vector3 targetPosition = _target.position;
            Vector3 newPosition = targetPosition + _cameraModel.GetOffset();
                            
            cameraView.SetPosition(newPosition);
        }
    }
}