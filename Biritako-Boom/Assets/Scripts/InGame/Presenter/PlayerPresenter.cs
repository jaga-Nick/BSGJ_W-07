using UnityEngine;
using InGame.Model;
using InGame.View;
using System.Security.Cryptography.X509Certificates;

namespace InGame.Presenter
{
    /// <summary>
    /// Playerのpresenter
    /// </summary>
    public class PlayerPresenter:MonoBehaviour
    {
        private PlayerModel Model;
        private PlayerView View;
        
        private void Update()
        {
            //View.OnChangedCodeGauge(Model.GetCodeGaugePercent());
        }
    }
}
