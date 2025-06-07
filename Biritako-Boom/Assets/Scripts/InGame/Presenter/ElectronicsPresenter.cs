using InGame.Model;
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

        /// <summary>
        /// イニシャライズ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view"></param>
        public void Init(ElectronicsModel model, ElectronicsView view)
        {
            this._model = model;
            this._view = view;
        }

        private void Update()
        {
            // 敵の移動ロジックを実装
            Vector3 newPosition = this._view.transform.position;
            
            // 移動アニメーションの制御
            
            // 敵が死んだ場合
        }
    }
}