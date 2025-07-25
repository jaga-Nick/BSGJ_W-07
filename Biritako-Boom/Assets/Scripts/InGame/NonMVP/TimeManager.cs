﻿using System;
using Common;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using Ending.Loader;
namespace InGame.NonMVP
{
    /// <summary>
    /// TimeManager
    /// ゲーム内の時間を管理するクラス。
    /// </summary>
    public class TimeManager : DestroyAvailable_SingletonMonoBehaviourBase<TimeManager>
    {
        /// <summary>
        /// ゲーム状態のパラメタ
        /// </summary>
        private float _inGameTime;
        private bool _isPaused;

        /// <summary>
        /// 常時タイマーを減らしていく
        /// </summary>
        private void Update()
        {
            if (!_isPaused)
            {
                _inGameTime -= Time.deltaTime;
            }
            //0以下の時、ゲームを終了する
            if (_inGameTime <= 0)
            {
                ISceneInfo info = new ResultSceneLoader();
                //SceneManager.Instance().LoadMainScene(info).Forget();
            }
        }

        /// <summary>
        /// 時間を変更する。
        /// </summary>
        /// <param name="time"></param>
        public void SetTimeScale(float time)
        {
            Time.timeScale = time;
        }

        /// <summary>
        /// ゲーム時間の取得
        /// </summary>
        /// <returns></returns>
        public float GetInGameTime()
        {
            return _inGameTime;
        }

        /// <summary>
        /// ゲーム時間をリセットする
        /// </summary>
        public void ResetInGameTime(float inGameSecond)
        {
            _inGameTime = inGameSecond;
        }

        /// <summary>
        /// ゲーム時間を加算する
        /// </summary>
        /// <param name="time"></param>
        public void AddInGameTime(float time)
        {
            _inGameTime += time;
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// 再開
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// 一時停止状態かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsPaused()
        {
            return _isPaused;
        }
    }
}