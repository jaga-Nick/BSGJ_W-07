namespace InGame.Model.States
{
    /// <summary>
    /// 母艦UFOの状態を表すインターフェース
    /// </summary>
    public interface IMotherShipState
    {
        /// <summary>
        /// このステートに遷移した瞬間の処理
        /// </summary>
        void OnEnter();

        /// <summary>
        /// このステートの間、一定間隔で呼ばれる更新処理
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// このステートから出る瞬間の処理
        /// </summary>
        void OnExit();
    }
}