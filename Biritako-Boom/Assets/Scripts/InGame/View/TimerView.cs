using UnityEngine;
using UnityEngine.UI;
namespace InGame.View
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField]
        private Image Timer;
        public void DisplayTimer(float TimePercent)
        {
            Timer.fillAmount = TimePercent;
        }
    }
}