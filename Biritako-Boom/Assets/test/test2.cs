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


    [Header("Prefabデータ")]
    [SerializeField] 
    private string CharacterAddress2="Player";
    
    private PlayerModel Model2;
    
    [Header("母艦生成するか")]
    public bool Ins1 = false;
    [Header("Player生成するか")]
    public bool Ins2 = false;
    
    // ★ CameraPresenterへの参照を保持するフィールドを追加
    private CameraPresenter _cameraPresenter;
    
    private void Awake()
    {
        Model = GetComponent<MotherShipModel>();
        Model2 = new PlayerModel();
        
        // ★ シーン内からCameraPresenterを探して取得しておく
        _cameraPresenter = FindObjectOfType<CameraPresenter>();
    }
        
    private async void Start()
    {
        if (Ins2)
        {
            // ★ 生成したプレイヤーインスタンスを受け取る変数を宣言
                    GameObject playerInstance = await Model2.AddressGeneratePlayerCharacter(CharacterAddress2, CancellationToken.None);
            
                    // ★ 生成が完了したPlayerのTransformをCameraPresenterに直接渡す
                    if (playerInstance != null && _cameraPresenter != null)
                    {
                        _cameraPresenter.SetTarget(playerInstance.transform);
                    }
        }


        if (Ins1)
        {
            await Model.GenerateMotherShip(CharacterAddress, Vector3.zero, CancellationToken.None);
        }
        
        
    }
        
    private void Update()
    {
            
    }
}