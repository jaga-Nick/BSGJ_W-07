using InGame.Model;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;

namespace InGame.Presenter
{
    public class ElectronicsPresenter : MonoBehaviour
    {
        /// <summary>
        /// modelとview
        /// </summary>
        private ElectronicsModel _model;
        private ElectronicsView _view;
        
        public void Start()
        {
            _model = new ElectronicsModel();
            _view = gameObject.AddComponent<ElectronicsView>();
        }

        /// <summary>
        /// Enemyをスポーンする座標を決める
        /// </summary>
        public Vector3 DetermineSpawnPoints()
        {
            // 座標を取得する
            var position = new Vector3(0, 0, 0);
            return position;
        }

        private void Update()
        {
            // 移動ロジックを実装
            var newPosition = _model.Position + transform.position * (_model.Speed * Time.deltaTime);
            _model.SetPositon(newPosition);
            _view.UpdatePosition(newPosition);
            
            // 移動アニメーションの制御
            _view.PlayMoveAnimation(_model.Speed > 0);
            
            // 死んだ場合
        }
    }
}