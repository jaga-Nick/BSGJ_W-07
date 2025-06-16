// InGame/Model/CameraModel.cs

using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// カメラのデータを保持するクラス
    /// </summary>
    public class CameraModel
    {
        // --- 基本データ ---
        private Vector3 _offset;

        // --- エフェクト用データ (privateフィールド) ---
        private bool _isPerlinNoiseShakeEnabled;
        private float _perlinNoiseStrength;
        private Vector3 _effectOffset;
        private bool _isColorblindModeEnabled;
        private int _colorblindType;
        private bool _isFocusEnabled;
        private Vector4 _focusPoint;

        /// <summary>
        /// Modelの状態を初期化する
        /// </summary>
        public void Initialize(Vector3 offset)
        {
            _offset = offset;

            // エフェクト関連の初期化
            _effectOffset = Vector3.zero;
            _isPerlinNoiseShakeEnabled = false;
            _isColorblindModeEnabled = false;
            _isFocusEnabled = false;
        }

        // --- アクセサメソッド ---

        public Vector3 GetOffset() { return _offset; }
        
        public bool GetIsPerlinNoiseShakeEnabled() { return _isPerlinNoiseShakeEnabled; }
        public void SetIsPerlinNoiseShakeEnabled(bool value) { _isPerlinNoiseShakeEnabled = value; }

        public float GetPerlinNoiseStrength() { return _perlinNoiseStrength; }
        public void SetPerlinNoiseStrength(float value) { _perlinNoiseStrength = value; }

        public Vector3 GetEffectOffset() { return _effectOffset; }
        public void SetEffectOffset(Vector3 value) { _effectOffset = value; }

        public bool GetIsColorblindModeEnabled() { return _isColorblindModeEnabled; }
        public void SetIsColorblindModeEnabled(bool value) { _isColorblindModeEnabled = value; }

        public int GetColorblindType() { return _colorblindType; }
        public void SetColorblindType(int value) { _colorblindType = value; }

        public bool GetIsFocusEnabled() { return _isFocusEnabled; }
        public void SetIsFocusEnabled(bool value) { _isFocusEnabled = value; }

        public Vector4 GetFocusPoint() { return _focusPoint; }
        public void SetFocusPoint(Vector4 value) { _focusPoint = value; }
    }
}