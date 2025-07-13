using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.Presenter; // CameraPresenterの名前空間を追加

public class test2 : MonoBehaviour
{
    [Header("母艦Prefabデータ")]
    [SerializeField]
    private string CharacterAddress="Enemy_MotherShip";
    
    private MotherShipModel Model;
    
    
    private PlayerModel Model2;
    
    [Header("母艦生成するか")]
    public bool Ins1 = false;

    

    
    private void Awake()
    {
        Model = GetComponent<MotherShipModel>();
        Model2 = new PlayerModel();
        

    }
        
    private async void Start()
    {

        if (Ins1)
        {
            await Model.GenerateMotherShip(CharacterAddress, Vector3.zero, CancellationToken.None);
        }
        
        
    }

}