using UnityEngine;
using UnityEngine.EventSystems;

namespace Title.Button
{
    /// <summary>
    /// ボタンのエフェクト
    /// </summary>
    public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// animatorとbutton
        /// </summary>
        private Animator _animator;
        private UnityEngine.UI.Button _button;
        
        /// <summary>
        /// animatorのハッシュ化された状態
        /// </summary>
        private static readonly int Highlighted = Animator.StringToHash("Highlighted");
        private static readonly int Normal = Animator.StringToHash("Normal");
        private static readonly int Pressed = Animator.StringToHash("Pressed");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _button = GetComponent<UnityEngine.UI.Button>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _animator.SetTrigger(Highlighted);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _animator.SetTrigger(Normal);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _animator.SetTrigger(Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _animator.SetTrigger(Highlighted);
        }
    }
}