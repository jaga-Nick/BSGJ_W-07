using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// IShakeParametersインターフェースの、ScriptableObjectアセットによる実装です。
    /// </summary>
    [CreateAssetMenu(fileName = "New Shake Preset", menuName = "Shake Effect/Shake Preset")]
    public class ShakePreset : ScriptableObject, IShakeParameters
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