using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.View;

namespace InGame.Presenter
{
    public class MotherShipPresenter : MonoBehaviour
    {
        
        [Header("母艦Prefabデータ")]
        //[SerializeField]
        //private GameObject CharacterPrefab;
        [SerializeField]
        private string CharacterAddress="MotherShip";
        
        //MotherShip統括
        private MotherShipModel Model;
        private MotherShipView View;


        private void Awake()
        {
            View = GetComponent<MotherShipView>();
            Model = GetComponent<MotherShipModel>();
            Model.Initialize();
            Model.SetRb(View.GetRb());
        }
        
        private void Start()
        {
            Model.FindTargets();
            Model.StartPatrol();
        }
        
        private void Update()
        {
            Model.Move();
        }
    }
}

