using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// このクラスは、ゲーム内の音量設定（マスター・BGM・効果音）を管理します。
// ユーザーがスライダーを使って音量を調整できるようにします。
public class SettingManager : MonoBehaviour
{
    // AudioMixer は Unity の音のミキサーです。
    // ここで音量を調整するためのパラメータを操作します。
    [SerializeField] private AudioMixer audioMixer;

    // 各スライダーは UI 上でユーザーが操作する部分です。
    // それぞれ、マスター音量・BGM音量・効果音音量を担当します。
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider effectSlider;

    // 音を再生するための AudioSource を設定
    [SerializeField] private AudioSource masterSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource effectSource;

    // UIボタンから呼び出す関数
    public void PlayMasterSound()
    {
        if (masterSource != null)
            masterSource.Play();
    }

    public void PlayBGMSound()
    {
        if (bgmSource != null)
            bgmSource.Play();
    }

    public void PlayEffectSound()
    {
       if (effectSource != null)
          effectSource.Play();
    }


    // ゲーム開始時に呼ばれる関数
    void Start()
    {
        // 各スライダーの設定（最小値・最大値・整数値のみ）を行います。
        SetupSlider(masterSlider);
        SetupSlider(bgmSlider);
        SetupSlider(effectSlider);

        // AudioMixer に現在設定されている音量を取得し、
        // スライダーの初期値として反映します。
        SyncSliderWithMixer(masterSlider, "MasterVolume");
        SyncSliderWithMixer(bgmSlider, "BGMVolume");
        SyncSliderWithMixer(effectSlider, "EffectVolume");

        // スライダーが動いたときに、それぞれの音量を変更する関数を登録します。
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        effectSlider.onValueChanged.AddListener(OnEffectVolumeChanged);
    }

    // スライダーの基本設定を行う関数
    // 最小値0、最大値100、整数値のみを扱うようにします。
    void SetupSlider(Slider slider)
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.wholeNumbers = true;
    }

    // AudioMixer に現在設定されている音量（dB）を取得し、
    // スライダーの値（0〜100）に変換して反映します。
    void SyncSliderWithMixer(Slider slider, string parameterName)
    {
        float dB;
        // AudioMixer から音量（dB）を取得
        if (audioMixer.GetFloat(parameterName, out dB))
        {
            // dB（-80〜0）をスライダーの値（0〜100）に変換
            float value = Mathf.InverseLerp(-80f, 0f, dB) * 100f;
            slider.value = Mathf.Round(value); // 小数点を四捨五入
        }
        else
        {
            // 値が取得できなかった場合は、スライダーを50に初期化
            slider.value = 50;
        }
    }

    // 以下は、スライダーが動いたときに呼ばれる関数です。
    // それぞれの音量パラメータに対応しています。

    void OnMasterVolumeChanged(float value)
    {
        SetVolume("MasterVolume", value);
    }

    void OnBGMVolumeChanged(float value)
    {
        SetVolume("BGMVolume", value);
    }

    void OnEffectVolumeChanged(float value)
    {
        SetVolume("EffectVolume", value);
    }

    // スライダーの値（0〜100）を dB（-80〜0）に変換し、
    // AudioMixer に設定する共通関数です。
    void SetVolume(string parameterName, float sliderValue)
    {
        int intVolume = Mathf.RoundToInt(sliderValue); // 小数点を整数に変換
        float dB = Mathf.Lerp(-80f, 0f, intVolume / 100f); // 線形補間でdBに変換
        audioMixer.SetFloat(parameterName, dB); // AudioMixerに設定
    }
}
