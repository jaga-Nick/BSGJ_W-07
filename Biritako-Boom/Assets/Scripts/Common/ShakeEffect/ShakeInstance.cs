using System;
using Common.ShakeEffect;
using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// シェイクインスタンスのデータを保持するコンテナです。
    /// </summary>
    [Serializable]
    public class ShakeInstance
    {
        private ShakeParameters shakeParameters;
        private float strengthScale;
        private float roughnessScale;
        private bool removeWhenStopped;
        private ShakeState state;
        private bool isPaused;
        
        private int baseSeed;
        private float seed1, seed2, seed3;
        private float noiseTimer;
        private float fadeTimer;
        private float fadeInTime;
        private float fadeOutTime;
        private float pauseTimer;
        private float pauseFadeTime;
        private int lastUpdatedFrame;

        // --- アクセサメソッド ---
        public ShakeParameters GetShakeParameters() { return shakeParameters; }
        public void SetShakeParameters(ShakeParameters value) { shakeParameters = value; }
        public float GetStrengthScale() { return strengthScale; }
        public void SetStrengthScale(float value) { strengthScale = value; }
        public float GetRoughnessScale() { return roughnessScale; }
        public void SetRoughnessScale(float value) { roughnessScale = value; }
        public bool GetRemoveWhenStopped() { return removeWhenStopped; }
        public void SetRemoveWhenStopped(bool value) { removeWhenStopped = value; }
        public ShakeState GetState() { return state; }
        private void SetState(ShakeState value) { state = value; }
        public bool IsPaused() { return isPaused; }
        private void SetIsPaused(bool value) { isPaused = value; }

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
            return GetShakeParameters().GetStrength() * fadeTimer * GetStrengthScale();
        }

        /// <summary>
        /// 現在のシェイクの荒々しさを取得します。
        /// </summary>
        public float GetCurrentRoughness()
        {
            return GetShakeParameters().GetRoughness() * fadeTimer * GetRoughnessScale();
        }

        public ShakeInstance(int? seed = null)
        {
            if (!seed.HasValue)
                seed = UnityEngine.Random.Range(-10000, 10000);

            baseSeed = seed.Value;
            seed1 = baseSeed / 2f;
            seed2 = baseSeed / 3f;
            seed3 = baseSeed / 4f;

            noiseTimer = this.baseSeed;
            fadeTimer = 0;
            pauseTimer = 0;

            SetStrengthScale(1);
            SetRoughnessScale(1);
        }

        public ShakeInstance(IShakeParameters shakeData, int? seed = null) : this(seed)
        {
            SetShakeParameters(new ShakeParameters(shakeData));
            fadeInTime = shakeData.GetFadeIn();
            fadeOutTime = shakeData.GetFadeOut();
            SetState(ShakeState.FadingIn);
        }

        public ShakeResult UpdateShake(float deltaTime)
        {
            ShakeResult result = new ShakeResult();
            
            result.PositionShake = getPositionShake();
            result.RotationShake = getRotationShake();
            
            if (Time.frameCount == lastUpdatedFrame)
                return result;

            if (pauseFadeTime > 0)
            {
                if (IsPaused())
                    pauseTimer += deltaTime / pauseFadeTime;
                else
                    pauseTimer -= deltaTime / pauseFadeTime;
            }
            pauseTimer = Mathf.Clamp01(pauseTimer);
            
            noiseTimer += (1 - pauseTimer) * deltaTime * GetCurrentRoughness();
            
            if (GetState() == ShakeState.FadingIn)
            {
                if (fadeInTime > 0)
                    fadeTimer += deltaTime / fadeInTime;
                else
                    fadeTimer = 1;
            }
            else if (GetState() == ShakeState.FadingOut)
            {
                if (fadeOutTime > 0)
                    fadeTimer -= deltaTime / fadeOutTime;
                else
                    fadeTimer = 0;
            }
            fadeTimer = Mathf.Clamp01(fadeTimer);
            
            if (fadeTimer >= 1)
            {
                if (GetShakeParameters().GetShakeType() == ShakeType.Sustained)
                    SetState(ShakeState.Sustained);
                else if (GetShakeParameters().GetShakeType() == ShakeType.OneShot)
                    Stop(GetShakeParameters().GetFadeOut(), true);
            }
            else if (fadeTimer <= 0)
            {
                SetState(ShakeState.Stopped);
            }

            lastUpdatedFrame = Time.frameCount;

            return result;
        }

        public void Start(float fadeTime)
        {
            this.fadeInTime = fadeTime;
            SetState(ShakeState.FadingIn);
        }

        public void Stop(float fadeTime, bool removeWhenStopped)
        {
            this.fadeOutTime = fadeTime;
            SetRemoveWhenStopped(removeWhenStopped);
            SetState(ShakeState.FadingOut);
        }

        public void Pause(float fadeTime)
        {
            SetIsPaused(true);
            pauseFadeTime = fadeTime;
            if (fadeTime <= 0)
                pauseTimer = 1;
        }
        
        public void Resume(float fadeTime)
        {
            SetIsPaused(false);
            pauseFadeTime = fadeTime;
            if (fadeTime <= 0)
                pauseTimer = 0;
        }

        public void TogglePaused(float fadeTime)
        {
            if (IsPaused())
                Resume(fadeTime);
            else
                Pause(fadeTime);
        }

        private Vector3 getPositionShake()
        {
            Vector3 v = Vector3.zero;
            v.x = getNoise(noiseTimer + seed1, baseSeed);
            v.y = getNoise(baseSeed, noiseTimer);
            v.z = getNoise(seed3 + noiseTimer, baseSeed + noiseTimer);
            return Vector3.Scale(v * GetCurrentStrength(), GetShakeParameters().GetPositionInfluence());
        }

        private Vector3 getRotationShake()
        {
            Vector3 v = Vector3.zero;
            v.x = getNoise(noiseTimer - baseSeed, seed3);
            v.y = getNoise(baseSeed, noiseTimer + seed2);
            v.z = getNoise(baseSeed + noiseTimer, seed1 + noiseTimer);
            return Vector3.Scale(v * GetCurrentStrength(), GetShakeParameters().GetRotationInfluence());
        }

        private float getNoise(float x, float y)
        {
            return (Mathf.PerlinNoise(x, y) - 0.5f) * 2f;
        }
    }
}