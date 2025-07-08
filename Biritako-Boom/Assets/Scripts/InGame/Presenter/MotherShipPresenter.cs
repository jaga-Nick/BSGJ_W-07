using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.View;
using ShakeEffect;

namespace InGame.Presenter
{
    public class MotherShipPresenter : MonoBehaviour
    {
        
        [Header("Cameraシェイク")]
        [SerializeField] private Shaker cameraShaker;
        [SerializeField] private ShakePreset explosionShake;
        
        //MotherShip統括
        private MotherShipModel Model;
        private MotherShipView View;


        private void Awake()
        {
            View = GetComponent<MotherShipView>();
            Model = GetComponent<MotherShipModel>();
            
            cameraShaker = Camera.main.GetComponent<Shaker>();
            
            Model.Initialize();
            Model.SetRb(View.GetRb());
        }
        
        private void Start()
        {
            Model.SetShaker(cameraShaker);
            Model.SetShakePreset(explosionShake);
            
            Model.FindTargets();
            Model.StartPatrol();
        }
        
        private void Update()
        {
            Model.Move();
        }
    }
}

