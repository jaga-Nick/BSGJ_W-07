using Common;
using Cysharp.Threading.Tasks;
using Setting;
using UnityEngine;

public class TitleSceneLoader : ISceneInfo
{
    public string SceneName => "Title";

    public bool IsDefault => true;

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
        AudioManager.Instance().LoadBgm("BgmTitle");
    }

    public void InputStart()
    {
    }

    public void InputStop()
    {
        AudioManager.Instance().StopBgm();
    }
}
