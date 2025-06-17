using System;
using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// IShakeParametersインターフェースの実装です。
    /// </summary>
    [Serializable]
    public class ShakeParameters : IShakeParameters
    {
        [Header("シェイクの種類")]
        [SerializeField]
        private ShakeType shakeType;

        [Header("シェイクの強さ")]
        [SerializeField]
        private float strength;
        [SerializeField]
        private float roughness;

        [Header("フェード")]
        [SerializeField]
        private float fadeIn;
        [SerializeField]
        private float fadeOut;

        [Header("シェイクの影響度")]
        [SerializeField]
        private Vector3 positionInfluence;
        [SerializeField]
        private Vector3 rotationInfluence;

        public ShakeParameters() { }

        public ShakeParameters(IShakeParameters original)
        {
            shakeType = original.GetShakeType();
            strength = original.GetStrength();
            roughness = original.GetRoughness();
            fadeIn = original.GetFadeIn();
            fadeOut = original.GetFadeOut();
            positionInfluence = original.GetPositionInfluence();
            rotationInfluence = original.GetRotationInfluence();
        }

        public ShakeType GetShakeType() { return shakeType; }
        public void SetShakeType(ShakeType value) { shakeType = value; }

        public float GetStrength() { return strength; }
        public void SetStrength(float value) { strength = value; }

        public float GetRoughness() { return roughness; }
        public void SetRoughness(float value) { roughness = value; }

        public float GetFadeIn() { return fadeIn; }
        public void SetFadeIn(float value) { fadeIn = value; }

        public float GetFadeOut() { return fadeOut; }
        public void SetFadeOut(float value) { fadeOut = value; }

        public Vector3 GetPositionInfluence() { return positionInfluence; }
        public void SetPositionInfluence(Vector3 value) { positionInfluence = value; }

        public Vector3 GetRotationInfluence() { return rotationInfluence; }
        public void SetRotationInfluence(Vector3 value) { rotationInfluence = value; }
    }
}