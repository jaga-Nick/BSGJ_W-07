using Common.GameSystem;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// InputSystem_Actions
    /// </summary>
    public class InputSystemActionsManager : SingletonMonoBehaviourBase<InputSystemActionsManager>
    {
        private InputSystem_Actions _inputSystemActions;
        /// <summary>
        /// アクションマップの情報を取得する
        /// </summary>
        /// <returns></returns>
        public InputSystem_Actions GetInputSystem_Actions()
        {
            if (_inputSystemActions == null)
            {
                _inputSystemActions = new InputSystem_Actions();
            }
            return _inputSystemActions;
        }

        /// <summary>
        /// Player入力の有効化
        /// </summary>
        public void PlayerEnable()
        {
            if (_inputSystemActions == null)
            {
                _inputSystemActions = new InputSystem_Actions();
            }
            _inputSystemActions?.Player.Enable();
            _inputSystemActions?.UI.Disable();
        }

        /// <summary>
        /// UI入力の有効化
        /// </summary>
        public void UIEnable()
        {
            if (_inputSystemActions == null)
            {
                _inputSystemActions = new InputSystem_Actions();
            }
            _inputSystemActions?.Player.Disable();
            _inputSystemActions?.UI.Enable();
        }

        /// <summary>
        /// Player入力の無効化
        /// </summary>
        public void PlayerDisable()
        {
            if (_inputSystemActions == null)
            {
                _inputSystemActions = new InputSystem_Actions();
            }
            _inputSystemActions?.Player.Disable();
            Debug.Log("プレイヤー操作無効化");
        }

        /// <summary>
        /// UI入力の有効化
        /// </summary>
        public void UIDisable()
        {
            if (_inputSystemActions == null)
            {
                _inputSystemActions = new InputSystem_Actions();
            }
            _inputSystemActions?.UI.Disable();
            Debug.Log("UI操作無効化");
        }
    }
}