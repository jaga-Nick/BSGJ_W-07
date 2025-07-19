using UnityEngine;
using Common;
using Cysharp.Threading.Tasks;

namespace Pose
{
    public class PoseSceneLoader : ISceneInfo
    {
        string ISceneInfo.SceneName => "Pose";

        async UniTask ISceneInfo.End()
        {
            
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
