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
        private int hp = 250;
        [Header("スピード"), SerializeField]
        private float speed = 2.5f;
        
        //MotherShip統括
        private MotherShipModel model;
        private MotherShipView view;


        private void Awake()
        {
            view = GetComponent<MotherShipView>();
            model = GetComponent<MotherShipModel>();
            
            cameraShaker = Camera.main.GetComponent<Shaker>();
            
            model.Initialize(hp, speed);
            model.SetRb(view.GetRb());
        }
        
        private void Start()
        {
            model.SetShaker(cameraShaker);
            model.SetShakePreset(explosionShake);
            
            model.FindTargets();
            model.StartPatrol();
        }
        
        private void Update()
        {
            model.Move();
        }
        
        public MotherShipModel GetModel() { return model; }
    }
}

