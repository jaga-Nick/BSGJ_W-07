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
        
        [Header("HP"), SerializeField]
        private int _hp = 250;
        [Header("スピード"), SerializeField]
        private float _speed = 2.5f;
        
        //MotherShip統括
        private MotherShipModel Model;
        private MotherShipView View;


        private void Awake()
        {
            View = GetComponent<MotherShipView>();
            Model = GetComponent<MotherShipModel>();
            
            cameraShaker = Camera.main.GetComponent<Shaker>();
            
            Model.Initialize(_hp, _speed);
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

