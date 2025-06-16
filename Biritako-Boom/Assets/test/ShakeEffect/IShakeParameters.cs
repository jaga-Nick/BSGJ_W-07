using UnityEngine;

namespace ShakeEffect
{
    /// <summary>
    /// シェイクのパラメータに関するインターフェースです。
    /// </summary>
    public interface IShakeParameters
    {
        /// <summary>
        /// シェイクの種類（ワンショットまたは持続）を取得または設定します。
        /// </summary>
        ShakeType GetShakeType();
        void SetShakeType(ShakeType type);

        /// <summary>
        /// シェイクの強度・大きさを取得または設定します。
        /// </summary>
        float GetStrength();
        void SetStrength(float strength);

        /// <summary>
        /// シェイクの荒々しさを取得または設定します。
        /// 値が低いほど遅く滑らかになり、高いほど速くノイジーになります。
        /// </summary>
        float GetRoughness();
        void SetRoughness(float roughness);

        /// <summary>
        /// シェイクがフェードインするのにかかる時間（秒）を取得または設定します。
        /// </summary>
        float GetFadeIn();
        void SetFadeIn(float fadeIn);

        /// <summary>
        /// シェイクがフェードアウトするのにかかる時間（秒）を取得または設定します。
        /// </summary>
        float GetFadeOut();
        void SetFadeOut(float fadeOut);

        /// <summary>
        /// シェイクがカメラの位置に与える影響の度合いを取得または設定します。
        /// </summary>
        Vector3 GetPositionInfluence();
        void SetPositionInfluence(Vector3 influence);

        /// <summary>
        /// シェイクがカメラの回転に与える影響の度合いを取得または設定します。
        /// </summary>
        Vector3 GetRotationInfluence();
        void SetRotationInfluence(Vector3 influence);
    }
}