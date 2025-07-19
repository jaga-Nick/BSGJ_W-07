using UnityEngine;
using Common;
using Cysharp.Threading.Tasks;
using InGame.NonMVP;

namespace Pose
{
    public class PauseSceneLoader : ISceneInfo
    {
        string ISceneInfo.SceneName => "Pause";

        async UniTask ISceneInfo.End()
        {
            TimeManager.Instance().SetTimeScale(1);
        }

        async UniTask ISceneInfo.Init()
        {
            
        }

        void ISceneInfo.InputStart()
        {
            InputSystemActionsManager.Instance().UIEnable();
        }

        void ISceneInfo.InputStop()
        {
            InputSystemActionsManager.Instance().UIDisable();
        }
    }
}
