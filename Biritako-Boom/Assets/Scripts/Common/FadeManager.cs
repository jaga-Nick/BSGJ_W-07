using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class FadeManager : ISceneInfo
    {
        private float _fadeDuration = 0.5f;
        private Image _fadePanel;

        public string SceneName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fadePanel"></param>
        public FadeManager(Image fadePanel)
        {
            _fadePanel = fadePanel;
            if (_fadePanel)
            {
                var initialColor = _fadePanel.color;
                initialColor.a = 0f;
                _fadePanel.color = initialColor;
                _fadePanel.raycastTarget = false;
            }
            else
            {
                Debug.LogError("FadeManager: fadePanelがnullです。Canvas上のImageコンポーネントをアタッチしてください。");
            }
        }
        
        /// <summary>
        /// フェードイン（明るくなる）
        /// </summary>
        public async UniTask Init()
        {
            Debug.Log($"Scene: {SceneName} Init started (Image Alpha Only).");
            
            if (_fadePanel)
            {
                var currentColor = _fadePanel.color;
                currentColor.a = 1f;
                _fadePanel.color = currentColor;
                
                _fadePanel.raycastTarget = true;  // フェード中はクリックをブロック
                await FadeImageAlpha(0f);         // 透明度を0までフェードイン
                _fadePanel.raycastTarget = false; // クリックのブロック解除
            }
        }

        /// <summary>
        /// フェードアウト（暗くなる）
        /// </summary>
        public async UniTask End()
        {
            Debug.Log($"Scene: {SceneName} End started (Image Alpha Only).");
            
            if (_fadePanel)
            {
                var currentColor = _fadePanel.color;
                currentColor.a = 0f;
                _fadePanel.color = currentColor;
                
                _fadePanel.raycastTarget = true; // フェード中はクリックをブロック
                await FadeImageAlpha(1f);        // 透明度を0までフェードイン
            }
        }

        public void InputStart()
        {
            throw new System.NotImplementedException();
        }

        public void InputStop()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// フェードイン・フェードアウトをする。
        /// CanvasGroupのアルファ値を指定された目標値まで時間で変更する。（DOTween不使用）
        /// </summary>
        /// <param name="targetAlpha"></param>
        private async UniTask FadeImageAlpha(float targetAlpha)
        {
            var currentColor = _fadePanel.color;
            var startAlpha = currentColor.a;
            var timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                var progress = Mathf.Clamp01(timer / _fadeDuration);
                
                // 現在のRGBを取得して、アルファ値のみを補間する
                currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, progress);
                _fadePanel.color = currentColor;
                
                await UniTask.Yield();
            }
            currentColor.a = targetAlpha;
            _fadePanel.color = currentColor;
        }
    }
}
