using UnityEngine;

namespace Common
{
    /// <summary>
    /// InputSystem_Actions
    /// </summary>
    public class InputSystemActionsManager : SingletonMonoBehaviourBase<InputSystemActionsManager>
    {
        private InputSystem_Actions _InputSystemActions;
        /// <summary>
        /// アクションマップの情報を取得する
        /// </summary>
        /// <returns></returns>
        public InputSystem_Actions GetInputSystem_Actions()
        {
            if (_InputSystemActions == null)
            {
                _InputSystemActions = new InputSystem_Actions();
            }
            return _InputSystemActions;
        }

        /// <summary>
        /// Player入力の有効化
        /// </summary>
        public void PlayerEnable()
        {
            _InputSystemActions?.Player.Enable();
            _InputSystemActions?.UI.Disable();
        }

        /// <summary>
        /// UI入力の有効化
        /// </summary>
        public void UIEnable()
        {
            _InputSystemActions?.Player.Disable();
            _InputSystemActions?.UI.Enable();
        }

        /// <summary>
        /// Player入力の無効化
        /// </summary>
        public void PlayerDisable()
        {
            _InputSystemActions?.Player.Disable();
            Debug.Log("プレイヤー操作無効化");
        }

        /// <summary>
        /// UI入力の有効化
        /// </summary>
        public void UIDisable()
        {
            _InputSystemActions?.UI.Disable();
            Debug.Log("UI操作無効化");
        }
    }
}