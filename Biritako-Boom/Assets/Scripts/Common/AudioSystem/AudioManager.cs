using System.Collections.Generic;
using Common.GameSystem;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.AudioSystem
{
    /// <summary>
    /// BGMとSEの管理をするマネージャー
    /// </summary>
    public class AudioManager : SingletonMonoBehaviourBase<AudioManager>
    {
        /// <summary>
        /// ボリューム保存用のkeyとデフォルト値
        /// </summary>
        private const string BGM_VOLUME_KEY = "BGM_VOLUME_KEY";

        private const string SE_VOLUME_KEY = "SE_VOLUME_KEY";
        private const string AMBIENT_VOLUME_KEY = "AMBIENT_VOLUME_KEY";
        private const float BGM_VOLUME_DEFULT = 1.0f;
        private const float SE_VOLUME_DEFULT = 1.0f;
        private const float AMBIENT_VOLUME_DEFULT = 1.0f;

        /// <summary>
        /// BGMがフェードするのにかかる時間
        /// </summary>
        private const float BGMFadeSpeedRateHigh = 0.9f;

        private const float BGMFadeSpeedRateLow = 0.3f;
        private float _bgmFadeSpeedRate = BGMFadeSpeedRateHigh;

        /// <summary>
        /// AudioMixer
        /// </summary>
        [SerializeField] private AudioMixer audioMixer;

        /// <summary>
        /// 全Audioを保持する
        /// </summary>
        private Dictionary<string, AudioClip> _bgmDictionary, _seDictionary;

        /// <summary>
        /// BGMとSEのAudioSourceを保持する
        /// </summary>
        public AudioSource attachBGMSource, attachSeSource;

        /// <summary>
        /// 次流すBGM名、SE名、環境音名
        /// </summary>
        private string _nextBGMName;

        private string _nextSeName;
        private string _nextAmbientName;

        /// <summary>
        /// BGMをフェードアウト中か
        /// </summary>
        private bool _isFadeOut = false;


        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();

            // リソースフォルダから全SE&BGM&環境音を読み込み初期化
            _bgmDictionary = new Dictionary<string, AudioClip>();
            _seDictionary = new Dictionary<string, AudioClip>();

            var bgmResources = Resources.LoadAll("Audio/BGM");
            var seResources = Resources.LoadAll("Audio/SE");

            foreach (var bgm in bgmResources)
            {
                _bgmDictionary[bgm.name] = (AudioClip)bgm;
            }

            foreach (var se in seResources)
            {
                _seDictionary[se.name] = (AudioClip)se;
            }
        }
        
        /// <summary>
        /// 指定したBGMを流す。
        /// ただし既に流れている場合は前の曲をフェードアウトさせてから。
        /// 第二引数のfadeSpeedRateに指定した割合でフェードアウトするスピードが変わる。
        /// </summary>
        /// <param name="bgmName"></param>
        /// <param name="fadeSpeed"></param>
        public void PlayBGM(string bgmName, float fadeSpeed = BGMFadeSpeedRateHigh)
        {
            // BGM名が空文字列の場合は何もしない
            if (!_bgmDictionary.ContainsKey(bgmName))
            {
                Debug.LogError($"{bgmName} というBGMは存在しません。");
                return;
            }
            
            // 現在のBGMが流れていないときはそのまま流す
            if (!attachBGMSource.isPlaying)
            {
                _nextBGMName = "";
                attachBGMSource.clip = _bgmDictionary[bgmName] as AudioClip;
                attachBGMSource.loop = true;
                attachBGMSource.Play();
            }
            // すでにBGMが流れている場合はフェードアウトさせてから新しいBGMを流す
            else if (_nextBGMName != bgmName)
            {
                _nextBGMName = bgmName;
                FadeOutBGM(fadeSpeed);
            }
        }
        
        
        /// <summary>
        /// 指定したSEを流す。
        /// delayを指定するとその時間だけ遅延して再生する。
        /// </summary>
        /// <param name="seName"></param>
        /// <param name="delay"></param>
        public void PlaySe(string seName, float delay = 0.0f)
        {
            // SE名が空文字列の場合は何もしない
            if (!_seDictionary.ContainsKey(seName))
            {
                Debug.LogError($"{seName} というSEは存在しません。");
                return;
            }
            
            _nextSeName = seName;
            // 遅延を指定している場合はInvokeで遅延させて再生
            Invoke(nameof(DelayPlaySe), delay);
        }

        private void DelayPlaySe()
        {
            attachSeSource.PlayOneShot(_seDictionary[_nextSeName] as AudioClip);
        }
        
        /// <summary>
        /// 現在流れている曲をフェードアウトさせる。
        /// </summary>
        /// <param name="fadeSpeed"></param>
        public void FadeOutBGM(float fadeSpeed = BGMFadeSpeedRateLow)
        {
            _bgmFadeSpeedRate = fadeSpeed;
            _isFadeOut = true;
        }

        private void Update()
        {
            if (!_isFadeOut) { return; }
            // 徐々にボリュームを下げていき、ボリュームが0を戻し次の曲を流す
            attachBGMSource.volume -= _bgmFadeSpeedRate * Time.deltaTime;
            if (attachBGMSource.volume <= 0.0f)
            {
                attachBGMSource.Stop();
                attachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
                _isFadeOut = false;
                if (!string.IsNullOrEmpty(_nextBGMName))
                {
                    PlayBGM(_nextBGMName);
                }
            }
        }

        /// <summary>
        /// BGMの音量を変更して保存する。スライダー用。
        /// </summary>
        /// <param name="volume"></param>
        public void SetBGMVolume(float volume)
        {
            var db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
            audioMixer.SetFloat("BGM", db);
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// SEの音量を変更して保存する。スライダー用。
        /// </summary>
        /// <param name="volume"></param>
        public void SetSeVolume(float volume)
        {
            var db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
            audioMixer.SetFloat("SE", db);
            PlayerPrefs.SetFloat(SE_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }
    }
}