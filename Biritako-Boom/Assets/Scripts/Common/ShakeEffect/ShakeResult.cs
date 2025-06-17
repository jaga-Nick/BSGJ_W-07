using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// シェイクによる位置と回転の情報を保持するためのデータ構造です。
    /// </summary>
    public struct ShakeResult
    {
        /// <summary>
        /// 位置の揺れ量。
        /// </summary>
        public Vector3 PositionShake;
        /// <summary>
        /// 回転の揺れ量。
        /// </summary>
        public Vector3 RotationShake;

        public static ShakeResult operator +(ShakeResult a, ShakeResult b)
        {
            ShakeResult c = new ShakeResult();
            c.PositionShake = a.PositionShake + b.PositionShake;
            c.RotationShake = a.RotationShake + b.RotationShake;
            return c;
        }
    }
}