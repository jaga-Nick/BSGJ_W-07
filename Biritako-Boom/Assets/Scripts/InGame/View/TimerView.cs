using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace InGame.View
{
    [Serializable]
    public class TimerView 
    {
        [FormerlySerializedAs("Timer")] [SerializeField]
        private Image timer;
        /// <summary>
        /// 引数:時間の割合
        /// </summary>
        /// <param name="timePercent"></param>
        public void DisplayTimer(float timePercent)
        {
            timer.fillAmount = timePercent;
        }
    }
}