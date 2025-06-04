using System;
using UnityEngine;
using UnityEngine.UI;
namespace InGame.View
{
    [Serializable]
    public class TimerView 
    {
        [SerializeField]
        private Image Timer;
        /// <summary>
        /// 引数:時間の割合
        /// </summary>
        /// <param name="TimePercent"></param>
        public void DisplayTimer(float TimePercent)
        {
            Timer.fillAmount = TimePercent;
        }
    }
}