namespace ShakeEffect
{
    /// <summary>
    /// シェイクの振る舞いを定義します。
    /// </summary>
    public enum ShakeType
    {
        /// <summary>
        /// 自動的にフェードインし、フェードアウトします。
        /// </summary>
        OneShot,

        /// <summary>
        /// 自動的にフェードインしますが、揺れ続け、停止を指示されるまでフェードアウトしません。
        /// </summary>
        Sustained
    }
}