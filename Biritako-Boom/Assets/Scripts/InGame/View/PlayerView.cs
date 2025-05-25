using UnityEngine;
using UnityEngine.UI;


namespace InGame.View
{
    /// <summary>
    /// Playerデータ表示
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        //延長コード
        [SerializeField]
        private Image CodeGauge;

        public void OnChangedCodeGauge(float GaugePercent)
        {
            CodeGauge.fillAmount = GaugePercent;
        } 
    }
}