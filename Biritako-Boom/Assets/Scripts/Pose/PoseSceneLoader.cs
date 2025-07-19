using UnityEngine;
using Common;
using Cysharp.Threading.Tasks;

namespace Pose
{
    public class PoseSceneLoader : ISceneInfo
    {
        string ISceneInfo.SceneName => "Pose";

        UniTask ISceneInfo.End()
        {
            throw new System.NotImplementedException();
        }

        UniTask ISceneInfo.Init()
        {
            throw new System.NotImplementedException();
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
