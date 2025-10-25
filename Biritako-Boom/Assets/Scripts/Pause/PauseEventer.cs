using Common;
using Common.SceneSystem;
using Cysharp.Threading.Tasks;
using InGame.NonMVP;
using Title.Loader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pause
{
    /// <summary>
    /// PoseのButtonイベント設定。
    /// </summary>
    public class PauseEventer : MonoBehaviour
    {
        private static readonly int Highlighted = Animator.StringToHash("Highlighted");
        private static readonly int Selected = Animator.StringToHash("Selected");
        private static readonly int Normal = Animator.StringToHash("Normal");
        private static readonly int Pressed = Animator.StringToHash("Pressed");


        [FormerlySerializedAs("PlayButton")] [SerializeField]
        private Button playButton;
        [FormerlySerializedAs("RestartButton")] [SerializeField]
        private Button restartButton;
        [FormerlySerializedAs("TitleButton")] [SerializeField]
        private Button titleButton;

        private InputSystem_Actions _action;


        private Button[] _buttons;
        private Button _previousButton;
        private int _currentIndex = 0;
        private float _navigateCooldown = 0.2f;
        private float _lastNavigateTime = 0f;

        public void Awake()
        {
            AddHoverEvents(playButton);
            AddHoverEvents(restartButton);
            AddHoverEvents(titleButton);

            playButton.onClick.AddListener(() => { ButtonEvents(playButton); });
            restartButton.onClick.AddListener(() => { ButtonEvents(restartButton); });
            titleButton.onClick.AddListener(() => { ButtonEvents(titleButton); });


            _action = InputSystemActionsManager.Instance.GetInputSystem_Actions();
            InputSystemActionsManager.Instance.UIEnable();

            _buttons = new Button[] { playButton, restartButton, titleButton };
            SelectButton(_buttons[_currentIndex]);

            foreach (var button in _buttons)
            {
                Animator animator = button.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
        }

        public void Update()
        {
            Vector2 nav = _action.UI.Navigate.ReadValue<Vector2>();
            // 時間経過チェック（入力連打防止）
            if (Time.unscaledTime - _lastNavigateTime < _navigateCooldown) return;

            if (nav.y > 0.5f) // 上方向
            {
                _currentIndex = (_currentIndex - 1 + _buttons.Length) % _buttons.Length;
                SelectButton(_buttons[_currentIndex]);
                _lastNavigateTime = Time.unscaledTime;
            }
            else if (nav.y < -0.5f) // 下方向
            {
                _currentIndex = (_currentIndex + 1) % _buttons.Length;
                SelectButton(_buttons[_currentIndex]);
                _lastNavigateTime = Time.unscaledTime;
            }

            if (_action.UI.Submit.WasPressedThisFrame())
            {
                ButtonEvents(_buttons[_currentIndex]);
            }
        }

        private void SelectButton(Button button)
        {
            // 前のボタンをNormalに戻す
            if (_previousButton != null && _previousButton != button)
            {
                Animator prevAnimator = _previousButton.GetComponent<Animator>();
                if (prevAnimator != null)
                {
                    prevAnimator.SetTrigger(Normal);
                }
            }

            // 現在のボタンを選択状態に
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            button.Select();

            Animator animator = button.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(Highlighted);
            }

            // 次回のために記録
            _previousButton = button;
        }


        /// <summary>
        /// 実行処理（OnClick用）
        /// </summary>
        /// <param name="button"></param>
        private void ButtonEvents(Button button)
        {
            switch (button)
            {
                case var _ when button == playButton:
                    OnPlay();
                    break;
                case var _ when button == restartButton:
                    OnRestart();
                    break;
                case var _ when button == titleButton:
                    OnTitle();
                    break;
            }
        }

        /// <summary>
        /// ButtonのAnimation
        /// </summary>
        /// <param name="target"></param>
        private void AddHoverEvents(Button target)
        {
            Animator animator=target.gameObject.GetComponent<Animator>();
            // EventTrigger がなければ追加
            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.gameObject.AddComponent<EventTrigger>();
            }

            //PointerEnter
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => {
                animator.SetTrigger(Highlighted);
            });
            trigger.triggers.Add(entryEnter);

            //PointerUp
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerDown;
            entryUp.callback.AddListener((eventData) => {
                animator.SetTrigger(Highlighted);
            });
            trigger.triggers.Add(entryUp);

            //PointerDown
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((eventData) => {
                animator.SetTrigger(Pressed);
            });
            trigger.triggers.Add(entryDown);

            //PointerExit
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => {
                animator.SetTrigger(Normal);
            });
            trigger.triggers.Add(entryExit);
        }

        //実行処理
        private void OnPlay()
        {
            //TimeScale変更
            TimeManager.Instance().SetTimeScale(1);
            SceneManager.Instance.UnloadSubScene().Forget();
        }
        private void OnRestart()
        {
            TimeManager.Instance().SetTimeScale(1);
            SceneManager.Instance.LoadMainScene(new InGameSceneLoader()).Forget();
        }
        private void OnTitle()
        {
            TimeManager.Instance().SetTimeScale(1);
            SceneManager.Instance.LoadMainScene(new TitleSceneLoader()).Forget();
        }
    }
}
