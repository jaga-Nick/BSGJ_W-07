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
        /// �A�N�V�����}�b�v�̏����擾����
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
        /// Player���̗͂L����
        /// </summary>
        public void PlayerEnable()
        {
            _InputSystemActions?.Player.Enable();
            _InputSystemActions?.UI.Disable();
        }

        /// <summary>
        /// UI���̗͂L����
        /// </summary>
        public void UIEnable()
        {
            _InputSystemActions?.Player.Disable();
            _InputSystemActions?.UI.Enable();
        }

        /// <summary>
        /// Player���̖͂�����
        /// </summary>
        public void PlayerDisable()
        {
            _InputSystemActions?.Player.Disable();
            Debug.Log("�v���C���[���얳����");
        }

        /// <summary>
        /// UI���̗͂L����
        /// </summary>
        public void UIDisable()
        {
            _InputSystemActions?.UI.Disable();
            Debug.Log("UI���얳����");
        }
    }
}