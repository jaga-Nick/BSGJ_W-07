using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Settings
{
    public class SettingAudio : MonoBehaviour
    {
        //Audioミキサーを入れるとこです
        [SerializeField] AudioMixer audioMixer;

        //それぞれのスライダーを入れるとこです。。
        [FormerlySerializedAs("BGMSlider")] [SerializeField] Slider bgmSlider;
        [FormerlySerializedAs("EffectSlider")] [SerializeField] Slider effectSlider;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            //ミキサーのvolumeにスライダーのvolumeを入れてます。

            //BGM
            audioMixer.GetFloat("BGM", out float bgmVolume);
            bgmSlider.value = bgmVolume;
            //Effect
            audioMixer.GetFloat("Effect", out float effectVolume);
            effectSlider.value = effectVolume;
        }

        public void SetBGM(float volume)
        {
            audioMixer.SetFloat("BGM", volume);
        }

        public void SetEffect(float volume)
        {
            audioMixer.SetFloat("Effect", volume);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
