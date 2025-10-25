using Common;
using Common.AudioSystem;
using Common.SceneSystem;
using Cysharp.Threading.Tasks;
using InGame.Model;
using InGame.NonMVP;
using UnityEngine;

namespace Title.Loader
{
    public class InGameSceneLoader : ISceneInfo
    {
        public string SceneName => "InGame";

        public bool IsDefault => throw new System.NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public async UniTask End()
        {
            Debug.Log("テスト_end");
        }

        /// <summary>
        /// 
        /// </summary>
        public async UniTask Init()
        {
            ScoreModel.Instance.RestoreScore();
            AudioManager.Instance.PlayBGM(AUDIO.BGM_TITLE);
            TimeManager.Instance().SetTimeScale(1);
        }

        public void InputStart()
        {
            InputSystemActionsManager.Instance.PlayerEnable();
        }

        public void InputStop()
        {
        }
    }
}