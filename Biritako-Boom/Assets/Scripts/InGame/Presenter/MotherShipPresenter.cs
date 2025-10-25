using System.Threading;
using Common.ShakeEffectSetting;
using UnityEngine;
using InGame.Model;
using InGame.View;
using UnityEngine.Serialization;

namespace InGame.Presenter
{
    public class MotherShipPresenter : MonoBehaviour
    {
        
        [Header("Cameraシェイク")]
        [SerializeField] private Shaker cameraShaker;
        [SerializeField] private ShakePreset explosionShake;
        
        [FormerlySerializedAs("_hp")] [Header("HP"), SerializeField]
        private int hp = 250;
        [FormerlySerializedAs("_speed")] [Header("スピード"), SerializeField]
        private float speed = 2.5f;
        
        //MotherShip統括
        private MotherShipModel _model;
        private MotherShipView _view;


        private void Awake()
        {
            _view = GetComponent<MotherShipView>();
            _model = GetComponent<MotherShipModel>();
            
            cameraShaker = Camera.main.GetComponent<Shaker>();
            
            _model.Initialize(hp, speed);
            _model.SetRb(_view.GetRb());
        }
        
        private void Start()
        {
            _model.SetShaker(cameraShaker);
            _model.SetShakePreset(explosionShake);
            
            _model.FindTargets();
            _model.StartPatrol();
        }
        
        private void Update()
        {
            _model.Move();
        }
        
        public MotherShipModel GetModel() { return _model; }
    }
}

