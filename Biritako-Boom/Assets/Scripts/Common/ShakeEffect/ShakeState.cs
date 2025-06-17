namespace ShakeEffect
{
    /// <summary>
    /// シェイクの現在の状態を表します。
    /// </summary>
    public enum ShakeState
    {
        /// <summary>
        /// シェイクが開始中／フェードイン中。
        /// </summary>
        FadingIn = 0,

        /// <summary>
        /// シェイクが最大の強度に達し、一定に揺れている状態。
        /// </summary>
        Sustained = 1,

        /// <summary>
        /// シェイクが停止中／フェードアウト中。
        /// </summary>
        FadingOut = 2,

        /// <summary>
        /// シェイクが完全に停止した状態。
        /// </summary>
        Stopped = 3
    }
}