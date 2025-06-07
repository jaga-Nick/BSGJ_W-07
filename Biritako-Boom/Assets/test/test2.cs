using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.View;

public class test2 : MonoBehaviour
{
    [Header("母艦Prefabデータ")]
    [SerializeField]
    private GameObject CharacterPrefab;
    [SerializeField]
    private string CharacterAddress="Enemy_MotherShip";
        
    //MotherShip統括
    private MotherShipModel Model;
    private MotherShipView View;


    private void Awake()
    {
        View = new MotherShipView();
        Model = new MotherShipModel(View.GetRb());
    }
        
    private void Start()
    {
        Model.GenerateMotherShip(CharacterAddress, Vector3.zero, CancellationToken.None);
    }
        
    private void Update()
    {
            
    }
}
