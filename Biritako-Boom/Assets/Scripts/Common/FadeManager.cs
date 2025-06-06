using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class FadeManager : ISceneInfo
    {
        private float _fadeDuration = 3.0f;
        private Image _fadePanel;

        public string SceneName { get; }
        
        /// <summary>
        /// フェードイン（明るくなる）
        /// </summary>
        public async UniTask Init()
        {
            if (_fadePanel)
            {
                var currentColor = _fadePanel.color;
                currentColor.a = 1f;
                _fadePanel.color = currentColor;
                
                _fadePanel.raycastTarget = true;    // フェード中はクリックをブロック
                await FadeCanvasGroup(0f); // 透明度を0までフェードイン
                _fadePanel.raycastTarget = false;   // クリックのブロック解除
            }
        }

        /// <summary>
        /// フェードアウト（暗くなる）
        /// </summary>
        public async UniTask End()
        {
            if (_fadePanel)
            {
                var currentColor = _fadePanel.color;
                currentColor.a = 1f;
                _fadePanel.color = currentColor;
                
                _fadePanel.raycastTarget = true;    // フェード中はクリックをブロック
                await FadeCanvasGroup(1f); // 透明度を0までフェードイン
                _fadePanel.raycastTarget = false;   // クリックのブロック解除
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
        private async UniTask FadeCanvasGroup(float targetAlpha)
        {
            var currentColor = _fadePanel.color;
            var startAlpha = currentColor.a;
            var timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                var progress = Mathf.Clamp01(timer / _fadeDuration);
                currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, progress);
                await UniTask.Yield(); // 1フレーム待機
            }
            currentColor.a = targetAlpha;
            _fadePanel.color = currentColor;
        }
    }
}
