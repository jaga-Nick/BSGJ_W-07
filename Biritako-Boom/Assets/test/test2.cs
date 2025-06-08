using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.View;

public class test2 : MonoBehaviour
{
    [Header("母艦Prefabデータ")]
    [SerializeField]
    private string CharacterAddress="Enemy_MotherShip";
    
    private MotherShipModel Model;
    
    



    private void Awake()
    {
        Model = GetComponent<MotherShipModel>();
    }
        
    private void Start()
    {
        Model.GenerateMotherShip(CharacterAddress, Vector3.zero, CancellationToken.None);
    }
        
    private void Update()
    {
            
    }
}
