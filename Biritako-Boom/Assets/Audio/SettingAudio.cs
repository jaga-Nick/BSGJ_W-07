using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SettingAudio : MonoBehaviour
{
    //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。。
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider EffectSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;
        //Effect
        audioMixer.GetFloat("Effect", out float effectVolume);
        EffectSlider.value = effectVolume;
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
