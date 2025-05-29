using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public AudioMixer audioMixer;   // AudioMixerをUnityで設定
    public Slider volumeSlider;     // VolumeSliderをUnityで設定

    void Start()
    {
        // スライダーが動いた時に音量を変更する関数を呼ぶようにする
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float sliderValue)
    {
        // intに変換（UI側はfloatでも値は）
        int intVolume = Mathf.RoundToInt(sliderValue);

        // 0~100 → -80~0dBに変換（100 = 0dB, 0 = -80dB）
        float dB = Mathf.Lerp(-80f, 0f, intVolume / 100f);

        // AudioMixerに設定
        audioMixer.SetFloat("BGMVolume", dB);
        
    }
}
