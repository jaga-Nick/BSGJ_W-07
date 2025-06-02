using Cysharp.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// シーン情報。
    /// </summary>
    public interface ISceneInfo
    {
        public string SceneName { get; }

        /// <summary>
        /// デフォルトシーンかどうか
        /// </summary>
        /// public bool IsDefault { get; }

        /// <summary>
        /// ゲーム開始時等の処理を走らせる場合。
        /// </summary>
        /// <returns></returns>
        public UniTask Init();
        
        /// <summary>
        /// シーン終了時
        /// </summary>
        public UniTask End();

        /// <summary>
        /// InputSystem起動
        /// </summary>
        public void InputStart();
        
        /// <summary>
        /// InputSystem停止
        /// </summary>
        public void InputStop();
    }
}
