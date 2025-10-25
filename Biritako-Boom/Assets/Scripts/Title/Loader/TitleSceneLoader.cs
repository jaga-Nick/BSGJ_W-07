using Common.AudioSystem;
using Common.SceneSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using InGame.Model;

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
        ScoreModel.Instance.RestoreScore();

        AudioManager.Instance.PlayBGM(AUDIO.BGM_TITLE);
    }

    public void InputStart()
    {
    }

    public void InputStop()
    {
    }
}
