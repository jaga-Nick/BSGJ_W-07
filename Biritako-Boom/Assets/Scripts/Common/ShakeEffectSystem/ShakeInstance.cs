using System;
using UnityEngine;

namespace Common.ShakeEffectSetting
{
    /// <summary>
    /// シェイクインスタンスのデータを保持するコンテナです。
    /// </summary>
    [Serializable]
    public class ShakeInstance
    {
        private ShakeParameters _shakeParameters;
        private float _strengthScale;
        private float _roughnessScale;
        private bool _removeWhenStopped;
        private ShakeState _state;
        private bool _isPaused;
        
        private int _baseSeed;
        private float _seed1, _seed2, _seed3;
        private float _noiseTimer;
        private float _fadeTimer;
        private float _fadeInTime;
        private float _fadeOutTime;
        private float _pauseTimer;
        private float _pauseFadeTime;
        private int _lastUpdatedFrame;

        // --- アクセサメソッド ---
        public ShakeParameters GetShakeParameters() { return _shakeParameters; }
        public void SetShakeParameters(ShakeParameters value) { _shakeParameters = value; }
        public float GetStrengthScale() { return _strengthScale; }
        public void SetStrengthScale(float value) { _strengthScale = value; }
        public float GetRoughnessScale() { return _roughnessScale; }
        public void SetRoughnessScale(float value) { _roughnessScale = value; }
        public bool GetRemoveWhenStopped() { return _removeWhenStopped; }
        public void SetRemoveWhenStopped(bool value) { _removeWhenStopped = value; }
        public ShakeState GetState() { return _state; }
        private void SetState(ShakeState value) { _state = value; }
        public bool IsPaused() { return _isPaused; }
        private void SetIsPaused(bool value) { _isPaused = value; }

        /// <summary>
        /// シェイクが完全に終了したかどうかを取得します。
        /// </summary>
        public bool IsFinished()
        {
            return GetState() == ShakeState.Stopped && GetRemoveWhenStopped();
        }

        /// <summary>
        /// 現在のシェイクの強度を取得します。
        /// </summary>
        public float GetCurrentStrength()
        {
            return GetShakeParameters().GetStrength() * _fadeTimer * GetStrengthScale();
        }

        /// <summary>
        /// 現在のシェイクの荒々しさを取得します。
        /// </summary>
        public float GetCurrentRoughness()
        {
            return GetShakeParameters().GetRoughness() * _fadeTimer * GetRoughnessScale();
        }

        public ShakeInstance(int? seed = null)
        {
            if (!seed.HasValue)
                seed = UnityEngine.Random.Range(-10000, 10000);

            _baseSeed = seed.Value;
            _seed1 = _baseSeed / 2f;
            _seed2 = _baseSeed / 3f;
            _seed3 = _baseSeed / 4f;

            _noiseTimer = this._baseSeed;
            _fadeTimer = 0;
            _pauseTimer = 0;

            SetStrengthScale(1);
            SetRoughnessScale(1);
        }

        public ShakeInstance(IShakeParameters shakeData, int? seed = null) : this(seed)
        {
            SetShakeParameters(new ShakeParameters(shakeData));
            _fadeInTime = shakeData.GetFadeIn();
            _fadeOutTime = shakeData.GetFadeOut();
            SetState(ShakeState.FadingIn);
        }

        public ShakeResult UpdateShake(float deltaTime)
        {
            ShakeResult result = new ShakeResult();
            
            result.PositionShake = GetPositionShake();
            result.RotationShake = GetRotationShake();
            
            if (Time.frameCount == _lastUpdatedFrame)
                return result;

            if (_pauseFadeTime > 0)
            {
                if (IsPaused())
                    _pauseTimer += deltaTime / _pauseFadeTime;
                else
                    _pauseTimer -= deltaTime / _pauseFadeTime;
            }
            _pauseTimer = Mathf.Clamp01(_pauseTimer);
            
            _noiseTimer += (1 - _pauseTimer) * deltaTime * GetCurrentRoughness();
            
            if (GetState() == ShakeState.FadingIn)
            {
                if (_fadeInTime > 0)
                    _fadeTimer += deltaTime / _fadeInTime;
                else
                    _fadeTimer = 1;
            }
            else if (GetState() == ShakeState.FadingOut)
            {
                if (_fadeOutTime > 0)
                    _fadeTimer -= deltaTime / _fadeOutTime;
                else
                    _fadeTimer = 0;
            }
            _fadeTimer = Mathf.Clamp01(_fadeTimer);
            
            if (_fadeTimer >= 1)
            {
                if (GetShakeParameters().GetShakeType() == ShakeType.Sustained)
                    SetState(ShakeState.Sustained);
                else if (GetShakeParameters().GetShakeType() == ShakeType.OneShot)
                    Stop(GetShakeParameters().GetFadeOut(), true);
            }
            else if (_fadeTimer <= 0)
            {
                SetState(ShakeState.Stopped);
            }

            _lastUpdatedFrame = Time.frameCount;

            return result;
        }

        public void Start(float fadeTime)
        {
            this._fadeInTime = fadeTime;
            SetState(ShakeState.FadingIn);
        }

        public void Stop(float fadeTime, bool removeWhenStopped)
        {
            this._fadeOutTime = fadeTime;
            SetRemoveWhenStopped(removeWhenStopped);
            SetState(ShakeState.FadingOut);
        }

        public void Pause(float fadeTime)
        {
            SetIsPaused(true);
            _pauseFadeTime = fadeTime;
            if (fadeTime <= 0)
                _pauseTimer = 1;
        }
        
        public void Resume(float fadeTime)
        {
            SetIsPaused(false);
            _pauseFadeTime = fadeTime;
            if (fadeTime <= 0)
                _pauseTimer = 0;
        }

        public void TogglePaused(float fadeTime)
        {
            if (IsPaused())
                Resume(fadeTime);
            else
                Pause(fadeTime);
        }

        private Vector3 GetPositionShake()
        {
            Vector3 v = Vector3.zero;
            v.x = GetNoise(_noiseTimer + _seed1, _baseSeed);
            v.y = GetNoise(_baseSeed, _noiseTimer);
            v.z = GetNoise(_seed3 + _noiseTimer, _baseSeed + _noiseTimer);
            return Vector3.Scale(v * GetCurrentStrength(), GetShakeParameters().GetPositionInfluence());
        }

        private Vector3 GetRotationShake()
        {
            Vector3 v = Vector3.zero;
            v.x = GetNoise(_noiseTimer - _baseSeed, _seed3);
            v.y = GetNoise(_baseSeed, _noiseTimer + _seed2);
            v.z = GetNoise(_baseSeed + _noiseTimer, _seed1 + _noiseTimer);
            return Vector3.Scale(v * GetCurrentStrength(), GetShakeParameters().GetRotationInfluence());
        }

        private float GetNoise(float x, float y)
        {
            return (Mathf.PerlinNoise(x, y) - 0.5f) * 2f;
        }
    }
}