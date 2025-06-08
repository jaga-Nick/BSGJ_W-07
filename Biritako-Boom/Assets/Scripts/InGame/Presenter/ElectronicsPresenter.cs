using System;
using InGame.Model;
using InGame.NonMVP;
using InGame.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Presenter
{
    public class ElectronicsPresenter : MonoBehaviour
    {
        /// <summary>
        /// modelとview
        /// </summary>
        private ElectronicsModel _model;
        private ElectronicsView _view;
        private UfoModel _ufoModel;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void Start()
        {
            _model = new ElectronicsModel
            {
                Rb = gameObject.GetComponent<Rigidbody2D>()
            };
            _view = gameObject.GetComponent<ElectronicsView>();
        }

        /// <summary>
        /// Enemyをスポーンする座標を決める
        /// </summary>
        public Vector3 DetermineSpawnPoints()
        {
            // UFOのmodelを取得して座標を確認
            // _ufoModel = new UfoModel();
            
            // ランダムな座標を生成
            var randomPositionX = RandomRun();
            var randomPositionY = RandomRun();

            // 画面外の座標を取得
            var position = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionX, randomPositionY, _camera.nearClipPlane));
            return position;
        }

        /// <summary>
        /// ランダムな値を取得
        /// </summary>
        /// <returns></returns>
        private static float RandomRun()
        {
            float value;
            do { value = Random.Range(-0.5f, 1.5f); } while (value is >= 0.0f and <= 1.0f);
            return value;
        }

        private void Update()
        {
            // 移動ロジックを実装
            var newPosition = _model.Position + transform.position * (_model.Speed * Time.deltaTime);
            _model.SetPositon(newPosition);
            _model.Move();
            
            // 移動アニメーションの制御
            _view.PlayMoveAnimation(_model.Speed > 0);
            
            // 死んだ場合
        }
    }
}