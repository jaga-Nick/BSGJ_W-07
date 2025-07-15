using UnityEngine;
using Common;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Setting
{
    /// <summary>
    /// AudioManager
    /// </summary>
    public class AudioManager : SingletonMonoBehaviourBase<AudioManager>
    {
        //BGM
        private AudioTokenPackage Bgm;
        //SE
        private AudioTokenPackage SoundEffectOne;
        private AudioTokenPackage SoundEffectTwo;


        AsyncOperationHandle<AudioMixer> handle;
        AudioMixer mixer;
        void Awake()
        {
            Bgm = new AudioTokenPackage(gameObject.AddComponent<AudioSource>());
            Bgm.LoopOnOff(true);
            SoundEffectOne = new AudioTokenPackage(gameObject.AddComponent<AudioSource>());
            SoundEffectTwo = new AudioTokenPackage(gameObject.AddComponent<AudioSource>());

            LoadAudioMixer().Forget();
        }
        
        public async UniTask LoadAudioMixer()
        {
            handle=Addressables.LoadAssetAsync<AudioMixer>("AudioMixer");
            mixer=await handle;

            Bgm.audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            SoundEffectOne.audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Effect")[0];
            SoundEffectTwo.audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Effect")[0];
        }

        public void LoadBgm(string Address)
        {
            Bgm.Load(Address).Forget();
        }

        public void StopBgm()
        {
            Bgm.Stop().Forget();
        }
        public void LoadSoundEffect(string Address)
        {
            SoundEffectOne.Load(Address).Forget();
        }

        private void OnDestroy()
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
            }
        }

        /*

        /// <summary>
        /// Bgmをかける
        /// </summary>
        /// <param name="Num"></param>
        public async UniTask LoadBgm(string SoundAddress)
        {
            try
            {
                //再生を終了
                await StopBgm();

                BgmToken?.Cancel();
                BgmToken?.Dispose();
                BgmToken = new CancellationTokenSource();


                handleBgm = Addressables.LoadAssetAsync<AudioClip>(SoundAddress);
                AudioClip audio = await handleBgm;
                AudioSourceBgm.clip = audio;
                AudioSourceBgm.Play();
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                BgmToken?.Dispose();
                BgmToken = null;
            }
            
        }
        public async UniTask StopBgm()
        {
            AudioSourceBgm.Stop();
            AudioSourceBgm.clip = null;
            if (handleBgm.IsValid() && handleBgm.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handleBgm);
            }
        }

        /// <summary>
        /// 一斉に音（SE）をかけるのは三つまでとする。
        /// </summary>
        /// <param name="SoundAddress"></param>
        public void LoadSoundEffect(string SoundAddress)
        {
            
        }

        */
    }
}


public class AudioTokenPackage
{
    public AudioTokenPackage(AudioSource _audioSource)
    {
        this.audioSource = _audioSource;
    }
    public AudioSource audioSource { get; private set;}
    private CancellationTokenSource token;
    AsyncOperationHandle<AudioClip> handle;

    public void LoopOnOff(bool onoff)
    {
        audioSource.loop = onoff;
    }

    public async UniTask Load(string SoundAddress)
    {
        try
        {
            //再生を終了
            await Stop();

            token?.Cancel();
            token?.Dispose();
            token = new CancellationTokenSource();


            handle = Addressables.LoadAssetAsync<AudioClip>(SoundAddress);
            AudioClip audio = await handle;
            audioSource.clip = audio;
            audioSource.Play();
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            token?.Dispose();
            token = null;
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
            }
        }
    }

    public async UniTask Stop()
    {
        audioSource.Stop();
        audioSource.clip = null;
        if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(handle);
        }
    }
}