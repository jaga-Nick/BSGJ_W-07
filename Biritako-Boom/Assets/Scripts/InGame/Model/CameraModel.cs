// InGame/Model/CameraModel.cs

using UnityEngine;

namespace InGame.Model
{
    /// <summary>
    /// カメラのデータを保持するクラス
    /// </summary>
    public class CameraModel
    {
        // フィールドはprivateにして、直接のアクセスを禁止する
        private Vector3 _offset;
        
        /// <summary>
        /// Modelの状態を初期化する
        /// </summary>
        /// <param name="offset">プレイヤーからの相対的な位置</param>
        public void Initialize(Vector3 offset)
        {
            _offset = offset;
        }

        /// <summary>
        /// Offsetの値を取得するためのアクセサメソッド
        /// </summary>
        /// <returns>プレイヤーからの相対的な位置</returns>
        public Vector3 GetOffset()
        {
            return _offset;
        }
    }
}