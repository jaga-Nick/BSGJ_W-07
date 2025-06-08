// InGame/View/CameraView.cs

using UnityEngine;

namespace InGame.View
{
    /// <summary>
    /// カメラの見た目（Transform）を操作するクラス
    /// </summary>
    public class CameraView : MonoBehaviour
    {
        /// <summary>
        /// 外部からカメラの位置を設定するためのアクセサメソッド
        /// </summary>
        /// <param name="newPosition">カメラの新しい座標</param>
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }
    }
}