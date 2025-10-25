using System.Threading;
using UnityEngine;
using InGame.Model;
using InGame.Presenter;
using UnityEngine.Serialization; // CameraPresenterの名前空間を追加

public class Test2 : MonoBehaviour
{
    [FormerlySerializedAs("CharacterAddress")]
    [Header("母艦Prefabデータ")]
    [SerializeField]
    private string characterAddress="Enemy_MotherShip";
    
    private MotherShipModel _model;
    
    
    private PlayerModel _model2;
    
    [FormerlySerializedAs("Ins1")] [Header("母艦生成するか")]
    public bool ins1 = false;

    

    
    private void Awake()
    {
        _model = GetComponent<MotherShipModel>();
        _model2 = new PlayerModel();
        

    }
        
    private async void Start()
    {

        if (ins1)
        {
            await _model.GenerateMotherShip(characterAddress, Vector3.zero, CancellationToken.None);
        }
        
        
    }

}