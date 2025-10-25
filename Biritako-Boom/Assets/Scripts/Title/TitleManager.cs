using Common.AudioSystem;
using UnityEngine;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.Instance.PlayBGM(AUDIO.BGM_TITLE);
        }
    }
}