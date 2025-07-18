using Common;
using Cysharp.Threading.Tasks;
using InGame.Model;
using InGame.NonMVP;
using Title.Loader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pose
{
    /// <summary>
    /// PoseのButtonイベント設定。
    /// </summary>
    public class PoseEventer : MonoBehaviour
    {
        [SerializeField]
        private Button PlayButton;
        [SerializeField]
        private Button RestartButton;
        [SerializeField]
        private Button TitleButton;

        public void Awake()
        {
            AddHoverEvents(PlayButton);
            AddHoverEvents(RestartButton);
        }

        public void Play()
        {

        }


        /// <summary>
        /// 実行処理（OnClick用）
        /// </summary>
        /// <param name="button"></param>
        private void ButttonEvents(Button button)
        {
            switch (button)
            {
                case var _ when button == PlayButton:
                    break;
                case var _ when button == RestartButton:
                    break;
            }
        }

        /// <summary>
        /// 画面用
        /// </summary>
        /// <param name="target"></param>
        private void AddHoverEvents(Button target)
        {
            // EventTrigger がなければ追加
            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.gameObject.AddComponent<EventTrigger>();
            }

            // ポインターが入ったとき
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => {
                Debug.Log("マウスがPlayボタンに乗った！");
            });
            trigger.triggers.Add(entryEnter);

            // ポインターが出たとき（任意）
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => {
                Debug.Log("マウスがPlayボタンから外れた！");
            });
            trigger.triggers.Add(entryExit);
        }

        //実行処理
        private void OnPlay()
        {
            SceneManager.Instance().UnloadSubScene().Forget();
            //TimeScale変更
            TimeManager.Instance().SetTimeScale(1);
        }
        private void OnRestart()
        {
            SceneManager.Instance().LoadMainScene(new InGameSceneLoader()).Forget();
        }

        private void OnTitle()
        {
            SceneManager.Instance().LoadMainScene(new TitleSceneLoader()).Forget();
        }
    }
}
